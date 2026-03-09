using _Project.Develop.Runtime.Gameplay.Factories;
using _Project.Develop.Runtime.Gameplay.Main;
using _Project.Develop.Runtime.Infrastructure.DI;
using _Project.Develop.Runtime.Utilities;
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
            container.RegisterAsSingle(CreateGameplayCycle);
            container.RegisterAsSingle(CreateCorrectSequenceChecker);
        }

        private static GameplaySequenceGeneratorService CreateGameplaySequenceGeneratorService(DIContainer container) 
            =>new GameplaySequenceGeneratorService();
        
        private static CorrectSequenceChecker CreateCorrectSequenceChecker(DIContainer container)
            => new CorrectSequenceChecker();

        public static GameMode CreateGameMode(DIContainer container)
        {
            CorrectSequenceChecker checker = container.Resolve<CorrectSequenceChecker>();
            
            return new GameMode(checker);
        }
        
        public static GameplayCycle CreateGameplayCycle(DIContainer container)
        {
            GameMode gameMode = container.Resolve<GameMode>();
            IInputService input = container.Resolve<IInputService>();
            SceneSwitcherService sceneSwitcher = container.Resolve<SceneSwitcherService>();
            CoroutinesPerformer coroutinesPerformer = container.Resolve<CoroutinesPerformer>();
            
            return new GameplayCycle(gameMode, input, sceneSwitcher, coroutinesPerformer);
        }
    }
}