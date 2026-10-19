using System;
using System.Collections.Generic;
using _Project.Develop.Runtime.Configs.Meta.Impact;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters;
using _Project.Develop.Runtime.Utilities.AssetsManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Impact
{
    public sealed class HitImpactService : IDisposable
    {
        private const string PrefabPath = "Prefabs/Impact/HitImpactEffect";

        private readonly HitImpactConfig _config;
        private readonly GameObject _prefab;
        private readonly DamageableRegistry _damageables;
        private readonly Queue<HitImpactEffect> _pool = new();
        private readonly List<ActiveImpact> _active = new();
        private readonly Transform _root;
        private bool _disposed;

        public HitImpactService(
            HitImpactConfig config,
            ResourcesAssetsLoader assetsLoader,
            DamageableRegistry damageables)
        {
            _config = config;
            _damageables = damageables;
            _prefab = assetsLoader.Load<GameObject>(PrefabPath);

            GameObject rootObject = new GameObject("HitImpactPool");
            Object.DontDestroyOnLoad(rootObject);
            _root = rootObject.transform;

            for (int i = 0; i < _config.PoolSize; i++)
                _pool.Enqueue(CreateEffect());
        }

        public void Play(RaycastHit hit)
        {
            Play(hit.point, hit.normal, hit.collider);
        }

        public void Play(Vector3 point, Vector3 normal, Collider collider)
        {
            if (_disposed)
                return;

            SurfaceType surface = ResolveSurface(collider);
            HitImpactSurfaceSettings settings = _config.GetSurface(surface);
            HitImpactEffect effect = Rent();

            effect.Show(
                point,
                normal,
                settings,
                _config.DecalSize,
                _config.SurfaceOffset,
                _config.MinDecalScale);

            float particleDuration = _config.ParticleDuration;
            float lifetime = settings.SpawnDecal
                ? Mathf.Max(particleDuration, _config.DecalDuration)
                : particleDuration;

            _active.Add(new ActiveImpact
            {
                Effect = effect,
                ParticleStopTimer = particleDuration,
                Lifetime = lifetime,
                ParticlesStopped = false
            });
        }

        public void Tick(float deltaTime)
        {
            if (_disposed || _active.Count == 0)
                return;

            for (int i = _active.Count - 1; i >= 0; i--)
            {
                ActiveImpact impact = _active[i];

                if (impact.ParticlesStopped == false)
                {
                    impact.ParticleStopTimer -= deltaTime;
                    if (impact.ParticleStopTimer <= 0f)
                    {
                        impact.Effect.StopEmitting();
                        impact.ParticlesStopped = true;
                    }
                }

                impact.Lifetime -= deltaTime;
                if (impact.Lifetime > 0f)
                {
                    _active[i] = impact;
                    continue;
                }

                impact.Effect.Hide();
                _active.RemoveAt(i);
                if (_disposed == false)
                    _pool.Enqueue(impact.Effect);
            }
        }

        public void ClearActive()
        {
            for (int i = 0; i < _active.Count; i++)
            {
                HitImpactEffect effect = _active[i].Effect;
                effect.Hide();
                if (_disposed == false)
                    _pool.Enqueue(effect);
            }

            _active.Clear();
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            ClearActive();
            _pool.Clear();
            Object.Destroy(_root.gameObject);
        }

        private SurfaceType ResolveSurface(Collider collider)
        {
            if (collider == null)
                return SurfaceType.Default;

            SurfaceTag tag = collider.GetComponentInParent<SurfaceTag>();
            if (tag != null)
                return tag.Surface;

            if (_damageables.TryGet(collider, out _))
                return SurfaceType.Flesh;

            return SurfaceType.Default;
        }

        private HitImpactEffect Rent()
        {
            if (_pool.Count > 0)
                return _pool.Dequeue();

            return CreateEffect();
        }

        private HitImpactEffect CreateEffect()
        {
            GameObject instance = Object.Instantiate(_prefab, _root);
            instance.name = "HitImpact";
            return instance.GetComponent<HitImpactEffect>();
        }

        private struct ActiveImpact
        {
            public HitImpactEffect Effect;
            public float ParticleStopTimer;
            public float Lifetime;
            public bool ParticlesStopped;
        }
    }
}
