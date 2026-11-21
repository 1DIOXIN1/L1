using System;
using System.Collections.Generic;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.WeaponsType;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Weapon
{
    public class WeaponInventory
    {
        private readonly List<IWeapon> _weapons = new();
        private readonly IWeapon _stunGun;
        private IWeapon _selectedWeapon;
        private bool _stunGunDrawn;

        public WeaponInventory(IWeapon stunGun) => _stunGun = stunGun;

        public event Action<IWeapon> WeaponChanged;
        public event Action Changed;

        public IWeapon CurrentWeapon => _stunGunDrawn ? _stunGun : _selectedWeapon;
        public WeaponType? SelectedWeaponType => _selectedWeapon?.Type;
        public IReadOnlyList<IWeapon> Weapons => _weapons;

        public void ToggleStunGun()
        {
            _stunGunDrawn = !_stunGunDrawn;
            WeaponChanged?.Invoke(CurrentWeapon);
        }

        public bool Add(IWeapon weapon)
        {
            if (weapon == null || weapon.Type == WeaponType.StunGun)
                return false;

            if (Contains(weapon.Type))
                return false;

            _weapons.Add(weapon);
            Changed?.Invoke();

            if (_selectedWeapon == null)
            {
                _selectedWeapon = weapon;
                if (!_stunGunDrawn)
                    WeaponChanged?.Invoke(weapon);
            }

            return true;
        }

        public bool Remove(WeaponType type)
        {
            int index = IndexOf(type);
            if (index < 0)
                return false;

            IWeapon removed = _weapons[index];
            _weapons.RemoveAt(index);
            Changed?.Invoke();

            if (_selectedWeapon == removed)
            {
                _selectedWeapon = _weapons.Count > 0 ? _weapons[0] : null;
                if (!_stunGunDrawn)
                    WeaponChanged?.Invoke(_selectedWeapon);
            }

            return true;
        }

        public bool Equip(WeaponType type)
        {
            int index = IndexOf(type);
            if (index < 0)
                return false;

            IWeapon weapon = _weapons[index];
            if (!_stunGunDrawn && _selectedWeapon == weapon)
                return true;

            _stunGunDrawn = false;
            _selectedWeapon = weapon;
            WeaponChanged?.Invoke(_selectedWeapon);
            return true;
        }

        public bool Contains(WeaponType type) => IndexOf(type) >= 0;

        public bool TryGet(WeaponType type, out IWeapon weapon)
        {
            int index = IndexOf(type);
            if (index < 0)
            {
                weapon = null;
                return false;
            }

            weapon = _weapons[index];
            return true;
        }

        private int IndexOf(WeaponType type)
        {
            for (int i = 0; i < _weapons.Count; i++)
            {
                if (_weapons[i].Type == type)
                    return i;
            }

            return -1;
        }
    }
}
