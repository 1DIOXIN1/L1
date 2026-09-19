using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Core;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Detection;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter;
using _Project.Develop.Runtime.Gameplay.Features.Main.Interactables;
using _Project.Develop.Runtime.Infrastructure.DI;
using _Project.Develop.Runtime.Meta.Features.Player;
using _Project.Develop.Runtime.UI.Core;
using _Project.Develop.Runtime.UI.Gameplay.Detection;
using _Project.Develop.Runtime.UI.Gameplay.Interaction;
using _Project.Develop.Runtime.UI.Gameplay.Phone;
using _Project.Develop.Runtime.Utilities.ConfigsManagement;
using _Project.Develop.Runtime.Utilities.CoroutinesManagement;
using _Project.Develop.Runtime.Utilities.DataManagement.DataProviders;
using _Project.Develop.Runtime.Utilities.InputManagement;
using _Project.Develop.Runtime.Utilities.SceneManagement;
using _Project.Develop.Runtime.Meta.Features.Settings;
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

        public PhonePresenter CreatePhonePresenter(PhoneView phoneView)
        {
            if (phoneView == null)
                return null;

            SettingsPresenter settingsPresenter = CreateSettingsPresenter(phoneView.SettingsPanelView);

            return new PhonePresenter(
                phoneView,
                _container.Resolve<IInputService>(),
                settingsPresenter);
        }

        public SettingsPresenter CreateSettingsPresenter(SettingsPanelView settingsPanelView)
        {
            if (settingsPanelView == null)
                return null;

            return new SettingsPresenter(
                settingsPanelView,
                _container.Resolve<SettingsService>(),
                _container.Resolve<SettingsDataProvider>(),
                _container.Resolve<PlayerDataProvider>(),
                _container.Resolve<GameplayDataProvider>(),
                _container.Resolve<SceneSwitcherService>(),
                _container.Resolve<CoroutinesPerformer>(),
                _container.Resolve<IInputService>());
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
            float heightOffset,
            Camera camera)
        {
            return new EnemyDetectionIconPresenter(
                awareness,
                view,
                followTarget,
                heightOffset,
                camera);
        }

        public InteractionHintPresenter CreateInteractionHintPresenter(PlayerCamera playerCamera)
        {
            InteractionHintView view =
                _container.Resolve<ViewsFactory>().Create<InteractionHintView>(ViewIDs.InteractionHint);

            return new InteractionHintPresenter(
                _container.Resolve<InteractionService>(),
                view,
                playerCamera);
        }
    }
}
