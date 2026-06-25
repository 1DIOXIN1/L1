using System.Collections.Generic;
using _Project.Develop.Runtime.UI.Core;
using _Project.Develop.Runtime.UI.Progress;
using _Project.Develop.Runtime.UI.Wallet;

namespace _Project.Develop.Runtime.UI.MainMenu
{
    public class MainMenuScreenPresenter : IPresenter
    {
        private readonly MainMenuScreenView _screen;
        private readonly ProjectPresentersFactory _projectPresentersFactory;
        private readonly MainMenuPopupService _popupService;
        private readonly MainMenuNavigationService _navigationService;
        private readonly List<IPresenter> _childPresenters = new();

        public MainMenuScreenPresenter(
            MainMenuScreenView screen,
            ProjectPresentersFactory projectPresentersFactory,
            MainMenuPopupService popupService,
            MainMenuNavigationService navigationService)
        {
            _screen = screen;
            _projectPresentersFactory = projectPresentersFactory;
            _popupService = popupService;
            _navigationService = navigationService;
        }

        public void Initialize()
        {
            _screen.ResetProgressButtonClicked += OnResetProgressButtonClicked;
            _screen.StartGameButtonClicked += OnStartGameButtonClicked;
            _screen.ExitGameButtonClicked += OnExitGameButtonClicked;

            CreateWallet();
            CreateProgressPresenter();

            foreach (IPresenter childPresenter in _childPresenters)
                childPresenter.Initialize();
        }

        public void Dispose()
        {
            _screen.ResetProgressButtonClicked -= OnResetProgressButtonClicked;
            _screen.StartGameButtonClicked -= OnStartGameButtonClicked;
            _screen.ExitGameButtonClicked -= OnExitGameButtonClicked;

            foreach (IPresenter childPresenter in _childPresenters)
                childPresenter.Dispose();

            _childPresenters.Clear();
        }

        private void CreateWallet()
        {
            if (_screen.WalletView == null)
                return;

            WalletPresenter walletPresenter = _projectPresentersFactory.CreateWalletPresenter(_screen.WalletView);
            _childPresenters.Add(walletPresenter);
        }

        private void CreateProgressPresenter()
        {
            if (_screen.ProgressItemlistView == null)
                return;

            ProgressPresenter progressPresenter =
                _projectPresentersFactory.CreateProgressPresenter(_screen.ProgressItemlistView);

            _childPresenters.Add(progressPresenter);
        }

        private void OnResetProgressButtonClicked()
        {
            _popupService.OpenResetProgressPopup();
        }

        private void OnStartGameButtonClicked()
        {
            _navigationService.StartGame();
        }

        private void OnExitGameButtonClicked()
        {
            _navigationService.ExitGame();
        }
    }
}
