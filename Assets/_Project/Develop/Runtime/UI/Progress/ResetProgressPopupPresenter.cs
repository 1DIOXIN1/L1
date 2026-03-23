using _Project.Develop.Runtime.Meta.Features.Progress;
using _Project.Develop.Runtime.UI.Core;

namespace _Project.Develop.Runtime.UI.Progress
{
    public class ResetProgressPopupPresenter : PopupPresenterBase
    {
        private readonly ResetProgressPopupView _view;
        private readonly ProgressService _progressService;

        public ResetProgressPopupPresenter(ResetProgressPopupView view, ProgressService progressService)
        {
            _view = view;
            _progressService = progressService;
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
            if(_progressService.TryReset())
                OnCloseRequest();
        }

        private void OnCancelButtonClicked() => OnCloseRequest();
    }
}