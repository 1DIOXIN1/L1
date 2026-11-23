using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter;
using _Project.Develop.Runtime.Gameplay.Features.Main.Interactables;
using _Project.Develop.Runtime.UI.Core;
using UnityEngine;

namespace _Project.Develop.Runtime.UI.Gameplay.Interaction
{
    public sealed class InteractionHintPresenter : IPresenter
    {
        private readonly InteractionService _interactionService;
        private readonly InteractionHintView _view;
        private readonly InteractionHintView _workbenchHint;
        private readonly PlayerCamera _playerCamera;

        private IInteractable _focus;

        private bool UsesWorkbenchHint =>
            _focus is WorkbenchInteractable workbench && workbench.MonitorView != null;

        public InteractionHintPresenter(
            InteractionService interactionService,
            InteractionHintView view,
            InteractionHintView workbenchHint,
            PlayerCamera playerCamera)
        {
            _interactionService = interactionService;
            _view = view;
            _workbenchHint = workbenchHint;
            _playerCamera = playerCamera;
        }

        public InteractionHintView View => _view;

        public void Initialize()
        {
            _interactionService.FocusChanged += OnFocusChanged;
            _view.SetCamera(_playerCamera != null ? _playerCamera.LookCamera : null);
            OnFocusChanged(_interactionService.CurrentFocus);
        }

        public void Tick()
        {
            if (_view == null || _focus == null || UsesWorkbenchHint || _playerCamera == null)
                return;

            Camera camera = _playerCamera.LookCamera;
            if (camera == null)
                return;

            _view.SetWorldPose(_focus.HintAnchor.position, camera.transform.rotation);
        }

        public void Dispose()
        {
            _interactionService.FocusChanged -= OnFocusChanged;
            if (_view != null)
                Object.Destroy(_view.gameObject);
            if (_workbenchHint != null)
                Object.Destroy(_workbenchHint.gameObject);
        }

        private void OnFocusChanged(IInteractable focus)
        {
            _focus = focus;
            _view.SetVisible(focus != null && !UsesWorkbenchHint);
            _workbenchHint.SetVisible(UsesWorkbenchHint);
            Tick();
        }
    }
}
