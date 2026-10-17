using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Develop.Runtime.Configs.Meta.Impact
{
    [CreateAssetMenu(
        menuName = "Configs/Core/Gameplay/Impact/HitImpact",
        fileName = "HitImpactConfig")]
    public sealed class HitImpactConfig : ScriptableObject
    {
        [Header("Pool")]
        [SerializeField] private int poolSize = 48;

        [Header("Timing")]
        [FormerlySerializedAs("effectDuration")]
        [SerializeField] private float particleDuration = 0.35f;
        [SerializeField] private float decalDuration = 8f;

        [Header("Placement")]
        [SerializeField] private float surfaceOffset = 0.025f;
        [SerializeField] private float decalSize = 0.2f;
        [SerializeField] private float minDecalScale = 0.12f;

        [Header("Surfaces")]
        [SerializeField] private HitImpactSurfaceSettings[] surfaces =
        {
            new HitImpactSurfaceSettings(
                SurfaceType.Default,
                new Color(0.65f, 0.6f, 0.5f, 0.85f),
                new Color(0.06f, 0.05f, 0.04f, 1f),
                0.04f,
                true),
            new HitImpactSurfaceSettings(
                SurfaceType.Flesh,
                new Color(0.55f, 0.05f, 0.05f, 0.9f),
                new Color(0.35f, 0.02f, 0.02f, 0.7f),
                0.05f,
                false),
            new HitImpactSurfaceSettings(
                SurfaceType.Metal,
                new Color(1f, 0.9f, 0.55f, 0.95f),
                new Color(0.08f, 0.08f, 0.1f, 1f),
                0.035f,
                true),
            new HitImpactSurfaceSettings(
                SurfaceType.Concrete,
                new Color(0.7f, 0.68f, 0.62f, 0.85f),
                new Color(0.07f, 0.06f, 0.05f, 1f),
                0.045f,
                true),
            new HitImpactSurfaceSettings(
                SurfaceType.Wood,
                new Color(0.55f, 0.4f, 0.22f, 0.85f),
                new Color(0.1f, 0.06f, 0.03f, 1f),
                0.04f,
                true)
        };

        public int PoolSize => Mathf.Max(8, poolSize);
        public float ParticleDuration => Mathf.Max(0.05f, particleDuration);
        public float DecalDuration => Mathf.Max(0.05f, decalDuration);
        public float SurfaceOffset => Mathf.Max(0f, surfaceOffset);
        public float DecalSize => Mathf.Max(0.01f, decalSize);
        public float MinDecalScale => Mathf.Max(0.01f, minDecalScale);
        public HitImpactSurfaceSettings[] Surfaces => surfaces;

        public HitImpactSurfaceSettings GetSurface(SurfaceType type)
        {
            for (int i = 0; i < surfaces.Length; i++)
            {
                if (surfaces[i].Surface == type)
                    return surfaces[i];
            }

            for (int i = 0; i < surfaces.Length; i++)
            {
                if (surfaces[i].Surface == SurfaceType.Default)
                    return surfaces[i];
            }

            return surfaces[0];
        }
    }

    [Serializable]
    public struct HitImpactSurfaceSettings
    {
        [SerializeField] private SurfaceType surface;
        [SerializeField] private Color particleColor;
        [SerializeField] private Color decalColor;
        [SerializeField] private float particleSize;
        [SerializeField] private bool spawnDecal;

        public HitImpactSurfaceSettings(
            SurfaceType surface,
            Color particleColor,
            Color decalColor,
            float particleSize,
            bool spawnDecal)
        {
            this.surface = surface;
            this.particleColor = particleColor;
            this.decalColor = decalColor;
            this.particleSize = particleSize;
            this.spawnDecal = spawnDecal;
        }

        public SurfaceType Surface => surface;
        public Color ParticleColor => particleColor;
        public Color DecalColor => decalColor;
        public float ParticleSize => particleSize;
        public bool SpawnDecal => spawnDecal;
    }
}
