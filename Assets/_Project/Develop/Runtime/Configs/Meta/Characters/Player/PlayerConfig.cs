using UnityEngine;

namespace _Project.Develop.Runtime.Configs.Meta.Characters.Player
{
    [CreateAssetMenu(menuName = "Configs/Core/Gameplay/Player", fileName = "PlayerConfig")]
    public class PlayerConfig : ScriptableObject, ICharacterConfig
    {
        [field: Header("Vitality")]
        [field: SerializeField] public int Health { get; private set; } = 100;

        [field: Header("Movement")]
        [field: SerializeField] public float Speed { get; private set; } = 4f;
        [field: SerializeField] public float SprintSpeedMultiplier { get; private set; } = 1.5f;
        [field: SerializeField] public float CrouchSpeedMultiplier { get; private set; } = 0.5f;
        [field: SerializeField] public float JumpHeight { get; private set; } = 1.5f;
        [field: SerializeField] public float Gravity { get; private set; } = -20f;
        [field: SerializeField] public float RotationSpeed { get; private set; } = 12f;
        [field: SerializeField] public float GroundSnapDistance { get; private set; } = 5f;
        [field: SerializeField] public float StandingHeight { get; private set; } = 2f;
        [field: SerializeField] public float CrouchHeight { get; private set; } = 1f;

        [field: Header("Stamina")]
        [field: SerializeField] public float MaxStamina { get; private set; } = 100f;
        [field: SerializeField] public float SprintStaminaSpendPerSecond { get; private set; } = 25f;
        [field: SerializeField] public float StaminaRecoveryPerSecond { get; private set; } = 18f;
        [field: SerializeField] public float StaminaRecoveryDelay { get; private set; } = 1f;

        [field: Header("Look / Camera")]
        [field: SerializeField] public float LookSensitivity { get; private set; } = 2f;
        [field: SerializeField] public float MinPitch { get; private set; } = -70f;
        [field: SerializeField] public float MaxPitch { get; private set; } = 70f;
        [field: SerializeField] public Vector3 CameraDefaultOffset { get; private set; } = new(0.08f, -0.02f, 0.12f);
        [field: SerializeField] public Vector3 CameraAimOffset { get; private set; } = new(0.05f, -0.02f, 0.2f);
        [field: SerializeField] public float CameraDefaultFov { get; private set; } = 60f;
        [field: SerializeField] public float CameraAimFov { get; private set; } = 45f;
        [field: SerializeField] public float CameraOffsetSmoothTime { get; private set; } = 0.12f;
        [field: SerializeField] public float CameraFovLerpSpeed { get; private set; } = 12f;
        [field: SerializeField] public float HipBodyTurnSpeed { get; private set; } = 270f;
        [field: SerializeField] public float AimBodyTurnSpeed { get; private set; } = 720f;
        [field: SerializeField] public float MaxFreeLookYaw { get; private set; } = 80f;

        [field: Header("Camera Fire Shake")]
        [field: SerializeField] public float CameraFireShakePosition { get; private set; } = 0.012f;
        [field: SerializeField] public float CameraFireShakeRotation { get; private set; } = 0.55f;
        [field: SerializeField] public float CameraFireShakeReturnSpeed { get; private set; } = 16f;
        [field: SerializeField] public float CameraAimFireShakeMultiplier { get; private set; } = 0.45f;

        [field: Header("Weapon Follow")]
        [field: SerializeField] public float WeaponPositionFollowSpeed { get; private set; } = 20f;
        [field: SerializeField] public float WeaponRotationFollowSpeed { get; private set; } = 5f;
        [field: SerializeField] public float WeaponAimPositionFollowSpeed { get; private set; } = 30f;
        [field: SerializeField] public float WeaponAimRotationFollowSpeed { get; private set; } = 12f;

        [field: Header("Weapon Sprint")]
        [field: SerializeField] public float WeaponSprintDip { get; private set; } = 0.045f;
        [field: SerializeField] public float WeaponSprintDipPitch { get; private set; } = 4f;
        [field: SerializeField] public float WeaponSprintDipSpeed { get; private set; } = 8f;

        [field: Header("Weapon Bob")]
        [field: SerializeField] public float WeaponBobAmplitude { get; private set; } = 0.012f;
        [field: SerializeField] public float WeaponBobFrequency { get; private set; } = 9f;
        [field: SerializeField] public float WeaponBobRollDegrees { get; private set; } = 0.85f;
        [field: SerializeField] public float WeaponSprintBobMultiplier { get; private set; } = 1.45f;
        [field: SerializeField] public float WeaponSprintBobFrequencyMultiplier { get; private set; } = 1.35f;
        [field: SerializeField] public float WeaponAimBobMultiplier { get; private set; } = 0.2f;

        [field: Header("Weapon Recoil")]
        [field: SerializeField] public float WeaponRecoilKickBack { get; private set; } = 0.022f;
        [field: SerializeField] public float WeaponRecoilKickUp { get; private set; } = 0.008f;
        [field: SerializeField] public float WeaponRecoilPitch { get; private set; } = 2.4f;
        [field: SerializeField] public float WeaponRecoilYaw { get; private set; } = 0.55f;
        [field: SerializeField] public float WeaponRecoilReturnSpeed { get; private set; } = 14f;
        [field: SerializeField] public float WeaponAimRecoilMultiplier { get; private set; } = 0.4f;

        [field: Header("Weapon Strafe")]
        [field: SerializeField] public float WeaponStrafeRollDegrees { get; private set; } = 2.2f;
        [field: SerializeField] public float WeaponStrafeYawDegrees { get; private set; } = 1.2f;
        [field: SerializeField] public float WeaponStrafeSmoothSpeed { get; private set; } = 8f;

        [field: Header("Weapon Landing")]
        [field: SerializeField] public float WeaponLandingDip { get; private set; } = 0.04f;
        [field: SerializeField] public float WeaponLandingPitch { get; private set; } = 3.5f;
        [field: SerializeField] public float WeaponLandingReturnSpeed { get; private set; } = 7f;

        [field: Header("Weapon Breath")]
        [field: SerializeField] public float WeaponBreathAmplitude { get; private set; } = 0.0035f;
        [field: SerializeField] public float WeaponBreathFrequency { get; private set; } = 1.15f;
        [field: SerializeField] public float WeaponAimBreathMultiplier { get; private set; } = 0.55f;

        [field: Header("Muzzle VFX")]
        [field: SerializeField] public float MuzzleFlashLightIntensity { get; private set; } = 3.5f;
        [field: SerializeField] public float MuzzleFlashLightDuration { get; private set; } = 0.07f;
    }
}
