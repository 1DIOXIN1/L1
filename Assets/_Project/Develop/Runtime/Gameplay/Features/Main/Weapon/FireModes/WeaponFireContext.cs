using _Project.Develop.Runtime.Configs.Meta.Weapon;
using _Project.Develop.Runtime.Gameplay.Features.Main.Impact;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.FireModes
{
    public readonly struct WeaponFireContext
    {
        public WeaponFireContext(
            Transform firePoint,
            GameObject owner,
            WeaponConfig config,
            GameObject bulletPrefab,
            Camera aimCamera,
            HitImpactService hitImpacts,
            bool isAiming,
            BulletTracerService tracers,
            float currentSpreadDegrees)
        {
            FirePoint = firePoint;
            Owner = owner;
            Config = config;
            BulletPrefab = bulletPrefab;
            AimCamera = aimCamera;
            HitImpacts = hitImpacts;
            IsAiming = isAiming;
            Tracers = tracers;
            CurrentSpreadDegrees = currentSpreadDegrees;
        }

        public Transform FirePoint { get; }
        public GameObject Owner { get; }
        public WeaponConfig Config { get; }
        public GameObject BulletPrefab { get; }
        public Camera AimCamera { get; }
        public HitImpactService HitImpacts { get; }
        public bool IsAiming { get; }
        public BulletTracerService Tracers { get; }
        public float CurrentSpreadDegrees { get; }
    }
}
