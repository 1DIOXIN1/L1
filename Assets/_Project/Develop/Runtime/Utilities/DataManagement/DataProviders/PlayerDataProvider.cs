using System;
using System.Collections.Generic;
using _Project.Develop.Runtime.Configs.Meta.Characters.Player;
using _Project.Develop.Runtime.Configs.Meta.Wallet;
using _Project.Develop.Runtime.Configs.Meta.Weapon;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.WeaponsType;
using _Project.Develop.Runtime.Meta.Features.Wallet;
using _Project.Develop.Runtime.Utilities.ConfigsManagement;

namespace _Project.Develop.Runtime.Utilities.DataManagement.DataProviders
{
    public class PlayerDataProvider : DataProvider<PlayerData>
    {
        private readonly ConfigsProviderService _configsProviderService;

        public PlayerDataProvider(ISaveLoadService saveLoadService, ConfigsProviderService configsProviderService) : base(saveLoadService)
        {
            _configsProviderService = configsProviderService;
        }

        protected override PlayerData GetOriginData()
        {
            PlayerWeaponInventoryConfig inventoryConfig = _configsProviderService.GetConfig<PlayerWeaponInventoryConfig>();

            return new PlayerData
            {
                WalletData = InitWalletData(),
                Health = _configsProviderService.GetConfig<PlayerConfig>().Health,
                SelectedWeaponType = inventoryConfig.DefaultSelectedWeapon,
                OwnedWeapons = new List<WeaponType>(inventoryConfig.StartingWeapons),
                AmmoByWeapon = InitAmmoData(),
                ReserveAmmoByWeapon = InitReserveAmmoData()
            };
        }

        private Dictionary<CurrencyTypes, int> InitWalletData()
        {
            Dictionary<CurrencyTypes, int> walletData = new();
            StartWalletConfig startWalletConfig = _configsProviderService.GetConfig<StartWalletConfig>();

            foreach (CurrencyTypes currencyType in Enum.GetValues(typeof(CurrencyTypes)))
                walletData[currencyType] = startWalletConfig.GetValueFor(currencyType);

            return walletData;
        }

        private Dictionary<WeaponType, int> InitAmmoData()
        {
            Dictionary<WeaponType, int> ammo = new();
            PlayerWeaponInventoryConfig inventoryConfig = _configsProviderService.GetConfig<PlayerWeaponInventoryConfig>();
            WeaponsCatalogConfig catalog = _configsProviderService.GetConfig<WeaponsCatalogConfig>();

            for (int i = 0; i < inventoryConfig.StartingWeapons.Count; i++)
            {
                WeaponType type = inventoryConfig.StartingWeapons[i];
                ammo[type] = catalog.GetWeapon(type).MagazineSize;
            }

            return ammo;
        }

        private Dictionary<WeaponType, int> InitReserveAmmoData()
        {
            Dictionary<WeaponType, int> reserve = new();
            PlayerWeaponInventoryConfig inventoryConfig = _configsProviderService.GetConfig<PlayerWeaponInventoryConfig>();
            WeaponsCatalogConfig catalog = _configsProviderService.GetConfig<WeaponsCatalogConfig>();

            for (int i = 0; i < inventoryConfig.StartingWeapons.Count; i++)
            {
                WeaponType type = inventoryConfig.StartingWeapons[i];
                reserve[type] = catalog.GetWeapon(type).ReserveAmmo;
            }

            return reserve;
        }
    }
}
