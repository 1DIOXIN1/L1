using System.Collections;
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
        private void Awake()
        {
            DIContainer projectContainer = new DIContainer();

            SetupAppSettings();

            ProjectContextRegistrations.Process(projectContainer);
            
            projectContainer.Initialize();

            projectContainer.Resolve<CoroutinesPerformer>().StartCoroutine(StartGame(projectContainer));
        }
    
        private void SetupAppSettings()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }

        private IEnumerator StartGame(DIContainer projectContainer)
        {
            Debug.Log("Start load");
            PlayerDataProvider playerDataProvider = projectContainer.Resolve<PlayerDataProvider>();
            
            yield return projectContainer.Resolve<ConfigsProviderService>().LoadAsync();

            bool isPlayerSaveExists = false;

            yield return playerDataProvider.Exists(result => isPlayerSaveExists = result);
            
            if(isPlayerSaveExists)
                yield return playerDataProvider.Load();
            else
                playerDataProvider.Reset();
                
            Debug.Log("End load");
            
            yield return projectContainer.Resolve<SceneSwitcherService>().ProcessSwitchTo(Scenes.MainMenu);
        }
    }
}
