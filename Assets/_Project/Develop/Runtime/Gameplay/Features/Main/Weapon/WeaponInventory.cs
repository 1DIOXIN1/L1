using System.Collections.Generic;
using _Project.Develop.Runtime.Configs.Meta.Weapon;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Weapon
{
    public class WeaponInventory
    {
        private SlotWeaponType? _currentSlot;
        private IWeapon _currentWeapon;
        
        private readonly Dictionary<SlotWeaponType, WeaponSlot> _slots;

        public WeaponInventory(Dictionary<SlotWeaponType, WeaponSlot> weaponSlots) // взять с конфига 
        {
            _slots = new Dictionary<SlotWeaponType, WeaponSlot> (weaponSlots);
        }

        public IWeapon CurrentWeapon => _currentWeapon;
        
        public void EquipWeapon(SlotWeaponType slotType)
        {
            if (!_slots.TryGetValue(slotType, out WeaponSlot slot)) 
                return;
            
            if (slot.Weapon == null) return;
            
            _currentSlot = slot.SlotType;
            _currentWeapon = slot.Weapon;
        }

        public void RemoveWeaponOutInventory(SlotWeaponType slotType)
        {
            if (!_slots.TryGetValue(slotType, out WeaponSlot slot)) return;
            
            slot.Clear();
            
            if (_currentSlot.HasValue && _currentSlot.Value == slotType)
            {
                _currentSlot = null;
                _currentWeapon = null;
            }
        }
    }
}