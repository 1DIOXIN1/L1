using System;
using System.Collections;
using _Project.Develop.Runtime.Gameplay.Infrastructure;
using _Project.Develop.Runtime.Utilities.CoroutinesManagement;
using _Project.Develop.Runtime.Utilities.DataManagement.DataProviders;
using _Project.Develop.Runtime.Utilities.InputManagement;
using _Project.Develop.Runtime.Utilities.SceneManagement;
using UnityEngine;

namespace _Project.Develop.Runtime.Meta.Features.Missions
{
    public class LocationTravelService
    {
        private readonly SceneSwitcherService _sceneSwitcher;
        private readonly CoroutinesPerformer _coroutinesPerformer;
        private readonly MissionService _missionService;
        private readonly PlayerDataProvider _playerDataProvider;
        private readonly GameplayDataProvider _gameplayDataProvider;
        private readonly SettingsDataProvider _settingsDataProvider;
        private readonly IInputService _inputService;

        private bool _isTraveling;

        public LocationTravelService(
            SceneSwitcherService sceneSwitcher,
            CoroutinesPerformer coroutinesPerformer,
            MissionService missionService,
            PlayerDataProvider playerDataProvider,
            GameplayDataProvider gameplayDataProvider,
            SettingsDataProvider settingsDataProvider,
            IInputService inputService)
        {
            _sceneSwitcher = sceneSwitcher;
            _coroutinesPerformer = coroutinesPerformer;
            _missionService = missionService;
            _playerDataProvider = playerDataProvider;
            _gameplayDataProvider = gameplayDataProvider;
            _settingsDataProvider = settingsDataProvider;
            _inputService = inputService;
        }

        public void GoToHub()
        {
            if (_isTraveling)
            {
                Debug.LogWarning("[LocationTravel] GoToHub ignored: already traveling.");
                return;
            }

            BeginTravel();
            _coroutinesPerformer.StartPerform(GoToHubRoutine());
        }

        public bool TryTravelToMission(string missionId)
        {
            if (_isTraveling)
            {
                Debug.LogWarning($"[LocationTravel] Travel to '{missionId}' ignored: already traveling.");
                return false;
            }

            if (string.IsNullOrEmpty(missionId))
            {
                Debug.LogWarning("[LocationTravel] Travel ignored: mission id is empty.");
                return false;
            }

            if (_missionService.IsUnlocked(missionId) == false)
            {
                Debug.LogWarning($"[LocationTravel] Travel to '{missionId}' ignored: mission is locked.");
                return false;
            }

            if (_missionService.TryGetMission(missionId, out MissionDefinition mission) == false)
            {
                Debug.LogWarning($"[LocationTravel] Travel to '{missionId}' ignored: mission not found in catalog.");
                return false;
            }

            BeginTravel();
            _coroutinesPerformer.StartPerform(TravelToMissionRoutine(mission));
            return true;
        }

        private void BeginTravel()
        {
            _isTraveling = true;

            if (_inputService is Controller controller)
                controller.Disable();
        }

        private IEnumerator GoToHubRoutine()
        {
            yield return TravelRoutine(Scenes.Hub, new HubInputArgs());
        }

        private IEnumerator TravelToMissionRoutine(MissionDefinition mission)
        {
            yield return TravelRoutine(mission.SceneName, new GameplayInputArgs(mission.Id));
        }

        private IEnumerator TravelRoutine(string sceneName, IInputSceneArgs sceneArgs)
        {
            bool completed = false;

            try
            {
                yield return SaveAll();
                yield return _sceneSwitcher.ProcessSwitchTo(sceneName, sceneArgs);
                completed = true;
            }
            finally
            {
                _isTraveling = false;

                if (completed == false && _inputService is Controller controller)
                    controller.Enable();
            }
        }

        private IEnumerator SaveAll()
        {
            yield return _playerDataProvider.Save();
            yield return _gameplayDataProvider.Save();
            yield return _settingsDataProvider.Save();
        }
    }
}
