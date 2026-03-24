using System;
using _Project.Develop.Runtime.Configs.Meta.Progress;
using _Project.Develop.Runtime.Meta.Features.Progress;
using _Project.Develop.Runtime.UI.CommonViews;
using _Project.Develop.Runtime.UI.Core;
using _Project.Develop.Runtime.Utilities.Reactive;

namespace _Project.Develop.Runtime.UI.Progress
{
    public class ProgressItemPresenter : IPresenter
    {
        private IReadOnlyVariable<int> _item;
        private ProgressConfig _config;
        private ProgressTypes _type;
        private NameValueTextView _view;
        
        private IDisposable _disposable;

        public ProgressItemPresenter(
            IReadOnlyVariable<int> item,
            ProgressConfig config, 
            ProgressTypes type, 
            NameValueTextView view)
        {
            _item = item;
            _config = config;
            _type = type;
            _view = view;
        }
        
        public NameValueTextView View => _view;

        public void Initialize()
        {
            _view.SetName(_config.GetNameFor(_type));
            UpdateValueText(_item.Value);

            _disposable = _item.Subscribe(OnCurrencyChanged);
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
        
        private void UpdateValueText(int newValue) => _view.SetValueText(newValue);
        
        private void OnCurrencyChanged(int arg1, int newValue) => UpdateValueText(newValue);
    }
}