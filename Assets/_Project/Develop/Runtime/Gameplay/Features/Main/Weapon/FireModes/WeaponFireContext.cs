using _Project.Develop.Runtime.Configs.Meta.Weapon;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.FireModes
{
    public readonly struct WeaponFireContext
    {
        public WeaponFireContext(
            Transform firePoint,
            GameObject owner,
            WeaponConfig config,
            GameObject bulletPrefab = null,
            Camera aimCamera = null)
        {
            FirePoint = firePoint;
            Owner = owner;
            Config = config;
            BulletPrefab = bulletPrefab;
            AimCamera = aimCamera;
        }

        public Transform FirePoint { get; }
        public GameObject Owner { get; }
        public WeaponConfig Config { get; }
        public GameObject BulletPrefab { get; }
        public Camera AimCamera { get; }
    }
}
