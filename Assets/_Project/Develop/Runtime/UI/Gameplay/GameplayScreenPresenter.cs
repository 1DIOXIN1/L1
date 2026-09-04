using System.Collections.Generic;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter;
using _Project.Develop.Runtime.UI.Core;
using _Project.Develop.Runtime.UI.Gameplay.Detection;
using _Project.Develop.Runtime.UI.Gameplay.Interaction;

namespace _Project.Develop.Runtime.UI.Gameplay
{
    public class GameplayScreenPresenter : IPresenter
    {
        private readonly GameplayScreenView _view;
        private readonly GameplayPresentersFactory _presentersFactory;
        private readonly List<IPresenter> _childPresenters = new();

        private PlayerVitalsPresenter _vitalsPresenter;
        private WeaponHudPresenter _weaponHudPresenter;
        private EnemyDetectionIconsPresenter _detectionIconsPresenter;
        private InteractionPromptPresenter _interactionPromptPresenter;

        public GameplayScreenPresenter(GameplayScreenView view, GameplayPresentersFactory presentersFactory)
        {
            _view = view;
            _presentersFactory = presentersFactory;
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
        }

        public void AttachPlayer(Player player)
        {
            _vitalsPresenter?.AttachPlayer(player);
            _weaponHudPresenter?.AttachPlayer(player);

            if (_interactionPromptPresenter != null || player == null)
                return;

            _interactionPromptPresenter = _presentersFactory.CreateInteractionPromptPresenter(player);
            _interactionPromptPresenter.Initialize();
            _childPresenters.Add(_interactionPromptPresenter);
        }

        public void Tick()
        {
            _vitalsPresenter?.Tick();
            _detectionIconsPresenter?.Tick();
        }

        public void TickInteraction()
        {
            _interactionPromptPresenter?.Tick();
        }

        public void Dispose()
        {
            foreach (IPresenter childPresenter in _childPresenters)
                childPresenter.Dispose();

            _childPresenters.Clear();
            _vitalsPresenter = null;
            _weaponHudPresenter = null;
            _detectionIconsPresenter = null;
            _interactionPromptPresenter = null;
        }
    }
}
