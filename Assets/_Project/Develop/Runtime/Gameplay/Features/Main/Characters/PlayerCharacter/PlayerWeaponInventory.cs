using System.Collections.Generic;
using _Project.Develop.Runtime.Configs.Meta.Characters.Player;
using _Project.Develop.Runtime.Configs.Meta.Weapon;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon;
using _Project.Develop.Runtime.Meta.Features.Player;
using _Project.Develop.Runtime.Utilities.ConfigsManagement;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter
{
    public class PlayerWeaponInventory
    {
        private readonly WeaponFactory _factory;
        private readonly ConfigsProviderService _configsProviderService;
        private readonly PlayerStateService _playerStateService;

        public PlayerWeaponInventory(
            ConfigsProviderService configsProviderService,
            WeaponFactory factory,
            PlayerStateService playerStateService)
        {
            _factory = factory;
            _configsProviderService = configsProviderService;
            _playerStateService = playerStateService;
        }

        public WeaponInventory CreatePlayerWeaponInventory(
            Transform playerTransform,
            Transform firePoint,
            GameObject owner)
        {
            var inventoryConfig = _configsProviderService.GetConfig<PlayerWeaponInventoryConfig>();
            var slots = new Dictionary<SlotWeaponType, WeaponSlot>();

            foreach (var slotData in inventoryConfig.Slots)
            {
                int ammo = _playerStateService.GetAmmo(slotData.WeaponType);
                int reserveAmmo = _playerStateService.GetReserveAmmo(slotData.WeaponType);
                IWeapon weapon = _factory.CreateWeapon(slotData.WeaponType, firePoint, owner, ammo, reserveAmmo);
                slots[slotData.SlotType] = new WeaponSlot(weapon, slotData.SlotType);
            }

            var inventory = new WeaponInventory(slots);
            inventory.EquipWeapon(_playerStateService.SelectedWeaponSlot);
            return inventory;
        }
    }
}
