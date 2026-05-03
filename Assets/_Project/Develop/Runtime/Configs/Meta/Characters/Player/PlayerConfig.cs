using UnityEngine;

namespace _Project.Develop.Runtime.Configs.Meta.Characters.Player
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Player", fileName = "PlayerConfig")]
    public class PlayerConfig : ScriptableObject, ICharacterConfig
    {
        [field: SerializeField] public int Health { get; private set; } = 100;
        [field: SerializeField] public float Speed { get; private set; } = 4f;
        [field: SerializeField] public float SprintSpeedMultiplier { get; private set; } = 1.5f;
        [field: SerializeField] public float CrouchSpeedMultiplier { get; private set; } = 0.5f;
        [field: SerializeField] public float JumpHeight { get; private set; } = 1.5f;
        [field: SerializeField] public float Gravity { get; private set; } = -20f;
        [field: SerializeField] public float MaxStamina { get; private set; } = 100f;
        [field: SerializeField] public float SprintStaminaSpendPerSecond { get; private set; } = 25f;
        [field: SerializeField] public float StaminaRecoveryPerSecond { get; private set; } = 18f;
        [field: SerializeField] public float StaminaRecoveryDelay { get; private set; } = 1f;
        [field: SerializeField] public float StandingHeight { get; private set; } = 2f;
        [field: SerializeField] public float CrouchHeight { get; private set; } = 1f;
        [field: SerializeField] public float RotationSpeed { get; private set; } = 12f;
        [field: SerializeField] public float GroundSnapDistance { get; private set; } = 5f;
    }
}
