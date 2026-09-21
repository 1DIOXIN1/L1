using System.Collections.Generic;
using _Project.Develop.Runtime.Configs.Meta.Characters.Player;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.WeaponsType;
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
            var inventory = new WeaponInventory();
            var inventoryConfig = _configsProviderService.GetConfig<PlayerWeaponInventoryConfig>();

            IReadOnlyList<WeaponType> owned = _playerStateService.OwnedWeapons;
            if (owned == null || owned.Count == 0)
                owned = inventoryConfig.StartingWeapons;

            for (int i = 0; i < owned.Count; i++)
            {
                WeaponType type = owned[i];
                int ammo = _playerStateService.GetAmmo(type);
                int reserveAmmo = _playerStateService.GetReserveAmmo(type);
                IWeapon weapon = _factory.CreateWeapon(type, firePoint, owner, ammo, reserveAmmo);
                inventory.Add(weapon);
            }

            WeaponType selected = _playerStateService.SelectedWeaponType;
            if (inventory.Contains(selected))
                inventory.Equip(selected);
            else if (inventory.Weapons.Count > 0)
                inventory.Equip(inventory.Weapons[0].Type);

            return inventory;
        }
    }
}
