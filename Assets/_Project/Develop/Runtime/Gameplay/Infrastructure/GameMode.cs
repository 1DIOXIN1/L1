using System;
using _Project.Develop.Runtime.Configs.Core.Gameplay;
using _Project.Develop.Runtime.Configs.Meta.Wallet;
using _Project.Develop.Runtime.Gameplay.Main;
using _Project.Develop.Runtime.Meta.Features.Wallet;
using _Project.Develop.Runtime.Utilities.ConfigsManagement;

namespace _Project.Develop.Runtime.Gameplay.Infrastructure
{
    public class GameMode
    {
        public event Action Win;
        public event Action Defeat;

        private readonly SequenceChecker _sequenceChecker;
        private readonly WalletService _walletService;
        private readonly ConfigsProviderService _configsProviderService;

        public GameMode(SequenceChecker sequenceChecker, WalletService walletService, ConfigsProviderService configsProviderService)
        {
            _sequenceChecker = sequenceChecker;
            _walletService = walletService;
            _configsProviderService = configsProviderService;
        }

        public void Start()
        {
            _sequenceChecker.OnCorrectSequenceCheck += OnRightSequence;
            _sequenceChecker.OnWrongSequenceCheck += OnWrongSequence;
        }

        private void OnRightSequence()
        {
            _walletService.Add(CurrencyTypes.Gold, _configsProviderService.GetConfig<StartWalletConfig>().ValueToAdd);
            Win?.Invoke();
        }

        private void OnWrongSequence()
        {
            var valueToSpend = _configsProviderService.GetConfig<StartWalletConfig>().ValueToSpend;
            
            if(_walletService.Enough(CurrencyTypes.Gold, valueToSpend))
                _walletService.Spend(CurrencyTypes.Gold, valueToSpend);
            
            Defeat?.Invoke();
        }
    }
}