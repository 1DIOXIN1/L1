using System.Collections.Generic;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter;
using _Project.Develop.Runtime.UI.Core;
using _Project.Develop.Runtime.UI.Gameplay.Detection;
using _Project.Develop.Runtime.UI.Gameplay.Interaction;
using _Project.Develop.Runtime.Utilities.InputManagement;

namespace _Project.Develop.Runtime.UI.Gameplay
{
    public class GameplayScreenPresenter : IPresenter
    {
        private readonly GameplayScreenView _view;
        private readonly GameplayPresentersFactory _presentersFactory;
        private readonly IInputService _input;
        private readonly List<IPresenter> _childPresenters = new();

        private PlayerVitalsPresenter _vitalsPresenter;
        private WeaponHudPresenter _weaponHudPresenter;
        private EnemyDetectionIconsPresenter _detectionIconsPresenter;
        private InteractionHintPresenter _interactionHintPresenter;

        public GameplayScreenPresenter(
            GameplayScreenView view,
            GameplayPresentersFactory presentersFactory,
            IInputService input)
        {
            _view = view;
            _presentersFactory = presentersFactory;
            _input = input;
        }

        public void Initialize()
        {
            _vitalsPresenter = _presentersFactory.CreatePlayerVitalsPresenter(_view);
            _weaponHudPresenter = _presentersFactory.CreateWeaponHudPresenter(_view);
            _detectionIconsPresenter = _presentersFactory.CreateEnemyDetectionIconsPresenter();

            _childPresenters.Add(_vitalsPresenter);
            _childPresenters.Add(_weaponHudPresenter);
            _childPresenters.Add(_detectionIconsPresenter);

            foreach (IPresenter childPresenter in _childPresenters)
                childPresenter.Initialize();

            _input.PhonePressed += OnPhonePressed;
        }

        public void AttachPlayer(Player player, PlayerCamera playerCamera)
        {
            _vitalsPresenter?.AttachPlayer(player);
            _weaponHudPresenter?.AttachPlayer(player);
            _detectionIconsPresenter?.BindCamera(playerCamera != null ? playerCamera.LookCamera : null);

            if (_interactionHintPresenter != null || player == null || playerCamera == null)
                return;

            _interactionHintPresenter = _presentersFactory.CreateInteractionHintPresenter(playerCamera);
            _interactionHintPresenter.Initialize();
            _childPresenters.Add(_interactionHintPresenter);
        }

        public void Tick()
        {
            _vitalsPresenter?.Tick();
            _detectionIconsPresenter?.Tick();
        }

        public void TickInteraction()
        {
            _interactionHintPresenter?.Tick();
        }

        public void SetDetectionIconsVisible(bool visible)
        {
            _detectionIconsPresenter?.SetVisible(visible);
        }

        public void Dispose()
        {
            _input.PhonePressed -= OnPhonePressed;

            foreach (IPresenter childPresenter in _childPresenters)
                childPresenter.Dispose();

            _childPresenters.Clear();
            _vitalsPresenter = null;
            _weaponHudPresenter = null;
            _detectionIconsPresenter = null;
            _interactionHintPresenter = null;
        }

        private void OnPhonePressed()
        {
            _view.TogglePhoneVisible();
        }
    }
}
