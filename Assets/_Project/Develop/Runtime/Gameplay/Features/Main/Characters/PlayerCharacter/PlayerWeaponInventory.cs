using System.Collections.Generic;
using _Project.Develop.Runtime.Configs.Meta.Player;
using _Project.Develop.Runtime.Configs.Meta.Weapon;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon;
using _Project.Develop.Runtime.Utilities.ConfigsManagement;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter
{
    public class PlayerWeaponInventory
    {
        public PlayerWeaponInventory(
            ConfigsProviderService configsProviderService,
            WeaponFactory factory)
        {
            Factory =  factory;
            ConfigsProviderService = configsProviderService;
        }

        private WeaponFactory Factory {get; set;}
        private ConfigsProviderService ConfigsProviderService {get; set;}

        public WeaponInventory CreatePlayerWeaponInventory(
            Transform playerTransform,
            Transform firePoint,
            GameObject owner)
        {
            var inventoryConfig = ConfigsProviderService.GetConfig<WeaponInventoryConfig>();
            var slots = new Dictionary<SlotWeaponType, WeaponSlot>();

            foreach (var slotData in inventoryConfig.Slots)
            {
                IWeapon weapon = Factory.CreateWeapon(slotData.WeaponType, playerTransform.position, firePoint, owner);
                slots[slotData.SlotType] = new WeaponSlot(weapon, slotData.SlotType);
            }

            var inventory = new WeaponInventory(slots);
            inventory.EquipWeapon(inventoryConfig.DefaultSelectedSlot);
            Debug.Log("Инвентарь создан");
            return inventory;
        }
    }
}