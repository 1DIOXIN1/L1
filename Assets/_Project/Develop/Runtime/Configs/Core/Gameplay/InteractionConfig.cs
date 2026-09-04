using UnityEngine;

namespace _Project.Develop.Runtime.Configs.Core.Gameplay
{
    [CreateAssetMenu(
        menuName = "Configs/Core/Gameplay/InteractionConfig",
        fileName = "InteractionConfig")]
    public sealed class InteractionConfig : ScriptableObject
    {
        [SerializeField] private float maxDistance = 5f;
        [SerializeField, Range(0.05f, 0.5f)] private float maxScreenRadius = 0.22f;
        [SerializeField] private bool checkOcclusion = true;
        [SerializeField] private LayerMask occlusionMask = ~0;

        public float MaxDistance => maxDistance;
        public float MaxDistanceSqr => maxDistance * maxDistance;
        public float MaxScreenRadius => maxScreenRadius;
        public float MaxScreenRadiusSqr => maxScreenRadius * maxScreenRadius;
        public bool CheckOcclusion => checkOcclusion;
        public LayerMask OcclusionMask => occlusionMask;
    }
}
