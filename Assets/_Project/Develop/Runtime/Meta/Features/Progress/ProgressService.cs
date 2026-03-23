using System.Collections.Generic;
using _Project.Develop.Runtime.Configs.Meta.Progress;
using _Project.Develop.Runtime.Meta.Features.Wallet;
using _Project.Develop.Runtime.Utilities;
using _Project.Develop.Runtime.Utilities.ConfigsManagement;
using _Project.Develop.Runtime.Utilities.DataManagement;
using _Project.Develop.Runtime.Utilities.DataManagement.DataProviders;
using _Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace _Project.Develop.Runtime.Meta.Features.Progress
{
    public class ProgressService : IDataWriter<GameplayData>, IDataReader<GameplayData>
    {
        private GameplayDataProvider _gameplayDataProvider;
        private WalletService _walletService;
        private ConfigsProviderService _configsProviderService;
        private CoroutinesPerformer _coroutinesPerformer;
        
        private readonly Dictionary<ProgressTypes, ReactiveVariable<int>> _progressItems;
        
        public ProgressService(
            Dictionary<ProgressTypes, ReactiveVariable<int>> progressItems,
            GameplayDataProvider gameplayDataProvider, 
            WalletService walletService, 
            ConfigsProviderService configsProviderService, 
            CoroutinesPerformer coroutinesPerformer)
        {
            _gameplayDataProvider = gameplayDataProvider;
            _walletService = walletService;
            _configsProviderService = configsProviderService;
            _coroutinesPerformer = coroutinesPerformer;

            _progressItems = new Dictionary<ProgressTypes, ReactiveVariable<int>>(progressItems);
            
            _gameplayDataProvider.RegisterReader(this);
            _gameplayDataProvider.RegisterWriter(this);
        }
        
        public IReadOnlyVariable<int> GetProgress(ProgressTypes progressType) => _progressItems[progressType];
        
        public void Win() => _progressItems[ProgressTypes.CountWins].Value++;
        
        public void Lose() => _progressItems[ProgressTypes.CountLosses].Value++;

        public bool TryReset()
        {
            var valueToReset = _configsProviderService.GetConfig<ProgressConfig>().ValueToResetProgress;

            if (_walletService.GetCurrency(CurrencyTypes.Gold).Value >= valueToReset)
            {
                _walletService.Spend(CurrencyTypes.Gold, valueToReset);
                _gameplayDataProvider.Reset();
                _coroutinesPerformer.StartPerform(_gameplayDataProvider.Save());

                return true;
            }
            else
            {
                return false;
            }
        }

        public void WriteTo(GameplayData data)
        {
            data.CountWins = _progressItems[ProgressTypes.CountWins].Value;
            data.CountLoss = _progressItems[ProgressTypes.CountLosses].Value;
        }

        public void ReadFrom(GameplayData data)
        {
            _progressItems[ProgressTypes.CountWins].Value = data.CountWins;
            _progressItems[ProgressTypes.CountLosses].Value = data.CountLoss;
        }
    }
}