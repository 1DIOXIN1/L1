using UnityEngine;

namespace _Project.Develop.Runtime.Configs.Meta.Noise
{
    [CreateAssetMenu(menuName = "Configs/Core/Gameplay/Noise/NoiseConfig", fileName = "NoiseConfig")]
    public class NoiseConfig : ScriptableObject
    {
        [field: Header("Walk")]
        [field: SerializeField] public float WalkHearingRadius { get; private set; } = 8f;
        [field: SerializeField, Range(0f, 1f)] public float WalkSuspicion { get; private set; } = 0.35f;
        [field: SerializeField] public float WalkEmitInterval { get; private set; } = 0.45f;

        [field: Header("Run")]
        [field: SerializeField] public float RunHearingRadius { get; private set; } = 16f;
        [field: SerializeField, Range(0f, 1f)] public float RunSuspicion { get; private set; } = 0.7f;
        [field: SerializeField] public float RunEmitInterval { get; private set; } = 0.25f;

        [field: Header("Crouch walk (0 radius = silent)")]
        [field: SerializeField] public float CrouchHearingRadius { get; private set; } = 0f;
        [field: SerializeField, Range(0f, 1f)] public float CrouchSuspicion { get; private set; } = 0.15f;
        [field: SerializeField] public float CrouchEmitInterval { get; private set; } = 0.6f;
    }
}
