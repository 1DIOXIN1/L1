using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Weapon
{
    public sealed class PlayerWeaponView
    {
        private readonly Transform _socket;
        private GameObject _currentView;

        public PlayerWeaponView(Transform socket)
        {
            _socket = socket;
        }

        public void Show(IWeapon weapon)
        {
            Clear();

            if (weapon?.ViewPrefab == null || _socket == null)
                return;

            _currentView = Object.Instantiate(weapon.ViewPrefab, _socket);
            Transform viewTransform = _currentView.transform;
            viewTransform.localPosition = weapon.ViewLocalPosition;
            viewTransform.localRotation = Quaternion.Euler(weapon.ViewLocalEulerAngles);
            viewTransform.localScale = weapon.ViewLocalScale;
        }

        public void Clear()
        {
            if (_currentView == null)
                return;

            Object.Destroy(_currentView);
            _currentView = null;
        }

        public static Transform CreateSocket(Transform firePoint)
        {
            if (firePoint == null)
                return null;

            Transform parent = firePoint.parent != null ? firePoint.parent : firePoint;
            Transform existing = parent.Find("WeaponSocket");
            if (existing != null)
                return existing;

            GameObject socketObject = new GameObject("WeaponSocket");
            Transform socket = socketObject.transform;
            socket.SetParent(parent, false);
            socket.localPosition = firePoint.localPosition;
            socket.localRotation = firePoint.localRotation;
            socket.localScale = Vector3.one;
            return socket;
        }
    }
}
