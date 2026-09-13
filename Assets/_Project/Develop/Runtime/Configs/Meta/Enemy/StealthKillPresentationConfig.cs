using _Project.Develop.Runtime.Gameplay.Features.Main.Stealth;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace _Project.Develop.Runtime.Configs.Meta.Enemy
{
    [CreateAssetMenu(
        menuName = "Configs/Meta/Enemy/StealthKillPresentation",
        fileName = "StealthKillPresentation")]
    public sealed class StealthKillPresentationConfig : ScriptableObject
    {
        [SerializeField] private TimelineAsset timeline;
        [SerializeField] private StealthKillCameraRig cameraRigPrefab;

        public PlayableAsset Timeline => timeline;
        public StealthKillCameraRig CameraRigPrefab => cameraRigPrefab;

        public bool HasTimeline => timeline != null;
    }
}
