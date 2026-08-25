using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.WeaponsType;
using UnityEngine;

namespace _Project.Develop.Runtime.Configs.Meta.Weapon
{
    [CreateAssetMenu(menuName = "Configs/Core/Gameplay/Weapon/WeaponConfig", fileName = "WeaponConfig")]
    public class WeaponConfig : ScriptableObject
    {
        [field: SerializeField] public WeaponType Type { get; private set; }
        [field: SerializeField] public WeaponFireMode FireMode { get; private set; } = WeaponFireMode.Projectile;
        [field: SerializeField] public int MagazineSize { get; private set; } = 30;
        [field: SerializeField] public float ReloadDuration { get; private set; } = 3f;
        [field: SerializeField] public int Damage { get; private set; } = 10;
        [field: SerializeField] public float FireInterval { get; private set; } = 0.1f;
        [field: SerializeField] public float ProjectileSpeed { get; private set; } = 30f;
        [field: SerializeField] public float BulletLifeTime { get; private set; } = 0.8f;
        [field: SerializeField] public float Range { get; private set; } = 50f;
        [field: SerializeField] public bool IsAutomatic { get; private set; }
    }
}
