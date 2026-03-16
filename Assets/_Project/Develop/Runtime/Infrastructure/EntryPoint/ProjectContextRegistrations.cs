using System;
using System.Collections.Generic;
using _Project.Develop.Runtime.Infrastructure.DI;
using _Project.Develop.Runtime.Meta.Features.Wallet;
using _Project.Develop.Runtime.Utilities;
using _Project.Develop.Runtime.Utilities.AssetsManagement;
using _Project.Develop.Runtime.Utilities.ConfigsManagement;
using _Project.Develop.Runtime.Utilities.InputManagement;
using _Project.Develop.Runtime.Utilities.Reactive;
using _Project.Develop.Runtime.Utilities.SceneManagement;
using Object = UnityEngine.Object;

namespace _Project.Develop.Runtime.Infrastructure.EntryPoint
{
    public class ProjectContextRegistrations
    {
        public static void Process(DIContainer container)
        {
            container.RegisterAsSingle(CreateResuorcesAssetsLoader);
            container.RegisterAsSingle(CreateConfigsProviderService);
            container.RegisterAsSingle(CreateResourcesConfigsLoader);
            container.RegisterAsSingle(CreateCoroutinesPerformer);
            container.RegisterAsSingle(CreateSceneLoaderService);
            container.RegisterAsSingle(CreateSceneSwitcherService);
            container.RegisterAsSingle(CreateWalletService);
            container.RegisterAsSingle<IInputService>(CreateKeyboardInputService);
        }
        
        private static WalletService CreateWalletService(DIContainer container)
        {
            Dictionary<CurrencyTypes, ReactiveVariable<int>> currencies = new();

            foreach (CurrencyTypes currencyType in Enum.GetValues(typeof(CurrencyTypes)))
                currencies[currencyType] = new ReactiveVariable<int>();
            
            return new WalletService(currencies);
        }
        
        private static ResuorcesAssetsLoader CreateResuorcesAssetsLoader(DIContainer container) 
            => new ResuorcesAssetsLoader();
        
        private static SceneLoaderService CreateSceneLoaderService(DIContainer container)
            => new SceneLoaderService();

        private static SceneSwitcherService CreateSceneSwitcherService(DIContainer container)
            => new SceneSwitcherService(container.Resolve<SceneLoaderService>(), container);

        private static ConfigsProviderService CreateConfigsProviderService(DIContainer container)
        {
            IConfigsLoader loader = new ResourcesConfigsLoader(container.Resolve<ResuorcesAssetsLoader>()); 
            
            return new ConfigsProviderService(loader);
        }

        private static ResourcesConfigsLoader CreateResourcesConfigsLoader(DIContainer container)
        {
            ResuorcesAssetsLoader assetsLoader = container.Resolve<ResuorcesAssetsLoader>();
            
            return new ResourcesConfigsLoader(assetsLoader);
        }

        private static CoroutinesPerformer CreateCoroutinesPerformer(DIContainer container)
        {
            ResuorcesAssetsLoader assetsLoader = container.Resolve<ResuorcesAssetsLoader>();
            
            CoroutinesPerformer coroutinesPerformerPrefab = assetsLoader.Load<CoroutinesPerformer>("Utilities/CoroutinesPerformer");
            
            return Object.Instantiate(coroutinesPerformerPrefab);
        }
        
        private static KeyboardInputService CreateKeyboardInputService(DIContainer container)
            => new KeyboardInputService();
    }
}
