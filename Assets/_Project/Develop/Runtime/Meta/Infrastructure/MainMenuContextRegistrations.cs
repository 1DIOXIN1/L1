using _Project.Develop.Runtime.Infrastructure.DI;
using _Project.Develop.Runtime.UI;
using _Project.Develop.Runtime.UI.Core;
using _Project.Develop.Runtime.UI.MainMenu;
using _Project.Develop.Runtime.Utilities.AssetsManagement;
using UnityEngine;

namespace _Project.Develop.Runtime.Meta.Infrastructure
{
    public class MainMenuContextRegistrations
    {
        public static void Process(DIContainer container)
        {
            Debug.Log("Процесс регистрации сервисов на сцене меню");
            
            container.RegisterAsSingle(CreateMainMenuUIRoot).NonLazy();
            container.RegisterAsSingle(CreateMainMenuScreen).NonLazy();
            container.RegisterAsSingle(CreateMainMenuPresentersFactory);
            container.RegisterAsSingle(CreateMainMenuPopupService);
        }

        private static MainMenuPopupService CreateMainMenuPopupService(DIContainer container)
        {
            ViewsFactory viewsFactory = container.Resolve<ViewsFactory>();
            ProjectPresentersFactory projectPresentersFactory = container.Resolve<ProjectPresentersFactory>();
            MainMenuUIRoot uiRoot = container.Resolve<MainMenuUIRoot>();
            
            return new MainMenuPopupService(viewsFactory, projectPresentersFactory, uiRoot);
        }
        
        private static MainMenuUIRoot CreateMainMenuUIRoot(DIContainer container)
        {
            ResourcesAssetsLoader assetsLoader = container.Resolve<ResourcesAssetsLoader>();
            MainMenuUIRoot mainMenuUIRoot = assetsLoader.Load<MainMenuUIRoot>("UI/MainMenu/MainMenuUIRoot");
            return Object.Instantiate(mainMenuUIRoot);
        }

        public static MainMenuPresentersFactory CreateMainMenuPresentersFactory(DIContainer container)
        {
            return new MainMenuPresentersFactory(container);
        }

        public static MainMenuScreenPresenter CreateMainMenuScreen(DIContainer container)
        {
            MainMenuUIRoot uiRoot = container.Resolve<MainMenuUIRoot>();
            MainMenuScreenView mainMenuScreenView = container.Resolve<ViewsFactory>().Create<MainMenuScreenView>(ViewIDs.MainMenuScreen,  uiRoot.HUDLayer);
            MainMenuScreenPresenter presenter = container.Resolve<MainMenuPresentersFactory>().CreateMainMenuScreen(mainMenuScreenView);
            
            return presenter;
        }
    }
}