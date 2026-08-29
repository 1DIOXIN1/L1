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
            return new PlayerData
            {
                WalletData = InitWalletData(),
                Health = _configsProviderService.GetConfig<PlayerConfig>().Health,
                SelectedWeaponSlot = _configsProviderService.GetConfig<PlayerWeaponInventoryConfig>().DefaultSelectedSlot,
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

            foreach (PlayerWeaponInventoryConfig.StartWeaponSlot slot in inventoryConfig.Slots)
            {
                WeaponConfig weaponConfig = catalog.GetWeapon(slot.WeaponType);
                ammo[slot.WeaponType] = weaponConfig.MagazineSize;
            }

            return ammo;
        }

        private Dictionary<WeaponType, int> InitReserveAmmoData()
        {
            Dictionary<WeaponType, int> reserve = new();
            PlayerWeaponInventoryConfig inventoryConfig = _configsProviderService.GetConfig<PlayerWeaponInventoryConfig>();
            WeaponsCatalogConfig catalog = _configsProviderService.GetConfig<WeaponsCatalogConfig>();

            foreach (PlayerWeaponInventoryConfig.StartWeaponSlot slot in inventoryConfig.Slots)
            {
                WeaponConfig weaponConfig = catalog.GetWeapon(slot.WeaponType);
                reserve[slot.WeaponType] = weaponConfig.ReserveAmmo;
            }

            return reserve;
        }
    }
}
