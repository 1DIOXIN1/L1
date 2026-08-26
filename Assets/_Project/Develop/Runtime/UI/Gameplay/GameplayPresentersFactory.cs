using _Project.Develop.Runtime.Infrastructure.DI;
using _Project.Develop.Runtime.Meta.Features.Player;
using _Project.Develop.Runtime.Utilities.ConfigsManagement;

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
    }
}
