using System;
using _Project.Develop.Runtime.Configs.Meta.Characters.Player;
using _Project.Develop.Runtime.Utilities.AssetsManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Weapon
{
    public sealed class PlayerWeaponView
    {
        private const string MuzzleVfxPrefabPath = "Prefabs/Weapons/Muzzle/WeaponMuzzleVfx";

        private readonly Transform _socket;
        private readonly PlayerWeaponVisualFollow _visualFollow;
        private readonly Action _onFired;
        private readonly WeaponMuzzleVfx _muzzleVfx;
        private GameObject _currentView;
        private IWeapon _currentWeapon;

        public PlayerWeaponView(
            Transform socket,
            PlayerWeaponVisualFollow visualFollow,
            PlayerConfig config,
            ResourcesAssetsLoader assetsLoader,
            Action onFired = null)
        {
            _socket = socket;
            _visualFollow = visualFollow;
            _onFired = onFired;

            GameObject muzzleInstance = Object.Instantiate(
                assetsLoader.Load<GameObject>(MuzzleVfxPrefabPath),
                _socket,
                false);
            muzzleInstance.name = "WeaponMuzzleVfx";
            _muzzleVfx = muzzleInstance.GetComponent<WeaponMuzzleVfx>();
            _muzzleVfx.FlashPulse.Configure(config.MuzzleFlashLightIntensity, config.MuzzleFlashLightDuration);
        }

        public void Show(IWeapon weapon)
        {
            UnsubscribeWeapon();
            ClearView();

            _currentWeapon = weapon;
            if (_currentWeapon != null)
                _currentWeapon.Fired += OnWeaponFired;

            if (weapon?.ViewPrefab == null)
                return;

            _currentView = Object.Instantiate(weapon.ViewPrefab, _socket, false);
            Transform viewTransform = _currentView.transform;
            viewTransform.localPosition = weapon.ViewLocalPosition;
            viewTransform.localRotation *= Quaternion.Euler(weapon.ViewLocalEulerAngles);
            viewTransform.localScale = weapon.ViewLocalScale;
        }

        public void Clear()
        {
            UnsubscribeWeapon();
            ClearView();
        }

        public static Transform CreateSocket(Transform firePoint, out Transform pivot)
        {
            Transform mountParent = firePoint.parent != null && firePoint.parent.parent != null
                ? firePoint.parent.parent
                : firePoint.parent != null ? firePoint.parent : firePoint;

            pivot = mountParent.Find("WeaponVisualPivot");
            if (pivot == null)
            {
                GameObject pivotObject = new GameObject("WeaponVisualPivot");
                pivot = pivotObject.transform;
                pivot.SetParent(mountParent, false);
                pivot.position = firePoint.position;
                pivot.rotation = firePoint.rotation;
                pivot.localScale = Vector3.one;
            }

            Transform socket = pivot.Find("WeaponSocket");
            if (socket == null)
            {
                GameObject socketObject = new GameObject("WeaponSocket");
                socket = socketObject.transform;
                socket.SetParent(pivot, false);
                socket.localPosition = Vector3.zero;
                socket.localRotation = Quaternion.identity;
                socket.localScale = Vector3.one;
            }

            return socket;
        }

        private void OnWeaponFired()
        {
            _visualFollow.AddRecoil();
            _onFired?.Invoke();
            PlayParticle(_muzzleVfx.MuzzleFlash);
            PlayParticle(_muzzleVfx.MuzzleSmoke);
            PlayParticle(_muzzleVfx.CasingEject);
            _muzzleVfx.FlashPulse.Pulse();
        }

        private void UnsubscribeWeapon()
        {
            if (_currentWeapon == null)
                return;

            _currentWeapon.Fired -= OnWeaponFired;
            _currentWeapon = null;
        }

        private void ClearView()
        {
            if (_currentView == null)
                return;

            Object.Destroy(_currentView);
            _currentView = null;
        }

        private static void PlayParticle(ParticleSystem particles)
        {
            particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            particles.Play(true);
        }
    }
}
