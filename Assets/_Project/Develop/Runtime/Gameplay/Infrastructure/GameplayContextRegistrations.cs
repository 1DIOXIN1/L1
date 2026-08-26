using _Project.Develop.Runtime.Utilities.CoroutinesManagement;
using _Project.Develop.Runtime.Configs.Meta.Enemy;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.AttackBehaviors;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Core;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Spawning;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter;
using _Project.Develop.Runtime.Gameplay.Features.Main.Gadget;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.FireModes;
using _Project.Develop.Runtime.Infrastructure.DI;
using _Project.Develop.Runtime.Meta.Features.Player;
using _Project.Develop.Runtime.Meta.Features.Progress;
using _Project.Develop.Runtime.UI.Core;
using _Project.Develop.Runtime.UI.Gameplay;
using _Project.Develop.Runtime.Utilities.AssetsManagement;
using _Project.Develop.Runtime.Utilities.ConfigsManagement;
using _Project.Develop.Runtime.Utilities.DataManagement.DataProviders;
using _Project.Develop.Runtime.Utilities.InputManagement;
using _Project.Develop.Runtime.Utilities.SceneManagement;
using Object = UnityEngine.Object;

namespace _Project.Develop.Runtime.Gameplay.Infrastructure
{
    public class GameplayContextRegistrations
    {
        public static void Process(DIContainer container)
        {
            container.RegisterAsSingle(CreateGameplayUIRoot).NonLazy();
            container.RegisterAsSingle(CreateGameMode);
            container.RegisterAsSingle(CreateGameplayCycle).NonLazy();
            container.RegisterAsSingle(CreateGameplayPresentersFactory).NonLazy();
            container.RegisterAsSingle(CreateGameplayScreen).NonLazy();
            container.RegisterAsSingle(CreateCharactersFactory);
            container.RegisterAsSingle(CreateEnemyAIService);
            container.RegisterAsSingle(CreateEnemyAttackBehaviorFactory);
            container.RegisterAsSingle(CreateEnemySpawnService);
            container.RegisterAsSingle(CreateFireModeRegistry);
            container.RegisterAsSingle(CreateWeaponFactory);
            container.RegisterAsSingle(CreateGadgetFactory);
            container.RegisterAsSingle(CreatePlayerWeaponInventory);
            container.RegisterAsSingle(CreatePlayerGadgetInventory);
        }

        private static GameMode CreateGameMode(DIContainer container)
        {
            EnemyAIService enemyAIService = container.Resolve<EnemyAIService>();
            return new GameMode(enemyAIService);
        }
        
        private static GameplayCycle CreateGameplayCycle(DIContainer container)
        {
            GameMode gameMode = container.Resolve<GameMode>();
            IInputService input = container.Resolve<IInputService>();
            SceneSwitcherService sceneSwitcher = container.Resolve<SceneSwitcherService>();
            CoroutinesPerformer coroutinesPerformer = container.Resolve<CoroutinesPerformer>();
            GameplayDataProvider gameplayDataProvider = container.Resolve<GameplayDataProvider>();
            PlayerDataProvider playerDataProvider = container.Resolve<PlayerDataProvider>();
            PlayerStateService playerStateService = container.Resolve<PlayerStateService>();
            ProgressService progressService = container.Resolve<ProgressService>();

            return new GameplayCycle(
                gameMode,
                input,
                sceneSwitcher,
                coroutinesPerformer,
                gameplayDataProvider,
                playerDataProvider,
                playerStateService,
                progressService);
        }

        private static GameplayUIRoot CreateGameplayUIRoot(DIContainer container)
        {
            ResourcesAssetsLoader assetsLoader = container.Resolve<ResourcesAssetsLoader>();
            GameplayUIRoot gameplayUIRoot = assetsLoader.Load<GameplayUIRoot>("UI/Gameplay/GameplayUIRoot");
            
            return Object.Instantiate(gameplayUIRoot);
        }

        private static GameplayScreenPresenter CreateGameplayScreen(DIContainer container)
        {
            GameplayUIRoot uiRoot = container.Resolve<GameplayUIRoot>();
            GameplayScreenView screenView = container.Resolve<ViewsFactory>().Create<GameplayScreenView>(ViewIDs.GameplayScreen, uiRoot.HUDLayer);
            GameplayPresentersFactory presentersFactory = container.Resolve<GameplayPresentersFactory>();
            
            return new GameplayScreenPresenter(screenView, presentersFactory);
        }
    
        private static GameplayPresentersFactory CreateGameplayPresentersFactory(DIContainer container)
            => new GameplayPresentersFactory(container);
        
        private static CharactersFactory CreateCharactersFactory(DIContainer container)
            => new CharactersFactory(container);

        private static EnemyAIService CreateEnemyAIService(DIContainer container)
        {
            ConfigsProviderService configsProviderService = container.Resolve<ConfigsProviderService>();
            return new EnemyAIService(configsProviderService.GetConfig<EnemyConfig>());
        }

        private static EnemyAttackBehaviorFactory CreateEnemyAttackBehaviorFactory(DIContainer container)
            => new EnemyAttackBehaviorFactory();

        private static EnemySpawnService CreateEnemySpawnService(DIContainer container)
            => new EnemySpawnService(container.Resolve<CharactersFactory>());

        private static FireModeRegistry CreateFireModeRegistry(DIContainer container)
            => new FireModeRegistry();

        private static WeaponFactory CreateWeaponFactory(DIContainer container) 
            => new WeaponFactory(container);

        private static GadgetFactory CreateGadgetFactory(DIContainer container)
            => new GadgetFactory(container);

        private static PlayerWeaponInventory CreatePlayerWeaponInventory(DIContainer container)
        {
            ConfigsProviderService configsProviderService = container.Resolve<ConfigsProviderService>();
            WeaponFactory factory = container.Resolve<WeaponFactory>();
            PlayerStateService playerStateService = container.Resolve<PlayerStateService>();

            return new PlayerWeaponInventory(configsProviderService, factory, playerStateService);
        }

        private static PlayerGadgetInventory CreatePlayerGadgetInventory(DIContainer container)
        {
            ConfigsProviderService configsProviderService = container.Resolve<ConfigsProviderService>();
            GadgetFactory factory = container.Resolve<GadgetFactory>();

            return new PlayerGadgetInventory(configsProviderService, factory);
        }
    }
}
