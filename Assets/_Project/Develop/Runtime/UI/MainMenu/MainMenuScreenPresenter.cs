using System.Collections.Generic;
using _Project.Develop.Runtime.UI.Core;
using _Project.Develop.Runtime.UI.Wallet;

namespace _Project.Develop.Runtime.UI.MainMenu
{
    public class MainMenuScreenPresenter : IPresenter
    {
        private readonly MainMenuScreenView _screen;
        
        private readonly ProjectPresentersFactory _projectPresentersFactory;
        
        private readonly MainMenuPopupService _popupService;
        
        private readonly List<IPresenter> _childPresenters = new();

        public MainMenuScreenPresenter(
            MainMenuScreenView screen,
            ProjectPresentersFactory projectPresentersFactory,
            MainMenuPopupService popupService)
        {
            _screen = screen;
            _projectPresentersFactory = projectPresentersFactory;
            _popupService = popupService;
        }

        public void Initialize()
        {
            _screen.OpenTestPopupButtonClicked += OnOpenTestPopupButtonClicked;
            
            CreateWallet();
            
            foreach (var childPresenter in _childPresenters)
                childPresenter.Initialize();
        }

        public void Dispose()
        {
            _screen.OpenTestPopupButtonClicked -= OnOpenTestPopupButtonClicked;
            
            foreach (var childPresenter in _childPresenters)
                childPresenter.Dispose();
            
            _childPresenters.Clear();
        }
        
        private void CreateWallet()
        {
            WalletPresenter walletPresenter = _projectPresentersFactory.CreateWalletPresenter(_screen.WalletView);
            
            _childPresenters.Add(walletPresenter);
        }
        
        private void OnOpenTestPopupButtonClicked()
        {
            _popupService.OpenTestPopup();
        }
    }
}