using System;
using System.Collections.Generic;
using _Project.Develop.Runtime.Configs.Meta.Weapon;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.WeaponsType;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Develop.Runtime.Configs.Meta.Player
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Inventory/WeaponInventory", fileName = "WeaponInventoryConfig")]
    public class WeaponInventoryConfig : ScriptableObject
    {
        [field: SerializeField] public SlotWeaponType DefaultSelectedSlot { get; private set; } = SlotWeaponType.PrimarySlot;
        public IReadOnlyList<StartWeaponSlot> Slots => slots;

        [SerializeField] private List<StartWeaponSlot> slots = new()
        {
            new StartWeaponSlot(SlotWeaponType.PrimarySlot, WeaponType.Smg),
            new StartWeaponSlot(SlotWeaponType.SecondarySlot, WeaponType.Usp)
        };

        [Serializable]
        public class StartWeaponSlot
        {
            [field: SerializeField] public SlotWeaponType SlotType { get; private set; }
            [field: SerializeField] public WeaponType WeaponType { get; private set; }

            public StartWeaponSlot(SlotWeaponType slotType, WeaponType weaponType)
            {
                SlotType = slotType;
                WeaponType = weaponType;
            }
        }
    }
}