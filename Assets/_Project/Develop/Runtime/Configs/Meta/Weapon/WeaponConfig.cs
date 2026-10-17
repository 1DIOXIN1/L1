using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.WeaponsType;
using UnityEngine;

namespace _Project.Develop.Runtime.Configs.Meta.Weapon
{
    [CreateAssetMenu(menuName = "Configs/Core/Gameplay/Weapon/WeaponConfig", fileName = "WeaponConfig")]
    public class WeaponConfig : ScriptableObject
    {
        [field: SerializeField] public WeaponType Type { get; private set; }
        [field: SerializeField] public WeaponFireMode FireMode { get; private set; } = WeaponFireMode.Hitscan;
        [field: SerializeField] public int MagazineSize { get; private set; } = 30;
        [field: SerializeField] public int ReserveAmmo { get; private set; } = 270;
        [field: SerializeField] public float ReloadDuration { get; private set; } = 3f;
        [field: SerializeField] public int Damage { get; private set; } = 10;
        [field: SerializeField] public float FireInterval { get; private set; } = 0.1f;
        [field: SerializeField] public float ProjectileSpeed { get; private set; } = 30f;
        [field: SerializeField] public float BulletLifeTime { get; private set; } = 0.8f;
        [field: SerializeField] public float Range { get; private set; } = 50f;
        [field: Header("Spread (bloom)")]
        [field: SerializeField] public float HipSpreadDegrees { get; private set; } = 0.4f;
        [field: SerializeField] public float MaxHipSpreadDegrees { get; private set; } = 4.5f;
        [field: SerializeField] public float AimSpreadDegrees { get; private set; } = 0.15f;
        [field: SerializeField] public float MaxAimSpreadDegrees { get; private set; } = 1.6f;
        [field: SerializeField] public float SpreadPerShotDegrees { get; private set; } = 0.35f;
        [field: SerializeField] public float SpreadRecoveryPerSecond { get; private set; } = 6f;
        [field: SerializeField] public float SpreadRecoveryDelay { get; private set; } = 0.12f;
        [SerializeField, Range(0f, 1.5f)] private float spreadClimbBias = 1f;
        [SerializeField, Range(0f, 1f)] private float spreadSideRatio = 0.35f;
        public float SpreadClimbBias => spreadClimbBias;
        public float SpreadSideRatio => spreadSideRatio;
        [field: SerializeField] public bool IsAutomatic { get; private set; }
        [field: SerializeField] public Sprite HudIconActive { get; private set; }
        [field: SerializeField] public GameObject ViewPrefab { get; private set; }
        [field: SerializeField] public Vector3 ViewLocalPosition { get; private set; }
        [field: SerializeField] public Vector3 ViewLocalEulerAngles { get; private set; }
        [field: SerializeField] public Vector3 ViewLocalScale { get; private set; } = Vector3.one;
    }
}
