using System.Collections;
using _Project.Develop.Runtime.Infrastructure;
using _Project.Develop.Runtime.Infrastructure.DI;
using Object = UnityEngine.Object;

namespace _Project.Develop.Runtime.Utilities.SceneManagement
{
    public class SceneSwitcherService
    {
        private readonly SceneLoaderService _sceneLoaderService;
        private readonly DIContainer _projectContainer;
        private DIContainer _currentSceneContainer;

        public SceneSwitcherService(SceneLoaderService sceneLoaderService, DIContainer projectContainer)
        {
            _sceneLoaderService = sceneLoaderService;
            _projectContainer = projectContainer;
        }

        public IEnumerator ProcessSwitchTo(string sceneName, IInputSceneArgs sceneArgs = null)
        {
            _currentSceneContainer?.Dispose();
            
            yield return _sceneLoaderService.LoadAsync(Scenes.Empty);
            yield return _sceneLoaderService.LoadAsync(sceneName);
            
            SceneBootstrap sceneBootstrap = Object.FindObjectOfType<SceneBootstrap>();

            _currentSceneContainer = new DIContainer(_projectContainer);

            sceneBootstrap.ProcessRegistrations(_currentSceneContainer, sceneArgs);
            
            _currentSceneContainer.Initialize();

            yield return sceneBootstrap.Initialize();

            sceneBootstrap.Run();
        }
    }
}