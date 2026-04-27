using _Project.Develop.Runtime.Configs.Meta.Weapon.WeaponsConfigs;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.WeaponFireType;
using _Project.Develop.Runtime.Utilities.AssetsManagement;
using _Project.Develop.Runtime.Utilities.ConfigsManagement;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.WeaponsType
{
    public class Smg : IWeapon
    {
        private GameObject _owner;
        private Transform _firePoint;
        private readonly SmgConfig _config;
        private GameObject _bulletPrefab;
        
        public Smg(
            SmgConfig config,
            Transform firePoint,
            GameObject owner,
            GameObject bulletPrefab)
        {
            _owner = owner;
            _firePoint = firePoint;
            _config = config;
            _bulletPrefab = bulletPrefab;
            
            Ammo = config.Ammo;
            ReloadSpeed = config.ReloadSpeed;
            ShootSpeed = config.ShootSpeed;
            Damage = config.Damage;
            BulletLifeTime = config.BulletLifeTime;
        }
        
        public int Ammo { get; private set; }
        public float ShootSpeed { get; private set; }
        public float ReloadSpeed { get; private set; }
        public int Damage { get; private set; }
        private float BulletLifeTime { get; set; }
        
        public void Shoot()
        {
            var projectileObject = Object.Instantiate(_bulletPrefab, _firePoint.position, Quaternion.identity);

            var projectile = projectileObject.GetComponent<ProjectileShoot>();

            projectile.Initialize(_firePoint.forward, ShootSpeed, Damage, BulletLifeTime, _owner);
        }

        public void Reload()
        {
            
        }

    }
}