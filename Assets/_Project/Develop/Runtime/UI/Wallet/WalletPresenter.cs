using System.Collections.Generic;
using _Project.Develop.Runtime.Meta.Features.Wallet;
using _Project.Develop.Runtime.UI.CommonViews;
using _Project.Develop.Runtime.UI.Core;
using UnityEngine;

namespace _Project.Develop.Runtime.UI.Wallet
{
    public class WalletPresenter : IPresenter
    {
        private readonly WalletService _walletService;
        private readonly ProjectPresentersFactory _projectPresentersFactory;
        private readonly ViewsFactory _viewsFactory;
        
        private readonly IconTextListView _view;
        
        private readonly List<CurrencyPresenter> _currencyPresenters = new();

        public WalletPresenter(
            WalletService walletService, 
            ProjectPresentersFactory projectPresentersFactory, 
            ViewsFactory viewsFactory, 
            IconTextListView iconTextListView)
        {
            _walletService = walletService;
            _projectPresentersFactory = projectPresentersFactory;
            _viewsFactory = viewsFactory;
            _view = iconTextListView;
        }

        public void Initialize()
        {
            IconTextView currencyView = _viewsFactory.Create<IconTextView>(ViewIDs.Currency);
                
            _view.Add(currencyView);
                
            CurrencyPresenter currencyPresenter = _projectPresentersFactory.CreateCurrencyPresenter(
                currencyView, 
                _walletService.GetCurrency(CurrencyTypes.Gold),
                CurrencyTypes.Gold);
                
            currencyPresenter.Initialize();
            _currencyPresenters.Add(currencyPresenter);
        }

        public void Dispose()
        {
            foreach (CurrencyPresenter currencyPresenter in _currencyPresenters)
            {
                _view.Remove(currencyPresenter.View);
                _viewsFactory.Release(currencyPresenter.View);
                
                currencyPresenter.Dispose();
            }
            
            _currencyPresenters.Clear();
        }
    }
}