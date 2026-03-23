using System;
using _Project.Develop.Runtime.Configs.Meta.Wallet;
using _Project.Develop.Runtime.Meta.Features.Wallet;
using _Project.Develop.Runtime.UI.CommonViews;
using _Project.Develop.Runtime.UI.Core;
using _Project.Develop.Runtime.Utilities.Reactive;

namespace _Project.Develop.Runtime.UI.Wallet
{
    public class CurrencyPresenter : IPresenter
    {
        private readonly IReadOnlyVariable<int> _currency;
        private readonly CurrencyTypes _currencyType;
        private readonly CurrencyIconsConfig _config;

        private IconTextView _view;
        
        private IDisposable _disposable;

        public CurrencyPresenter(IReadOnlyVariable<int> currency, 
            CurrencyTypes currencyType, 
            CurrencyIconsConfig config, 
            IconTextView view)
        {
            _currency = currency;
            _currencyType = currencyType;
            _config = config;
            _view = view;
        }
        
        public IconTextView View => _view;
        
        public void Initialize()
        {
            UpdateText(_currency.Value);
            _view.SetIcon(_config.GetSpriteFor(_currencyType));

            _disposable = _currency.Subscribe(OnCurrencyChanged);
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
        
        private void UpdateText(int newValue) => _view.SetText(newValue.ToString());
        
        private void OnCurrencyChanged(int arg1, int newValue) => UpdateText(newValue);

    }
}