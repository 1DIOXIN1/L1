using System;
using System.Collections.Generic;
using _Project.Develop.Runtime.Configs.Meta.Weapon;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Weapon
{
    public class WeaponInventory
    {
        private SlotWeaponType? _currentSlot;
        private IWeapon _currentWeapon;

        private readonly Dictionary<SlotWeaponType, WeaponSlot> _slots;

        public event Action<IWeapon> WeaponChanged;

        public WeaponInventory(Dictionary<SlotWeaponType, WeaponSlot> weaponSlots)
        {
            _slots = new Dictionary<SlotWeaponType, WeaponSlot>(weaponSlots);
        }

        public IWeapon CurrentWeapon => _currentWeapon;
        public SlotWeaponType? CurrentSlot => _currentSlot;
        public IReadOnlyDictionary<SlotWeaponType, WeaponSlot> Slots => _slots;

        public void EquipWeapon(SlotWeaponType slotType)
        {
            if (!_slots.TryGetValue(slotType, out WeaponSlot slot))
                return;

            if (slot.Weapon == null)
                return;

            if (_currentWeapon == slot.Weapon)
                return;

            _currentSlot = slot.SlotType;
            _currentWeapon = slot.Weapon;
            WeaponChanged?.Invoke(_currentWeapon);
        }

        public void RemoveWeaponOutInventory(SlotWeaponType slotType)
        {
            if (!_slots.TryGetValue(slotType, out WeaponSlot slot))
                return;

            slot.Clear();

            if (_currentSlot.HasValue && _currentSlot.Value == slotType)
            {
                _currentSlot = null;
                _currentWeapon = null;
                WeaponChanged?.Invoke(null);
            }
        }
    }
}
