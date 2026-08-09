using _Project.Develop.Runtime.Gameplay.Features.Main;
using _Project.Develop.Runtime.Infrastructure.DI;

namespace _Project.Develop.Runtime.UI.Gameplay
{
    public class GameplayPresentersFactory
    {
        private DIContainer _container;

        public GameplayPresentersFactory(DIContainer container)
        {
            _container = container;
        }

        public GameplayScreenPresenter CreateGameplayScreenPresenter(GameplayScreenView gameplayScreenView)
        {
            return new GameplayScreenPresenter(gameplayScreenView, this);
        }
    }
}