using System.Collections;
using _Project.Develop.Runtime.Configs;
using _Project.Develop.Runtime.Gameplay.Infrastructure;
using _Project.Develop.Runtime.Infrastructure.DI;
using _Project.Develop.Runtime.Utilities;
using _Project.Develop.Runtime.Utilities.ConfigsManagement;
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
            
            yield return projectContainer.Resolve<ConfigsProviderService>().LoadAsync();
            
            Debug.Log("End load");
            
            yield return projectContainer.Resolve<SceneSwitcherService>().ProcessSwitchTo(Scenes.MainMenu);
        }
    }
}
