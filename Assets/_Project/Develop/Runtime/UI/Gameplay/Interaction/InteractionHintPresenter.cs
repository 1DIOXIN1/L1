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
        private readonly PlayerCamera _playerCamera;

        private IInteractable _focus;

        public InteractionHintPresenter(
            InteractionService interactionService,
            InteractionHintView view,
            PlayerCamera playerCamera)
        {
            _interactionService = interactionService;
            _view = view;
            _playerCamera = playerCamera;
        }

        public InteractionHintView View => _view;

        public void Initialize()
        {
            _interactionService.FocusChanged += OnFocusChanged;
            _view.SetVisible(false);
            _view.SetCamera(_playerCamera != null ? _playerCamera.LookCamera : null);
            OnFocusChanged(_interactionService.CurrentFocus);
        }

        public void Tick()
        {
            if (_view == null || _focus == null || _playerCamera == null)
                return;

            Camera camera = _playerCamera.LookCamera;
            if (camera == null)
                return;

            _view.SetWorldPose(_focus.HintAnchor.position, camera.transform.rotation);
        }

        public void Dispose()
        {
            _interactionService.FocusChanged -= OnFocusChanged;
        }

        private void OnFocusChanged(IInteractable focus)
        {
            _focus = focus;
            _view.SetVisible(focus != null);
            Tick();
        }
    }
}
