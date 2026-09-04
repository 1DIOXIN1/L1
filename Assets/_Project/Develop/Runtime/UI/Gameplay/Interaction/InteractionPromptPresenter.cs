using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter;
using _Project.Develop.Runtime.Gameplay.Features.Main.Interactables;
using _Project.Develop.Runtime.UI.Core;
using UnityEngine;

namespace _Project.Develop.Runtime.UI.Gameplay.Interaction
{
    public sealed class InteractionPromptPresenter : IPresenter
    {
        private readonly InteractionService _interactionService;
        private readonly InteractionPromptView _view;
        private readonly Player _player;

        private Camera _camera;
        private Interactable _focus;

        public InteractionPromptPresenter(
            InteractionService interactionService,
            InteractionPromptView view,
            Player player)
        {
            _interactionService = interactionService;
            _view = view;
            _player = player;
        }

        public InteractionPromptView View => _view;

        public void Initialize()
        {
            _interactionService.FocusChanged += OnFocusChanged;
            _view.SetVisible(false);
            OnFocusChanged(_interactionService.CurrentFocus);
        }

        public void Tick()
        {
            if (_view == null || _focus == null)
                return;

            if (_camera == null || _camera.isActiveAndEnabled == false)
                ResolveCamera();

            if (_camera == null)
                return;

            Transform anchor = _focus.PromptAnchor;
            if (anchor == null)
                return;

            _view.SetWorldPose(anchor.position, _camera.transform.rotation);
        }

        public void Dispose()
        {
            _interactionService.FocusChanged -= OnFocusChanged;
        }

        private void OnFocusChanged(Interactable focus)
        {
            _focus = focus;
            _view.SetVisible(focus != null);
            Tick();
        }

        private void ResolveCamera()
        {
            if (_player != null)
                _camera = _player.LookCamera;

            if (_camera == null || _camera.isActiveAndEnabled == false)
                _camera = Camera.main;
        }
    }
}
