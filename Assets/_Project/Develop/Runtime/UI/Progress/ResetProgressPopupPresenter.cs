using _Project.Develop.Runtime.Meta.Features.Progress;
using _Project.Develop.Runtime.UI.Core;

namespace _Project.Develop.Runtime.UI.Progress
{
    public class ResetProgressPopupPresenter : PopupPresenterBase
    {
        private readonly ResetProgressPopupView _view;
        private readonly ResetProgressService _resetProgressService;

        public ResetProgressPopupPresenter(ResetProgressPopupView view, ResetProgressService progressService)
        {
            _view = view;
            _resetProgressService = progressService;
        }

        protected override PopupViewBase PopupView => _view;

        public override void Initialize()
        {
            base.Initialize();
            
            _view.AgreedButtonClicked += OnAgreedButtonClicked;
            _view.CancelButtonClicked += OnCancelButtonClicked;
        }

        public override void Dispose()
        {
            base.Dispose();
            
            _view.AgreedButtonClicked -= OnAgreedButtonClicked;
            _view.CancelButtonClicked -= OnCancelButtonClicked;
        }

        private void OnAgreedButtonClicked()
        {
            if(_resetProgressService.TryReset())
                OnCloseRequest();
        }

        private void OnCancelButtonClicked() => OnCloseRequest();
    }
}