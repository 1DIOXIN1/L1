using System.Collections;
using _Project.Develop.Runtime.Infrastructure;
using _Project.Develop.Runtime.Infrastructure.DI;
using _Project.Develop.Runtime.Utilities.SceneManagement;

namespace _Project.Develop.Runtime.Meta.Infrastructure
{
    public class MainMenuBootstrap : SceneBootstrap
    {
        private DIContainer _container;
        
        public override void ProcessRegistrations(DIContainer container, IInputSceneArgs sceneArgs = null)
        {
            _container =  container;
            
            MainMenuContextRegistrations.Process(container);
        }
        
        public override IEnumerator Initialize()
        {
            yield break;
        }

        public override void Run()
        {
            
        }
    }
}