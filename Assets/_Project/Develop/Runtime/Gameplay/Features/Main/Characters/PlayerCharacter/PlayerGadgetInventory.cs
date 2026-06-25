using System.Collections.Generic;
using _Project.Develop.Runtime.Configs.Meta.Characters.Player;
using _Project.Develop.Runtime.Gameplay.Features.Main.Gadget;
using _Project.Develop.Runtime.Gameplay.Features.Main.Gadget.GadgetsType;
using _Project.Develop.Runtime.Utilities.ConfigsManagement;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter
{
    public class PlayerGadgetInventory
    {
        public PlayerGadgetInventory(
            ConfigsProviderService configsProviderService,
            GadgetFactory factory)
        {
            ConfigsProviderService = configsProviderService;
            Factory = factory;
        }

        private ConfigsProviderService ConfigsProviderService { get; set; }
        private GadgetFactory Factory { get; set; }

        public GadgetInventory CreatePlayerGadgetInventory(
            Transform usePoint,
            GameObject owner)
        {
            var inventoryConfig = ConfigsProviderService.GetConfig<PlayerWeaponInventoryConfig>();
            var slots = new Dictionary<SlotGadgetType, GadgetSlot>();

            foreach (var slotData in inventoryConfig.GadgetSlots)
            {
                IGadget gadget = Factory.CreateGadget(slotData.GadgetType, usePoint, owner);
                slots[slotData.SlotType] = new GadgetSlot(gadget, slotData.SlotType);
            }

            var inventory = new GadgetInventory(slots);
            inventory.EquipGadget(inventoryConfig.DefaultSelectedGadgetSlot);

            return inventory;
        }
    }
}
