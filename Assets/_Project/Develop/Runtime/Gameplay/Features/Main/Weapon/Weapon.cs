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
        private int _reserveAmmo;
        private float _nextShotTime;
        private float _reloadEndTime;

        public event Action AmmoChanged;

        public Weapon(
            WeaponConfig config,
            IFireMode fireMode,
            Transform firePoint,
            GameObject owner,
            GameObject bulletPrefab = null,
            int? initialAmmo = null,
            int? initialReserveAmmo = null)
        {
            _config = config;
            _fireMode = fireMode;
            _ammo = Mathf.Clamp(initialAmmo ?? config.MagazineSize, 0, config.MagazineSize);
            _reserveAmmo = Mathf.Max(0, initialReserveAmmo ?? config.ReserveAmmo);
            _fireContext = new WeaponFireContext(firePoint, owner, config, bulletPrefab);
        }

        public WeaponType Type => _config.Type;
        public int Ammo => _ammo;
        public int ReserveAmmo => _reserveAmmo;
        public int MagazineSize => _config.MagazineSize;
        public int ReserveCapacity => _config.ReserveAmmo;
        public bool CanShoot => IsReloading == false && _ammo > 0 && Time.time >= _nextShotTime;
        public bool IsReloading => _reloadEndTime > 0f && Time.time < _reloadEndTime;
        public bool IsAutomatic => _config.IsAutomatic;
        public Sprite HudIconActive => _config.HudIconActive;
        public Sprite HudIconReloading => _config.HudIconReloading;
        public GameObject ViewPrefab => _config.ViewPrefab;
        public Vector3 ViewLocalPosition => _config.ViewLocalPosition;
        public Vector3 ViewLocalEulerAngles => _config.ViewLocalEulerAngles;
        public Vector3 ViewLocalScale => _config.ViewLocalScale == Vector3.zero
            ? Vector3.one
            : _config.ViewLocalScale;

        public void Tick(float deltaTime)
        {
            if (_reloadEndTime <= 0f || IsReloading)
                return;

            int needed = MagazineSize - _ammo;
            int taken = Mathf.Min(needed, _reserveAmmo);
            _ammo += taken;
            _reserveAmmo -= taken;
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
            if (IsReloading || _ammo >= MagazineSize || _reserveAmmo <= 0)
                return;

            _reloadEndTime = Time.time + _config.ReloadDuration;
            AmmoChanged?.Invoke();
        }

        public void SetAmmo(int ammo)
        {
            _ammo = Mathf.Clamp(ammo, 0, MagazineSize);
            AmmoChanged?.Invoke();
        }

        public void SetReserveAmmo(int reserveAmmo)
        {
            _reserveAmmo = Mathf.Max(0, reserveAmmo);
            AmmoChanged?.Invoke();
        }
    }
}
