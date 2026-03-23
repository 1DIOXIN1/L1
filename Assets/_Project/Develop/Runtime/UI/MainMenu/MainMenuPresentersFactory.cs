using _Project.Develop.Runtime.Infrastructure.DI;
using _Project.Develop.Runtime.UI.Progress;

namespace _Project.Develop.Runtime.UI.MainMenu
{
    public class MainMenuPresentersFactory
    {
        private readonly DIContainer _container;
        
        public MainMenuPresentersFactory(DIContainer container)
        {
            _container = container;
        }
        
        public MainMenuScreenPresenter CreateMainMenuScreen(MainMenuScreenView mainMenuScreenView)
        {
            ProjectPresentersFactory projectPresentersFactory = _container.Resolve<ProjectPresentersFactory>();
            MainMenuPopupService popupService = _container.Resolve<MainMenuPopupService>();
            
            return new MainMenuScreenPresenter(mainMenuScreenView, projectPresentersFactory, popupService);
        }
    }
}