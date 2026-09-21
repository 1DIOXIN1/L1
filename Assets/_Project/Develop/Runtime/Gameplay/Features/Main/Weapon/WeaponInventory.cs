using System;
using System.Collections.Generic;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.WeaponsType;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Weapon
{
    public class WeaponInventory
    {
        private readonly List<IWeapon> _weapons = new();
        private IWeapon _currentWeapon;

        public event Action<IWeapon> WeaponChanged;
        public event Action Changed;

        public IWeapon CurrentWeapon => _currentWeapon;
        public WeaponType? CurrentWeaponType => _currentWeapon?.Type;
        public IReadOnlyList<IWeapon> Weapons => _weapons;

        public bool Add(IWeapon weapon)
        {
            if (weapon == null)
                return false;

            if (Contains(weapon.Type))
                return false;

            _weapons.Add(weapon);
            Changed?.Invoke();

            if (_currentWeapon == null)
                Equip(weapon.Type);

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

            if (_currentWeapon == removed)
            {
                _currentWeapon = _weapons.Count > 0 ? _weapons[0] : null;
                WeaponChanged?.Invoke(_currentWeapon);
            }

            return true;
        }

        public bool Equip(WeaponType type)
        {
            int index = IndexOf(type);
            if (index < 0)
                return false;

            IWeapon weapon = _weapons[index];
            if (_currentWeapon == weapon)
                return true;

            _currentWeapon = weapon;
            WeaponChanged?.Invoke(_currentWeapon);
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
