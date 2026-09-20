using System.Collections;
using _Project.Develop.Runtime.Meta.Features.Missions;
using _Project.Develop.Runtime.Utilities.CoroutinesManagement;
using _Project.Develop.Runtime.Utilities.DataManagement.DataProviders;
using _Project.Develop.Runtime.Utilities.InputManagement;
using UnityEngine;

namespace _Project.Develop.Runtime.UI.MainMenu
{
    public class MainMenuNavigationService
    {
        private readonly LocationTravelService _locationTravel;
        private readonly CoroutinesPerformer _coroutinesPerformer;
        private readonly PlayerDataProvider _playerDataProvider;
        private readonly IInputService _inputService;

        public MainMenuNavigationService(
            LocationTravelService locationTravel,
            CoroutinesPerformer coroutinesPerformer,
            PlayerDataProvider playerDataProvider,
            IInputService inputService)
        {
            _locationTravel = locationTravel;
            _coroutinesPerformer = coroutinesPerformer;
            _playerDataProvider = playerDataProvider;
            _inputService = inputService;
        }

        public void StartGame()
        {
            _locationTravel.GoToHub();
        }

        public void ExitGame()
        {
            if (_inputService is Controller controller)
                controller.Disable();

            _coroutinesPerformer.StartPerform(ExitGameRoutine());
        }

        private IEnumerator ExitGameRoutine()
        {
            yield return _playerDataProvider.Save();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
