using _Project.Develop.Runtime.Configs.Meta.Wallet;
using _Project.Develop.Runtime.Infrastructure.DI;
using _Project.Develop.Runtime.Meta.Features.Wallet;
using _Project.Develop.Runtime.UI.CommonViews;
using _Project.Develop.Runtime.UI.Core;
using _Project.Develop.Runtime.UI.Core.TestPopup;
using _Project.Develop.Runtime.UI.Wallet;
using _Project.Develop.Runtime.Utilities.ConfigsManagement;
using _Project.Develop.Runtime.Utilities.Reactive;

namespace _Project.Develop.Runtime.UI
{
    public class ProjectPresentersFactory
    {
        private readonly DIContainer _container;

        public ProjectPresentersFactory(DIContainer container)
        {
            _container = container;
        }

        public CurrencyPresenter CreateCurrencyPresenter(
            IconTextView iconTextView,
            IReadOnlyVariable<int> currency,
            CurrencyTypes currencyType)
        {
            CurrencyIconsConfig config = _container.Resolve<ConfigsProviderService>().GetConfig<CurrencyIconsConfig>();
            
            return new CurrencyPresenter(currency, currencyType, config, iconTextView);
        }

        public WalletPresenter CreateWalletPresenter(IconTextListView iconTextListView)
        {
            WalletService walletService = _container.Resolve<WalletService>();
            ViewsFactory viewsFactory = _container.Resolve<ViewsFactory>();
            
            return new WalletPresenter(
                walletService, 
                this, 
                viewsFactory,
                iconTextListView);
        }
        
        public TestPopupPresenter CreateTestPopupPresenter(TestPopupView view)
        {
            return new TestPopupPresenter(view);
        }
    }
}