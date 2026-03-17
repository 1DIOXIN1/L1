using _Project.Develop.Runtime.Configs.Meta.Progress;
using _Project.Develop.Runtime.Meta.Features.Wallet;
using _Project.Develop.Runtime.Utilities;
using _Project.Develop.Runtime.Utilities.ConfigsManagement;
using _Project.Develop.Runtime.Utilities.DataManagement.DataProviders;
using UnityEngine;

namespace _Project.Develop.Runtime.Meta.Features.Progress
{
    public class ResetProgressService
    {
        private GameplayDataProvider _gameplayDataProvider;
        private WalletService _walletService;
        private ConfigsProviderService _configsProviderService;
        private CoroutinesPerformer _coroutinesPerformer;
        
        public ResetProgressService(GameplayDataProvider gameplayDataProvider, 
            WalletService walletService, 
            ConfigsProviderService configsProviderService, 
            CoroutinesPerformer coroutinesPerformer)
        {
            _gameplayDataProvider = gameplayDataProvider;
            _walletService = walletService;
            _configsProviderService = configsProviderService;
            _coroutinesPerformer = coroutinesPerformer;
        }

        public void TryReset()
        {
            var valueToReset = _configsProviderService.GetConfig<ProgressConfig>().ValueToResetProgress;

            if (_walletService.GetCurrency(CurrencyTypes.Gold).Value >= valueToReset)
            {
                _walletService.Spend(CurrencyTypes.Gold, valueToReset);
                _gameplayDataProvider.Reset();
                _coroutinesPerformer.StartPerform(_gameplayDataProvider.Save());
                
                Debug.Log("Сброс");
            }
            else
            {
                Debug.Log("Не хватает валюты");
            }
        }
    }
}