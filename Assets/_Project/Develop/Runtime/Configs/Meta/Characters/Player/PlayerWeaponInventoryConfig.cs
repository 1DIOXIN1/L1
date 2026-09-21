using System.Collections.Generic;
using _Project.Develop.Runtime.Configs.Meta.Characters.Player;
using _Project.Develop.Runtime.Gameplay.Features.Main.Gadget.GadgetsType;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.WeaponsType;
using UnityEngine;

namespace _Project.Develop.Runtime.Configs.Meta.Characters.Player
{
    [CreateAssetMenu(menuName = "Configs/Core/Gameplay/Inventory/PlayerWeaponInventory", fileName = "PlayerWeaponInventoryConfig")]
    public class PlayerWeaponInventoryConfig : ScriptableObject
    {
        [field: SerializeField] public WeaponType DefaultSelectedWeapon { get; private set; } = WeaponType.Ak74;
        [field: SerializeField] public SlotGadgetType DefaultSelectedGadgetSlot { get; private set; } = SlotGadgetType.GrenadeSlot;

        public IReadOnlyList<WeaponType> StartingWeapons => startingWeapons;
        public IReadOnlyList<StartGadgetSlot> GadgetSlots => gadgetSlots;

        [SerializeField] private List<WeaponType> startingWeapons = new()
        {
            WeaponType.Ak74,
            WeaponType.Usp
        };

        [SerializeField] private List<StartGadgetSlot> gadgetSlots = new()
        {
            new StartGadgetSlot(SlotGadgetType.GrenadeSlot, GadgetType.Grenade)
        };

        [System.Serializable]
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
