using System;
using System.Collections.Generic;
using _Project.Develop.Runtime.Configs.Meta.Wallet;
using _Project.Develop.Runtime.Meta.Features.Wallet;
using _Project.Develop.Runtime.Utilities.ConfigsManagement;

namespace _Project.Develop.Runtime.Utilities.DataManagement.DataProviders
{
    public class PlayerDataProvider : DataProvider<PlayerData>
    {
        private readonly ConfigsProviderService _configsProviderService;
        
        public PlayerDataProvider(ISaveLoadService saveLoadService, ConfigsProviderService configsProviderService) : base(saveLoadService)
        {
            _configsProviderService =  configsProviderService;
        }

        protected override PlayerData GetOriginData()
        {
            return new PlayerData()
            {
                WalletData = InitWalletData(),
            };
        }

        private Dictionary<CurrencyTypes, int> InitWalletData()
        {
            Dictionary<CurrencyTypes, int> walletData = new();
            StartWalletConfig startWalletConfig = _configsProviderService.GetConfig<StartWalletConfig>();
            
            foreach (CurrencyTypes currencyType in Enum.GetValues(typeof(CurrencyTypes)))
                walletData[currencyType] = startWalletConfig.GetValueFor((currencyType));
            
            return walletData;
        }
    }
}