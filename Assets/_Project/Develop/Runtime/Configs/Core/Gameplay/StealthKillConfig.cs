using UnityEngine;

namespace _Project.Develop.Runtime.Configs.Core.Gameplay
{
    [CreateAssetMenu(
        menuName = "Configs/Core/Gameplay/StealthKillConfig",
        fileName = "StealthKillConfig")]
    public sealed class StealthKillConfig : ScriptableObject
    {
        [SerializeField, Range(10f, 90f)] private float behindAngle = 50f;
        [SerializeField] private float maxDistance = 2.2f;
        [SerializeField] private float killDuration = 1.5f;
        [SerializeField] private float playerStandDistance = 0.85f;
        [SerializeField] private float cameraReturnBlendDuration = 0.75f;

        public float BehindAngle => behindAngle;
        public float MaxDistance => maxDistance;
        public float MaxDistanceSqr => maxDistance * maxDistance;
        public float KillDuration => killDuration;
        public float PlayerStandDistance => playerStandDistance;
        public float CameraReturnBlendDuration => cameraReturnBlendDuration;
    }
}
