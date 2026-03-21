using _Project.Develop.Runtime.Configs.Meta.Progress;
using _Project.Develop.Runtime.Meta.Features.Wallet;
using _Project.Develop.Runtime.Utilities;
using _Project.Develop.Runtime.Utilities.ConfigsManagement;
using _Project.Develop.Runtime.Utilities.DataManagement;
using _Project.Develop.Runtime.Utilities.DataManagement.DataProviders;
using UnityEngine;

namespace _Project.Develop.Runtime.Meta.Features.Progress
{
    public class ProgressService : IDataWriter<GameplayData>, IDataReader<GameplayData>
    {
        private GameplayDataProvider _gameplayDataProvider;
        private WalletService _walletService;
        private ConfigsProviderService _configsProviderService;
        private CoroutinesPerformer _coroutinesPerformer;
         
        private int _countWins;
        private int _countLoss;
        
        public ProgressService(GameplayDataProvider gameplayDataProvider, 
            WalletService walletService, 
            ConfigsProviderService configsProviderService, 
            CoroutinesPerformer coroutinesPerformer)
        {
            _gameplayDataProvider = gameplayDataProvider;
            _walletService = walletService;
            _configsProviderService = configsProviderService;
            _coroutinesPerformer = coroutinesPerformer;
            
            _gameplayDataProvider.RegisterReader(this);
            _gameplayDataProvider.RegisterWriter(this);
        }
        
        public int CountWins => _countWins;
        
        public int CountLoss => _countLoss;
        
        public void Win() => _countWins++;
        
        public void Lose() => _countLoss++;

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

        public void WriteTo(GameplayData data)
        {
            data.CountWins = _countWins;
            data.CountLoss = _countLoss;
        }

        public void ReadFrom(GameplayData data)
        {
            _countWins = data.CountWins;
            _countLoss = data.CountLoss;
        }
    }
}