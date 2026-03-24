using System.Collections.Generic;
using _Project.Develop.Runtime.UI.Core;
using _Project.Develop.Runtime.UI.Progress;
using _Project.Develop.Runtime.UI.Wallet;
using UnityEngine;

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
            _screen.ResetProgressButtonClicked += OnResetProgressButtonClicked;
            
            CreateWallet();
            CreateProgressPresenter();
            
            foreach (var childPresenter in _childPresenters)
                childPresenter.Initialize();
        }

        public void Dispose()
        {
            _screen.ResetProgressButtonClicked -= OnResetProgressButtonClicked;
            
            foreach (var childPresenter in _childPresenters)
                childPresenter.Dispose();
            
            _childPresenters.Clear();
        }
        
        private void CreateWallet()
        {
            WalletPresenter walletPresenter = _projectPresentersFactory.CreateWalletPresenter(_screen.WalletView);
            
            _childPresenters.Add(walletPresenter);
        }

        private void CreateProgressPresenter()
        {
            ProgressPresenter progressPresenter = _projectPresentersFactory.CreateProgressPresenter(_screen.ProgressItemlistView);
            
            _childPresenters.Add(progressPresenter);
        }
        
        private void OnResetProgressButtonClicked()
        {
            _popupService.OpenResetProgressPopup();
        }
    }
}