using System;
using System.Collections.Generic;
using _Project.Develop.Runtime.Configs.Meta.Characters.Player;
using _Project.Develop.Runtime.Configs.Meta.Weapon;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.WeaponsType;
using _Project.Develop.Runtime.Utilities.ConfigsManagement;
using _Project.Develop.Runtime.Utilities.DataManagement;
using _Project.Develop.Runtime.Utilities.DataManagement.DataProviders;
using UnityEngine;
using PlayerCharacter = _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter.Player;

namespace _Project.Develop.Runtime.Meta.Features.Player
{
    public class PlayerStateService : IDataReader<PlayerData>, IDataWriter<PlayerData>
    {
        private readonly ConfigsProviderService _configsProviderService;
        private readonly Dictionary<WeaponType, int> _ammoByWeapon = new();

        private int _health;
        private SlotWeaponType _selectedWeaponSlot;

        public PlayerStateService(PlayerDataProvider playerDataProvider, ConfigsProviderService configsProviderService)
        {
            _configsProviderService = configsProviderService;

            playerDataProvider.RegisterReader(this);
            playerDataProvider.RegisterWriter(this);
        }

        public int Health => _health;
        public int MaxHealth => _configsProviderService.GetConfig<PlayerConfig>().Health;
        public SlotWeaponType SelectedWeaponSlot => _selectedWeaponSlot;
        public IReadOnlyDictionary<WeaponType, int> AmmoByWeapon => _ammoByWeapon;

        public int GetAmmo(WeaponType type)
        {
            if (_ammoByWeapon.TryGetValue(type, out int ammo))
                return ammo;

            WeaponsCatalogConfig catalog = _configsProviderService.GetConfig<WeaponsCatalogConfig>();
            return catalog.GetWeapon(type).MagazineSize;
        }

        public void CaptureFrom(PlayerCharacter player, WeaponInventory weaponInventory)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));
            if (weaponInventory == null)
                throw new ArgumentNullException(nameof(weaponInventory));

            _health = player.CurrentHealth;
            _selectedWeaponSlot = weaponInventory.CurrentSlot ?? _selectedWeaponSlot;

            _ammoByWeapon.Clear();
            foreach (KeyValuePair<SlotWeaponType, WeaponSlot> pair in weaponInventory.Slots)
            {
                if (pair.Value?.Weapon == null)
                    continue;

                _ammoByWeapon[pair.Value.Weapon.Type] = pair.Value.Weapon.Ammo;
            }
        }

        public void RestoreHealth()
        {
            _health = MaxHealth;
        }

        public void RefillAmmo()
        {
            WeaponsCatalogConfig catalog = _configsProviderService.GetConfig<WeaponsCatalogConfig>();
            PlayerWeaponInventoryConfig inventoryConfig = _configsProviderService.GetConfig<PlayerWeaponInventoryConfig>();

            _ammoByWeapon.Clear();
            foreach (PlayerWeaponInventoryConfig.StartWeaponSlot slot in inventoryConfig.Slots)
            {
                WeaponConfig weaponConfig = catalog.GetWeapon(slot.WeaponType);
                _ammoByWeapon[slot.WeaponType] = weaponConfig.MagazineSize;
            }
        }

        public void WriteTo(PlayerData data)
        {
            data.Health = _health;
            data.SelectedWeaponSlot = _selectedWeaponSlot;
            data.AmmoByWeapon = new Dictionary<WeaponType, int>(_ammoByWeapon);
        }

        public void ReadFrom(PlayerData data)
        {
            if (data.AmmoByWeapon == null)
            {
                ApplyOriginCombatStats();
                return;
            }

            _health = Mathf.Clamp(data.Health, 0, MaxHealth);
            _selectedWeaponSlot = data.SelectedWeaponSlot == SlotWeaponType.None
                ? _configsProviderService.GetConfig<PlayerWeaponInventoryConfig>().DefaultSelectedSlot
                : data.SelectedWeaponSlot;

            _ammoByWeapon.Clear();
            foreach (KeyValuePair<WeaponType, int> pair in data.AmmoByWeapon)
            {
                WeaponsCatalogConfig catalog = _configsProviderService.GetConfig<WeaponsCatalogConfig>();
                int magazineSize = catalog.GetWeapon(pair.Key).MagazineSize;
                _ammoByWeapon[pair.Key] = Mathf.Clamp(pair.Value, 0, magazineSize);
            }
        }

        private void ApplyOriginCombatStats()
        {
            PlayerConfig playerConfig = _configsProviderService.GetConfig<PlayerConfig>();
            PlayerWeaponInventoryConfig inventoryConfig = _configsProviderService.GetConfig<PlayerWeaponInventoryConfig>();
            WeaponsCatalogConfig catalog = _configsProviderService.GetConfig<WeaponsCatalogConfig>();

            _health = playerConfig.Health;
            _selectedWeaponSlot = inventoryConfig.DefaultSelectedSlot;

            _ammoByWeapon.Clear();
            foreach (PlayerWeaponInventoryConfig.StartWeaponSlot slot in inventoryConfig.Slots)
            {
                WeaponConfig weaponConfig = catalog.GetWeapon(slot.WeaponType);
                _ammoByWeapon[slot.WeaponType] = weaponConfig.MagazineSize;
            }
        }
    }
}
