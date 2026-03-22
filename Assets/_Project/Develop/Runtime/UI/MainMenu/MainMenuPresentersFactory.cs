using _Project.Develop.Runtime.Infrastructure.DI;

namespace _Project.Develop.Runtime.UI.MainMenu
{
    public class MainMenuPresentersFactory
    {
        private readonly DIContainer _container;
        
        public MainMenuPresentersFactory(DIContainer container)
        {
            _container = container;
        }
        
        public MainMenuScreenPresenter CreateMainMenuScreen(MainMenuScreenView view)
        {
            ProjectPresentersFactory projectPresentersFactory = _container.Resolve<ProjectPresentersFactory>();
            MainMenuPopupService popupService = _container.Resolve<MainMenuPopupService>();
            
            return new MainMenuScreenPresenter(view, projectPresentersFactory, popupService);
        }
    }
}