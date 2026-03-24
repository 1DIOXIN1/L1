using System;
using _Project.Develop.Runtime.Gameplay.Main;
using _Project.Develop.Runtime.Infrastructure.DI;
using _Project.Develop.Runtime.Meta.Features.Progress;
using _Project.Develop.Runtime.Meta.Features.Wallet;
using _Project.Develop.Runtime.UI.Core;
using _Project.Develop.Runtime.UI.Gameplay;
using _Project.Develop.Runtime.Utilities;
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
            container.RegisterAsSingle(CreateGameplaySequenceGeneratorService);
            container.RegisterAsSingle(CreateGameMode);
            container.RegisterAsSingle(CreateGameplayCycle).NonLazy();
            container.RegisterAsSingle(CreateGameplayPresentersFactory).NonLazy();
            container.RegisterAsSingle(CreateSequenceChecker);
            container.RegisterAsSingle(CreateGameplayScreen).NonLazy();
        }

        private static GameplaySequenceGeneratorService CreateGameplaySequenceGeneratorService(DIContainer container) 
            =>new GameplaySequenceGeneratorService();
        
        private static SequenceChecker CreateSequenceChecker(DIContainer container)
            => new SequenceChecker();

        private static GameMode CreateGameMode(DIContainer container)
        {
            SequenceChecker checker = container.Resolve<SequenceChecker>();
            WalletService walletService = container.Resolve<WalletService>();
            ConfigsProviderService configsProviderService = container.Resolve<ConfigsProviderService>();
            
            return new GameMode(checker, walletService, configsProviderService);
        }
        
        private static GameplayCycle CreateGameplayCycle(DIContainer container)
        {
            GameMode gameMode = container.Resolve<GameMode>();
            IInputService input = container.Resolve<IInputService>();
            SceneSwitcherService sceneSwitcher = container.Resolve<SceneSwitcherService>();
            CoroutinesPerformer coroutinesPerformer = container.Resolve<CoroutinesPerformer>();
            GameplayDataProvider gameplayDataProvider = container.Resolve<GameplayDataProvider>();
            ProgressService progressService = container.Resolve<ProgressService>();
            
            return new GameplayCycle(gameMode, input, sceneSwitcher, coroutinesPerformer, gameplayDataProvider, progressService);
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
    }
}