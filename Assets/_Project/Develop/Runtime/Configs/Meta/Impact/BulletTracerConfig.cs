using UnityEngine;

namespace _Project.Develop.Runtime.Configs.Meta.Impact
{
    [CreateAssetMenu(
        menuName = "Configs/Core/Gameplay/Impact/BulletTracer",
        fileName = "BulletTracerConfig")]
    public sealed class BulletTracerConfig : ScriptableObject
    {
        [Header("Pool")]
        [SerializeField] private int poolSize = 24;

        [Header("Motion")]
        [SerializeField] private float duration = 0.18f;
        [SerializeField] private float trailLength = 2.8f;
        [SerializeField] private float holdDuration = 0.04f;
        [SerializeField] private float seedLength = 0.35f;

        [Header("Look")]
        [SerializeField] private float width = 0.035f;
        [SerializeField] private float endWidthRatio = 0.45f;
        [SerializeField] private float viewSideOffset = 0f;
        [SerializeField] private Color color = new Color(1f, 0.95f, 0.55f, 1f);

        public int PoolSize => Mathf.Max(4, poolSize);
        public float Duration => Mathf.Max(0.05f, duration);
        public float TrailLength => Mathf.Max(0.5f, trailLength);
        public float HoldDuration => Mathf.Max(0f, holdDuration);
        public float SeedLength => Mathf.Max(0.05f, seedLength);
        public float Width => Mathf.Max(0.01f, width);
        public float EndWidthRatio => Mathf.Clamp01(endWidthRatio);
        public float ViewSideOffset => Mathf.Max(0f, viewSideOffset);
        public Color Color => color;
    }
}
