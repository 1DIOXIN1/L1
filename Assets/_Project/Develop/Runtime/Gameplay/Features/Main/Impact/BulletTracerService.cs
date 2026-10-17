using System;
using System.Collections.Generic;
using _Project.Develop.Runtime.Configs.Meta.Impact;
using _Project.Develop.Runtime.Utilities.AssetsManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Impact
{
    public sealed class BulletTracerService : IDisposable
    {
        private const string PrefabPath = "Prefabs/Impact/BulletTracer";

        private readonly BulletTracerConfig _config;
        private readonly GameObject _prefab;
        private readonly Queue<BulletTracer> _pool = new();
        private readonly List<ActiveTracer> _active = new();
        private readonly Transform _root;
        private bool _disposed;

        public BulletTracerService(BulletTracerConfig config, ResourcesAssetsLoader assetsLoader)
        {
            _config = config;
            _prefab = assetsLoader.Load<GameObject>(PrefabPath);

            GameObject rootObject = new GameObject("BulletTracerPool");
            Object.DontDestroyOnLoad(rootObject);
            _root = rootObject.transform;

            for (int i = 0; i < _config.PoolSize; i++)
                _pool.Enqueue(CreateTracer());
        }

        public void Play(Vector3 start, Vector3 end, Camera viewCamera = null)
        {
            if (_disposed)
                return;

            if ((end - start).sqrMagnitude < 0.01f)
                return;

            Vector3 tracerStart = start;
            if (viewCamera != null && _config.ViewSideOffset > 0f)
            {
                tracerStart += viewCamera.transform.right * _config.ViewSideOffset;
                tracerStart += viewCamera.transform.up * (-_config.ViewSideOffset * 0.25f);
            }

            BulletTracer tracer = Rent();
            tracer.Show(_config.Width, _config.EndWidthRatio, _config.Color);

            Vector3 path = end - tracerStart;
            float pathLength = path.magnitude;
            Vector3 direction = pathLength > 0.0001f ? path / pathLength : Vector3.forward;
            float seed = Mathf.Min(_config.SeedLength, pathLength * 0.08f);
            tracer.SetEndpoints(tracerStart, tracerStart + direction * seed);

            _active.Add(new ActiveTracer
            {
                Tracer = tracer,
                Start = tracerStart,
                End = end,
                Duration = _config.Duration,
                TrailLength = _config.TrailLength,
                HoldDuration = _config.HoldDuration,
                SeedLength = _config.SeedLength,
                Elapsed = 0f,
                Finishing = false
            });
        }

        public void Tick(float deltaTime)
        {
            if (_disposed || _active.Count == 0)
                return;

            for (int i = _active.Count - 1; i >= 0; i--)
            {
                ActiveTracer state = _active[i];
                state.Elapsed += deltaTime;

                if (state.Finishing)
                {
                    if (state.Elapsed < state.HoldDuration)
                    {
                        _active[i] = state;
                        continue;
                    }

                    FinishAt(i, state.Tracer);
                    continue;
                }

                float t = Mathf.Clamp01(state.Elapsed / state.Duration);
                float eased = 1f - (1f - t) * (1f - t);
                Vector3 head = Vector3.Lerp(state.Start, state.End, eased);
                Vector3 path = state.End - state.Start;
                float pathLength = path.magnitude;
                Vector3 direction = pathLength > 0.0001f ? path / pathLength : Vector3.forward;
                float traveled = pathLength * eased;
                Vector3 tail = head - direction * Mathf.Min(
                    state.TrailLength,
                    Mathf.Max(traveled, state.SeedLength * 0.5f));

                state.Tracer.SetEndpoints(tail, head);

                if (t < 1f)
                {
                    _active[i] = state;
                    continue;
                }

                state.Finishing = true;
                state.Elapsed = 0f;
                _active[i] = state;
            }
        }

        public void ClearActive()
        {
            for (int i = 0; i < _active.Count; i++)
            {
                BulletTracer tracer = _active[i].Tracer;
                tracer.Hide();
                if (_disposed == false)
                    _pool.Enqueue(tracer);
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

        private void FinishAt(int index, BulletTracer tracer)
        {
            tracer.Hide();
            _active.RemoveAt(index);
            if (_disposed == false)
                _pool.Enqueue(tracer);
        }

        private BulletTracer Rent()
        {
            if (_pool.Count > 0)
                return _pool.Dequeue();

            return CreateTracer();
        }

        private BulletTracer CreateTracer()
        {
            GameObject instance = Object.Instantiate(_prefab, _root);
            instance.name = "BulletTracer";
            return instance.GetComponent<BulletTracer>();
        }

        private struct ActiveTracer
        {
            public BulletTracer Tracer;
            public Vector3 Start;
            public Vector3 End;
            public float Duration;
            public float TrailLength;
            public float HoldDuration;
            public float SeedLength;
            public float Elapsed;
            public bool Finishing;
        }
    }
}
