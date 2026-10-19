using System;
using _Project.Develop.Runtime.Configs.Meta.Weapon;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters;
using _Project.Develop.Runtime.Gameplay.Features.Main.Impact;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.FireModes;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.WeaponsType;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Weapon
{
    public sealed class Weapon : IWeapon
    {
        private readonly WeaponConfig _config;
        private readonly IFireMode _fireMode;
        private readonly Transform _firePoint;
        private readonly IDamageble _attacker;
        private readonly GameObject _bulletPrefab;
        private readonly HitImpactService _hitImpacts;
        private readonly BulletTracerService _tracers;

        private Camera _aimCamera;
        private Func<bool> _isAimingProvider;
        private int _ammo;
        private int _reserveAmmo;
        private float _nextShotTime;
        private float _reloadEndTime;
        private float _currentSpreadDegrees;
        private float _lastShotTime = -999f;

        public event Action AmmoChanged;
        public event Action Fired;

        public Weapon(
            WeaponConfig config,
            IFireMode fireMode,
            Transform firePoint,
            IDamageble attacker,
            GameObject bulletPrefab = null,
            int? initialAmmo = null,
            int? initialReserveAmmo = null,
            Camera aimCamera = null,
            HitImpactService hitImpacts = null,
            BulletTracerService tracers = null)
        {
            _config = config;
            _fireMode = fireMode;
            _firePoint = firePoint;
            _attacker = attacker;
            _bulletPrefab = bulletPrefab;
            _aimCamera = aimCamera;
            _hitImpacts = hitImpacts;
            _tracers = tracers;
            _ammo = Mathf.Clamp(initialAmmo ?? config.MagazineSize, 0, config.MagazineSize);
            _reserveAmmo = Mathf.Max(0, initialReserveAmmo ?? config.ReserveAmmo);
            _currentSpreadDegrees = GetMinSpread(false);
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
        public GameObject ViewPrefab => _config.ViewPrefab;
        public Vector3 ViewLocalPosition => _config.ViewLocalPosition;
        public Vector3 ViewLocalEulerAngles => _config.ViewLocalEulerAngles;
        public Vector3 ViewLocalScale => _config.ViewLocalScale == Vector3.zero
            ? Vector3.one
            : _config.ViewLocalScale;

        public void SetAimCamera(Camera aimCamera)
        {
            _aimCamera = aimCamera;
        }

        public void SetAimingProvider(Func<bool> isAimingProvider)
        {
            _isAimingProvider = isAimingProvider;
        }

        public void Tick(float deltaTime)
        {
            UpdateSpreadRecovery(deltaTime);

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

            bool isAiming = _isAimingProvider != null && _isAimingProvider();
            float spread = GetSpreadForShot(isAiming);

            _fireMode.Fire(new WeaponFireContext(
                _firePoint,
                _attacker,
                _config,
                _bulletPrefab,
                _aimCamera,
                _hitImpacts,
                isAiming,
                _tracers,
                spread));

            ApplySpreadKick(isAiming);
            _lastShotTime = Time.time;
            _ammo--;
            _nextShotTime = Time.time + _config.FireInterval;
            AmmoChanged?.Invoke();
            Fired?.Invoke();
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

        private float GetSpreadForShot(bool isAiming)
        {
            float min = GetMinSpread(isAiming);
            float max = GetMaxSpread(isAiming);
            _currentSpreadDegrees = Mathf.Clamp(_currentSpreadDegrees, min, max);
            return _currentSpreadDegrees;
        }

        private void ApplySpreadKick(bool isAiming)
        {
            float max = GetMaxSpread(isAiming);
            _currentSpreadDegrees = Mathf.Min(
                max,
                _currentSpreadDegrees + Mathf.Max(0f, _config.SpreadPerShotDegrees));
        }

        private void UpdateSpreadRecovery(float deltaTime)
        {
            if (Time.time - _lastShotTime < _config.SpreadRecoveryDelay)
                return;

            bool isAiming = _isAimingProvider != null && _isAimingProvider();
            float min = GetMinSpread(isAiming);
            float recovery = Mathf.Max(0f, _config.SpreadRecoveryPerSecond) * deltaTime;
            _currentSpreadDegrees = Mathf.MoveTowards(_currentSpreadDegrees, min, recovery);
        }

        private float GetMinSpread(bool isAiming)
        {
            return Mathf.Max(0f, isAiming ? _config.AimSpreadDegrees : _config.HipSpreadDegrees);
        }

        private float GetMaxSpread(bool isAiming)
        {
            float min = GetMinSpread(isAiming);
            float max = isAiming ? _config.MaxAimSpreadDegrees : _config.MaxHipSpreadDegrees;
            return Mathf.Max(min, max);
        }
    }
}
