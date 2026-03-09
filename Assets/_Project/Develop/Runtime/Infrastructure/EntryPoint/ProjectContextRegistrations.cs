using _Project.Develop.Runtime.Infrastructure.DI;
using _Project.Develop.Runtime.Utilities;
using _Project.Develop.Runtime.Utilities.AssetsManagement;
using _Project.Develop.Runtime.Utilities.ConfigsManagement;
using _Project.Develop.Runtime.Utilities.SceneManagement;
using UnityEngine;

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
            ResuorcesAssetsLoader assetsLoader =  container.Resolve<ResuorcesAssetsLoader>();
            
            return new ResourcesConfigsLoader(assetsLoader);
        }

        private static CoroutinesPerformer CreateCoroutinesPerformer(DIContainer container)
        {
            ResuorcesAssetsLoader assetsLoader = container.Resolve<ResuorcesAssetsLoader>();
            
            CoroutinesPerformer coroutinesPerformerPrefab = assetsLoader.Load<CoroutinesPerformer>("Utilities/CoroutinesPerformer");
            
            return Object.Instantiate(coroutinesPerformerPrefab);
        }
    }
}
