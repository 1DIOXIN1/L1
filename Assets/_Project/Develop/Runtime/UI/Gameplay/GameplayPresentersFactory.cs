using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Core;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Detection;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter;
using _Project.Develop.Runtime.Gameplay.Features.Main.Interactables;
using _Project.Develop.Runtime.Gameplay.Infrastructure.Mission;
using _Project.Develop.Runtime.Infrastructure.DI;
using _Project.Develop.Runtime.Meta.Features.Missions;
using _Project.Develop.Runtime.Meta.Features.Player;
using _Project.Develop.Runtime.Meta.Features.Settings;
using _Project.Develop.Runtime.UI.Core;
using _Project.Develop.Runtime.UI.Gameplay.Arsenal;
using _Project.Develop.Runtime.UI.Gameplay.Detection;
using _Project.Develop.Runtime.UI.Gameplay.Interaction;
using _Project.Develop.Runtime.UI.Gameplay.Phone;
using _Project.Develop.Runtime.Utilities.ConfigsManagement;
using _Project.Develop.Runtime.Utilities.CoroutinesManagement;
using _Project.Develop.Runtime.Utilities.DataManagement.DataProviders;
using _Project.Develop.Runtime.Utilities.InputManagement;
using _Project.Develop.Runtime.Utilities.SceneManagement;
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
            return new PhonePresenter(
                phoneView,
                _container.Resolve<IInputService>(),
                CreateSettingsPresenter(phoneView.SettingsPanelView),
                CreateMapPresenter(phoneView.MapPanelView),
                CreateQuestsPresenter(phoneView.QuestsPanelView));
        }

        public SettingsPresenter CreateSettingsPresenter(SettingsPanelView settingsPanelView)
        {
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

        public MapPresenter CreateMapPresenter(MapPanelView mapPanelView)
        {
            return new MapPresenter(
                mapPanelView,
                _container.Resolve<MissionService>(),
                _container.Resolve<LocationTravelService>());
        }

        public QuestsPresenter CreateQuestsPresenter(QuestsPanelView questsPanelView)
        {
            return new QuestsPresenter(
                questsPanelView,
                _container.Resolve<MissionQuestTracker>());
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

        public InteractionHintPresenter CreateInteractionHintPresenter(
            PlayerCamera playerCamera,
            InteractionService interactionService)
        {
            InteractionHintView view =
                _container.Resolve<ViewsFactory>().Create<InteractionHintView>(ViewIDs.InteractionHint);

            return new InteractionHintPresenter(
                interactionService,
                view,
                playerCamera);
        }

        public WeaponArsenalPresenter CreateWeaponArsenalPresenter(GameplayScreenView screenView)
        {
            WeaponArsenalView arsenalView =
                _container.Resolve<ViewsFactory>().Create<WeaponArsenalView>(ViewIDs.WeaponArsenal, screenView.transform);

            return new WeaponArsenalPresenter(
                arsenalView,
                _container.Resolve<IInputService>(),
                _container.Resolve<PlayerStateService>());
        }
    }
}
