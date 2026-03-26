using _Project.Develop.Runtime.Configs.Meta.Progress;
using _Project.Develop.Runtime.Meta.Features.Wallet;
using _Project.Develop.Runtime.Utilities.ConfigsManagement;

namespace _Project.Develop.Runtime.Meta.Features.Progress
{
    public class ResetProgressService
    {
        private readonly ProgressService _progressService;
        private readonly WalletService _walletService;
        private readonly ConfigsProviderService _configsProviderService;

        public ResetProgressService(
            ProgressService progressService,
            WalletService walletService,
            ConfigsProviderService configsProviderService)
        {
            _progressService = progressService;
            _walletService = walletService;
            _configsProviderService = configsProviderService;
        }

        public bool TryReset()
        {
            int requiredGold = _configsProviderService.GetConfig<ProgressConfig>().ValueToResetProgress;

            if (_walletService.GetCurrency(CurrencyTypes.Gold).Value >= requiredGold)
            {
                _walletService.Spend(CurrencyTypes.Gold, requiredGold);
                _progressService.ResetProgress();
                return true;
            }

            return false;
        }
    }
}