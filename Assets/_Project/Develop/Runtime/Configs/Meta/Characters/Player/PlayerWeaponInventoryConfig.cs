using System;
using System.Collections.Generic;
using _Project.Develop.Runtime.Configs.Meta.Weapon;
using _Project.Develop.Runtime.Gameplay.Features.Main.Gadget.GadgetsType;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.WeaponsType;
using UnityEngine;

namespace _Project.Develop.Runtime.Configs.Meta.Characters.Player
{
    [CreateAssetMenu(menuName = "Configs/Core/Gameplay/Inventory/PlayerWeaponInventory", fileName = "PlayerWeaponInventoryConfig")]
    public class PlayerWeaponInventoryConfig : ScriptableObject
    {
        [field: SerializeField] public SlotWeaponType DefaultSelectedSlot { get; private set; } = SlotWeaponType.PrimarySlot;
        [field: SerializeField] public SlotGadgetType DefaultSelectedGadgetSlot { get; private set; } = SlotGadgetType.GrenadeSlot;
        public IReadOnlyList<StartWeaponSlot> Slots => slots;
        public IReadOnlyList<StartGadgetSlot> GadgetSlots => gadgetSlots;

        [SerializeField] private List<StartWeaponSlot> slots = new()
        {
            new StartWeaponSlot(SlotWeaponType.PrimarySlot, WeaponType.Ak74),
            new StartWeaponSlot(SlotWeaponType.SecondarySlot, WeaponType.Usp)
        };

        [SerializeField] private List<StartGadgetSlot> gadgetSlots = new()
        {
            new StartGadgetSlot(SlotGadgetType.GrenadeSlot, GadgetType.Grenade)
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

        [Serializable]
        public class StartGadgetSlot
        {
            [field: SerializeField] public SlotGadgetType SlotType { get; private set; }
            [field: SerializeField] public GadgetType GadgetType { get; private set; }

            public StartGadgetSlot(SlotGadgetType slotType, GadgetType gadgetType)
            {
                SlotType = slotType;
                GadgetType = gadgetType;
            }
        }
    }
}
