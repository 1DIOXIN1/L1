using _Project.Develop.Runtime.Gameplay.Factories;
using _Project.Develop.Runtime.Gameplay.Main;
using _Project.Develop.Runtime.Infrastructure.DI;
using _Project.Develop.Runtime.Meta.Features.Progress;
using _Project.Develop.Runtime.Meta.Features.Wallet;
using _Project.Develop.Runtime.Utilities;
using _Project.Develop.Runtime.Utilities.ConfigsManagement;
using _Project.Develop.Runtime.Utilities.DataManagement.DataProviders;
using _Project.Develop.Runtime.Utilities.InputManagement;
using _Project.Develop.Runtime.Utilities.SceneManagement;

namespace _Project.Develop.Runtime.Gameplay.Infrastructure
{
    public class GameplayContextRegistrations
    {
        public static void Process(DIContainer container)
        {
            container.RegisterAsSingle(CreateGameplaySequenceGeneratorService);
            container.RegisterAsSingle(CreateGameMode);
            container.RegisterAsSingle(CreateGameplayCycle).NonLazy();
            container.RegisterAsSingle(CreateCorrectSequenceChecker);
        }

        private static GameplaySequenceGeneratorService CreateGameplaySequenceGeneratorService(DIContainer container) 
            =>new GameplaySequenceGeneratorService();
        
        private static CorrectSequenceChecker CreateCorrectSequenceChecker(DIContainer container)
            => new CorrectSequenceChecker();

        public static GameMode CreateGameMode(DIContainer container)
        {
            CorrectSequenceChecker checker = container.Resolve<CorrectSequenceChecker>();
            WalletService walletService = container.Resolve<WalletService>();
            ConfigsProviderService configsProviderService = container.Resolve<ConfigsProviderService>();
            
            return new GameMode(checker, walletService, configsProviderService);
        }
        
        public static GameplayCycle CreateGameplayCycle(DIContainer container)
        {
            GameMode gameMode = container.Resolve<GameMode>();
            IInputService input = container.Resolve<IInputService>();
            SceneSwitcherService sceneSwitcher = container.Resolve<SceneSwitcherService>();
            CoroutinesPerformer coroutinesPerformer = container.Resolve<CoroutinesPerformer>();
            GameplayDataProvider gameplayDataProvider = container.Resolve<GameplayDataProvider>();
            ProgressService progressService = container.Resolve<ProgressService>();
            
            return new GameplayCycle(gameMode, input, sceneSwitcher, coroutinesPerformer, gameplayDataProvider, progressService);
        }
    }
}