using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Core;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Detection;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter;
using _Project.Develop.Runtime.Gameplay.Features.Main.Interactables;
using _Project.Develop.Runtime.Infrastructure.DI;
using _Project.Develop.Runtime.Meta.Features.Player;
using _Project.Develop.Runtime.UI.Core;
using _Project.Develop.Runtime.UI.Gameplay.Detection;
using _Project.Develop.Runtime.UI.Gameplay.Interaction;
using _Project.Develop.Runtime.Utilities.ConfigsManagement;
using UnityEngine;

namespace _Project.Develop.Runtime.UI.Gameplay
{
    public class GameplayPresentersFactory
    {
        private readonly DIContainer _container;

        public GameplayPresentersFactory(DIContainer container)
        {
            _container = container;
        }

        public GameplayScreenPresenter CreateGameplayScreenPresenter(GameplayScreenView gameplayScreenView)
        {
            return new GameplayScreenPresenter(gameplayScreenView, this);
        }

        public PlayerVitalsPresenter CreatePlayerVitalsPresenter(GameplayScreenView view)
        {
            return new PlayerVitalsPresenter(
                view,
                _container.Resolve<PlayerStateService>(),
                _container.Resolve<ConfigsProviderService>());
        }

        public WeaponHudPresenter CreateWeaponHudPresenter(GameplayScreenView view)
        {
            return new WeaponHudPresenter(
                view,
                _container.Resolve<PlayerStateService>(),
                _container.Resolve<ConfigsProviderService>());
        }

        public EnemyDetectionIconsPresenter CreateEnemyDetectionIconsPresenter()
        {
            return new EnemyDetectionIconsPresenter(
                _container.Resolve<EnemyAIService>(),
                _container.Resolve<ViewsFactory>(),
                this);
        }

        public EnemyDetectionIconPresenter CreateEnemyDetectionIconPresenter(
            EnemyAwareness awareness,
            EnemyDetectionIconView view,
            Transform followTarget,
            Transform player,
            float heightOffset)
        {
            return new EnemyDetectionIconPresenter(
                awareness,
                view,
                followTarget,
                player,
                heightOffset);
        }

        public InteractionPromptPresenter CreateInteractionPromptPresenter(Player player)
        {
            InteractionPromptView view =
                _container.Resolve<ViewsFactory>().Create<InteractionPromptView>(ViewIDs.InteractionPrompt);

            return new InteractionPromptPresenter(
                _container.Resolve<InteractionService>(),
                view,
                player);
        }
    }
}
