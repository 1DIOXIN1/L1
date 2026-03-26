using System;
using System.Collections.Generic;
using _Project.Develop.Runtime.Infrastructure.DI;
using _Project.Develop.Runtime.Meta.Features.Progress;
using _Project.Develop.Runtime.Meta.Features.Wallet;
using _Project.Develop.Runtime.UI;
using _Project.Develop.Runtime.UI.Core;
using _Project.Develop.Runtime.Utilities;
using _Project.Develop.Runtime.Utilities.AssetsManagement;
using _Project.Develop.Runtime.Utilities.ConfigsManagement;
using _Project.Develop.Runtime.Utilities.DataManagement;
using _Project.Develop.Runtime.Utilities.DataManagement.DataProviders;
using _Project.Develop.Runtime.Utilities.DataManagement.DataRepository;
using _Project.Develop.Runtime.Utilities.DataManagement.KeyStorage;
using _Project.Develop.Runtime.Utilities.DataManagement.Serializers;
using _Project.Develop.Runtime.Utilities.InputManagement;
using _Project.Develop.Runtime.Utilities.Reactive;
using _Project.Develop.Runtime.Utilities.SceneManagement;
using UnityEngine;
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
            container.RegisterAsSingle(CreateWalletService).NonLazy();
            container.RegisterAsSingle(CreatePlayerDataProvider);
            container.RegisterAsSingle(CreateGameplayDataProvider);
            container.RegisterAsSingle(CreateProgressService);
            container.RegisterAsSingle(CreateProjectPresentersFactory);
            container.RegisterAsSingle(CreateViewsFactory);
            container.RegisterAsSingle(CreateResetProgressService);
            container.RegisterAsSingle<IInputService>(CreateKeyboardInputService);
            container.RegisterAsSingle<ISaveLoadService>(CreateSaveLoadService);
        }
        
        private static ViewsFactory CreateViewsFactory(DIContainer container)
            => new ViewsFactory(container.Resolve<ResourcesAssetsLoader>());

        private static ProjectPresentersFactory CreateProjectPresentersFactory(DIContainer container)
            => new ProjectPresentersFactory(container);
        
        private static PlayerDataProvider CreatePlayerDataProvider(DIContainer container)
        {
            ISaveLoadService saveLoadService = container.Resolve<ISaveLoadService>();
            ConfigsProviderService configsProviderService = container.Resolve<ConfigsProviderService>();
            
            return new PlayerDataProvider(saveLoadService, configsProviderService);
        }
        
        private static GameplayDataProvider CreateGameplayDataProvider(DIContainer container)
        {
            ISaveLoadService saveLoadService = container.Resolve<ISaveLoadService>();
            ConfigsProviderService configsProviderService = container.Resolve<ConfigsProviderService>();
            
            return new GameplayDataProvider(saveLoadService, configsProviderService);
        }
        
        private static ProgressService CreateProgressService(DIContainer container)
        {
            GameplayDataProvider gameplayDataProvider = container.Resolve<GameplayDataProvider>();
            CoroutinesPerformer coroutinesPerformer = container.Resolve<CoroutinesPerformer>();
            
            Dictionary<ProgressTypes, ReactiveVariable<int>> progressItems = new();

            foreach (ProgressTypes type in Enum.GetValues(typeof(ProgressTypes)))
                progressItems[type] = new ReactiveVariable<int>();
            
            return new ProgressService(progressItems, gameplayDataProvider, coroutinesPerformer);
        }

        private static ResetProgressService CreateResetProgressService(DIContainer container)
        {
            ProgressService progressService = container.Resolve<ProgressService>();
            WalletService walletService = container.Resolve<WalletService>();
            ConfigsProviderService configsProviderService = container.Resolve<ConfigsProviderService>();
            
            return new ResetProgressService(progressService, walletService, configsProviderService);
        }
        
        private static SaveLoadService CreateSaveLoadService(DIContainer container)
        {
            IDataSerializer serializer = new JsonSerializer();
            IDataKeyStorage dataKeyStorage = new MapDataKeysStorage();
            
            string saveFolderPath = Application.isEditor ? Application.dataPath : Application.persistentDataPath;
            
            IDataRepository dataRepository = new LocalFileDataRepository(saveFolderPath, "json");
            
            return new SaveLoadService(serializer, dataKeyStorage, dataRepository);
        }
        
        private static WalletService CreateWalletService(DIContainer container)
        {
            Dictionary<CurrencyTypes, ReactiveVariable<int>> currencies = new();

            foreach (CurrencyTypes currencyType in Enum.GetValues(typeof(CurrencyTypes)))
                currencies[currencyType] = new ReactiveVariable<int>();
            
            return new WalletService(currencies, container.Resolve<PlayerDataProvider>());
        }
        
        private static ResourcesAssetsLoader CreateResuorcesAssetsLoader(DIContainer container) 
            => new ResourcesAssetsLoader();
        
        private static SceneLoaderService CreateSceneLoaderService(DIContainer container)
            => new SceneLoaderService();

        private static SceneSwitcherService CreateSceneSwitcherService(DIContainer container)
            => new SceneSwitcherService(container.Resolve<SceneLoaderService>(), container);

        private static ConfigsProviderService CreateConfigsProviderService(DIContainer container)
        {
            IConfigsLoader loader = new ResourcesConfigsLoader(container.Resolve<ResourcesAssetsLoader>()); 
            
            return new ConfigsProviderService(loader);
        }

        private static ResourcesConfigsLoader CreateResourcesConfigsLoader(DIContainer container)
        {
            ResourcesAssetsLoader assetsLoader = container.Resolve<ResourcesAssetsLoader>();
            
            return new ResourcesConfigsLoader(assetsLoader);
        }

        private static CoroutinesPerformer CreateCoroutinesPerformer(DIContainer container)
        {
            ResourcesAssetsLoader assetsLoader = container.Resolve<ResourcesAssetsLoader>();
            
            CoroutinesPerformer coroutinesPerformerPrefab = assetsLoader.Load<CoroutinesPerformer>("Utilities/CoroutinesPerformer");
            
            return Object.Instantiate(coroutinesPerformerPrefab);
        }
        
        private static KeyboardInputService CreateKeyboardInputService(DIContainer container)
            => new KeyboardInputService();
    }
}
