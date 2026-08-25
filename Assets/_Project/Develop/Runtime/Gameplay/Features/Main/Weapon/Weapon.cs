using _Project.Develop.Runtime.Configs.Meta.Weapon;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.FireModes;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Weapon
{
    public sealed class Weapon : IWeapon
    {
        private readonly WeaponConfig _config;
        private readonly IFireMode _fireMode;
        private readonly WeaponFireContext _fireContext;

        private int _ammo;
        private float _nextShotTime;
        private float _reloadEndTime;

        public Weapon(
            WeaponConfig config,
            IFireMode fireMode,
            Transform firePoint,
            GameObject owner,
            GameObject bulletPrefab = null)
        {
            _config = config;
            _fireMode = fireMode;
            _ammo = config.MagazineSize;
            _fireContext = new WeaponFireContext(firePoint, owner, config, bulletPrefab);
        }

        public int Ammo => _ammo;
        public int MagazineSize => _config.MagazineSize;
        public bool CanShoot => IsReloading == false && _ammo > 0 && Time.time >= _nextShotTime;
        public bool IsReloading => Time.time < _reloadEndTime;
        public bool IsAutomatic => _config.IsAutomatic;

        public void Tick(float deltaTime)
        {
            if (_reloadEndTime <= 0f || IsReloading)
                return;

            _ammo = MagazineSize;
            _reloadEndTime = 0f;
        }

        public void Shoot()
        {
            if (CanShoot == false)
                return;

            _fireMode.Fire(_fireContext);
            _ammo--;
            _nextShotTime = Time.time + _config.FireInterval;
        }

        public void Reload()
        {
            if (IsReloading || _ammo == MagazineSize)
                return;

            _reloadEndTime = Time.time + _config.ReloadDuration;
        }
    }
}
