using System;
using _Project.Develop.Runtime.Configs.Meta.Weapon;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.FireModes;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.WeaponsType;
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

        public event Action AmmoChanged;

        public Weapon(
            WeaponConfig config,
            IFireMode fireMode,
            Transform firePoint,
            GameObject owner,
            GameObject bulletPrefab = null,
            int? initialAmmo = null)
        {
            _config = config;
            _fireMode = fireMode;
            _ammo = Mathf.Clamp(initialAmmo ?? config.MagazineSize, 0, config.MagazineSize);
            _fireContext = new WeaponFireContext(firePoint, owner, config, bulletPrefab);
        }

        public WeaponType Type => _config.Type;
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
            AmmoChanged?.Invoke();
        }

        public void Shoot()
        {
            if (CanShoot == false)
                return;

            _fireMode.Fire(_fireContext);
            _ammo--;
            _nextShotTime = Time.time + _config.FireInterval;
            AmmoChanged?.Invoke();
        }

        public void Reload()
        {
            if (IsReloading || _ammo == MagazineSize)
                return;

            _reloadEndTime = Time.time + _config.ReloadDuration;
        }

        public void SetAmmo(int ammo)
        {
            _ammo = Mathf.Clamp(ammo, 0, MagazineSize);
            AmmoChanged?.Invoke();
        }
    }
}
