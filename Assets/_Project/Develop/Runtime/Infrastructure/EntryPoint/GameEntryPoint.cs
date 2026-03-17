using System.Collections;
using System.Collections.Generic;
using _Project.Develop.Runtime.Infrastructure.DI;
using _Project.Develop.Runtime.Utilities;
using _Project.Develop.Runtime.Utilities.ConfigsManagement;
using _Project.Develop.Runtime.Utilities.DataManagement.DataProviders;
using _Project.Develop.Runtime.Utilities.SceneManagement;
using UnityEngine;

namespace _Project.Develop.Runtime.Infrastructure.EntryPoint
{
    public class GameEntryPoint : MonoBehaviour
    {
        private DIContainer _projectContainer;
        
        private void Awake()
        {
            _projectContainer = new DIContainer();

            SetupAppSettings();

            ProjectContextRegistrations.Process(_projectContainer);
            
            _projectContainer.Initialize();
            
            _projectContainer.Resolve<CoroutinesPerformer>().StartCoroutine(StartGame());
        }
    
        private void SetupAppSettings()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }

        private IEnumerator StartGame()
        {
            Debug.Log("Start load");
            
            yield return _projectContainer.Resolve<ConfigsProviderService>().LoadAsync();
            
            yield return LoadData();
            
            Debug.Log("End load");
            
            yield return _projectContainer.Resolve<SceneSwitcherService>().ProcessSwitchTo(Scenes.MainMenu);
        }

        private IEnumerator LoadData()
        {
            var providers = new List<IDataProvider>()
            {
                _projectContainer.Resolve<PlayerDataProvider>(),
                _projectContainer.Resolve<GameplayDataProvider>(),
            };

            foreach (var provider in providers)
            {
                bool exists = false;
                
                yield return provider.Exists(result => exists = result);

                if (exists)
                {
                    yield return provider.Load();
                }
                else
                {
                    provider.Reset();
                    yield return provider.Save();
                }
            }
        }
    }
}
