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
        private readonly Dictionary<WeaponType, int> _reserveAmmoByWeapon = new();
        private readonly List<WeaponType> _ownedWeapons = new();

        private int _health;
        private WeaponType _selectedWeaponType;

        public PlayerStateService(PlayerDataProvider playerDataProvider, ConfigsProviderService configsProviderService)
        {
            _configsProviderService = configsProviderService;

            playerDataProvider.RegisterReader(this);
            playerDataProvider.RegisterWriter(this);
        }

        public int Health => _health;
        public int MaxHealth => _configsProviderService.GetConfig<PlayerConfig>().Health;
        public WeaponType SelectedWeaponType => _selectedWeaponType;
        public IReadOnlyList<WeaponType> OwnedWeapons => _ownedWeapons;
        public IReadOnlyDictionary<WeaponType, int> AmmoByWeapon => _ammoByWeapon;
        public IReadOnlyDictionary<WeaponType, int> ReserveAmmoByWeapon => _reserveAmmoByWeapon;

        public int GetAmmo(WeaponType type)
        {
            if (_ammoByWeapon.TryGetValue(type, out int ammo))
                return ammo;

            return GetWeaponConfig(type).MagazineSize;
        }

        public int GetReserveAmmo(WeaponType type)
        {
            if (_reserveAmmoByWeapon.TryGetValue(type, out int reserve))
                return reserve;

            return GetWeaponConfig(type).ReserveAmmo;
        }

        public void CaptureFrom(PlayerCharacter player, WeaponInventory weaponInventory)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));
            if (weaponInventory == null)
                throw new ArgumentNullException(nameof(weaponInventory));

            _health = player.CurrentHealth;
            _selectedWeaponType = weaponInventory.CurrentWeaponType ?? _selectedWeaponType;

            _ammoByWeapon.Clear();
            _reserveAmmoByWeapon.Clear();
            _ownedWeapons.Clear();

            IReadOnlyList<IWeapon> weapons = weaponInventory.Weapons;
            for (int i = 0; i < weapons.Count; i++)
            {
                IWeapon weapon = weapons[i];
                _ownedWeapons.Add(weapon.Type);
                _ammoByWeapon[weapon.Type] = weapon.Ammo;
                _reserveAmmoByWeapon[weapon.Type] = weapon.ReserveAmmo;
            }
        }

        public void RestoreHealth()
        {
            _health = MaxHealth;
        }

        public void SetSelectedWeaponType(WeaponType type)
        {
            _selectedWeaponType = type;
        }

        public void RefillAmmo()
        {
            ApplyOriginCombatStats();
        }

        public void WriteTo(PlayerData data)
        {
            data.Health = _health;
            data.SelectedWeaponType = _selectedWeaponType;
            data.OwnedWeapons = new List<WeaponType>(_ownedWeapons);
            data.AmmoByWeapon = new Dictionary<WeaponType, int>(_ammoByWeapon);
            data.ReserveAmmoByWeapon = new Dictionary<WeaponType, int>(_reserveAmmoByWeapon);
        }

        public void ReadFrom(PlayerData data)
        {
            if (data.AmmoByWeapon == null)
            {
                ApplyOriginCombatStats();
                return;
            }

            PlayerWeaponInventoryConfig inventoryConfig =
                _configsProviderService.GetConfig<PlayerWeaponInventoryConfig>();

            _health = Mathf.Clamp(data.Health, 0, MaxHealth);
            _selectedWeaponType = data.SelectedWeaponType;

            _ownedWeapons.Clear();
            if (data.OwnedWeapons != null && data.OwnedWeapons.Count > 0)
            {
                for (int i = 0; i < data.OwnedWeapons.Count; i++)
                    _ownedWeapons.Add(data.OwnedWeapons[i]);
            }
            else
            {
                for (int i = 0; i < inventoryConfig.StartingWeapons.Count; i++)
                    _ownedWeapons.Add(inventoryConfig.StartingWeapons[i]);
            }

            if (_ownedWeapons.Count > 0 && _ownedWeapons.Contains(_selectedWeaponType) == false)
                _selectedWeaponType = _ownedWeapons[0];
            else if (_ownedWeapons.Count == 0)
                _selectedWeaponType = inventoryConfig.DefaultSelectedWeapon;

            _ammoByWeapon.Clear();
            _reserveAmmoByWeapon.Clear();

            foreach (KeyValuePair<WeaponType, int> pair in data.AmmoByWeapon)
            {
                int magazineSize = GetWeaponConfig(pair.Key).MagazineSize;
                _ammoByWeapon[pair.Key] = Mathf.Clamp(pair.Value, 0, magazineSize);
            }

            if (data.ReserveAmmoByWeapon != null)
            {
                foreach (KeyValuePair<WeaponType, int> pair in data.ReserveAmmoByWeapon)
                    _reserveAmmoByWeapon[pair.Key] = Mathf.Max(0, pair.Value);
            }
            else
            {
                foreach (KeyValuePair<WeaponType, int> pair in _ammoByWeapon)
                    _reserveAmmoByWeapon[pair.Key] = GetWeaponConfig(pair.Key).ReserveAmmo;
            }
        }

        private void ApplyOriginCombatStats()
        {
            PlayerConfig playerConfig = _configsProviderService.GetConfig<PlayerConfig>();
            PlayerWeaponInventoryConfig inventoryConfig = _configsProviderService.GetConfig<PlayerWeaponInventoryConfig>();

            _health = playerConfig.Health;
            _selectedWeaponType = inventoryConfig.DefaultSelectedWeapon;

            _ownedWeapons.Clear();
            _ammoByWeapon.Clear();
            _reserveAmmoByWeapon.Clear();

            for (int i = 0; i < inventoryConfig.StartingWeapons.Count; i++)
            {
                WeaponType type = inventoryConfig.StartingWeapons[i];
                WeaponConfig weaponConfig = GetWeaponConfig(type);
                _ownedWeapons.Add(type);
                _ammoByWeapon[type] = weaponConfig.MagazineSize;
                _reserveAmmoByWeapon[type] = weaponConfig.ReserveAmmo;
            }
        }

        private WeaponConfig GetWeaponConfig(WeaponType type)
        {
            return _configsProviderService.GetConfig<WeaponsCatalogConfig>().GetWeapon(type);
        }
    }
}
