using System;
using _Project.Develop.Runtime.Gameplay.Infrastructure.Mission;
using _Project.Develop.Runtime.Meta.Features.Missions;
using _Project.Develop.Runtime.Meta.Features.Player;
using _Project.Develop.Runtime.Meta.Features.Progress;
using _Project.Develop.Runtime.Utilities.CoroutinesManagement;
using _Project.Develop.Runtime.Utilities.DataManagement.DataProviders;
using _Project.Develop.Runtime.Utilities.InputManagement;

namespace _Project.Develop.Runtime.Gameplay.Infrastructure
{
    public class GameplayCycle : IDisposable
    {
        private readonly GameMode _gameMode;
        private readonly CoroutinesPerformer _coroutinesPerformer;
        private readonly GameplayDataProvider _gameplayDataProvider;
        private readonly PlayerDataProvider _playerDataProvider;
        private readonly PlayerStateService _playerStateService;
        private readonly ProgressService _progressService;
        private readonly MissionService _missionService;
        private readonly LocationTravelService _locationTravel;
        private readonly IInputService _inputService;

        private GameplayInputArgs _gameplayInputArgs;
        private bool _isGameFinished;
        private bool _isSwitchingScene;
        private bool _playerStateCaptured;

        public GameplayCycle(
            GameMode gameMode,
            IInputService inputService,
            CoroutinesPerformer coroutinesPerformer,
            GameplayDataProvider gameplayDataProvider,
            PlayerDataProvider playerDataProvider,
            PlayerStateService playerStateService,
            ProgressService progressService,
            MissionService missionService,
            LocationTravelService locationTravel)
        {
            _gameMode = gameMode;
            _inputService = inputService;
            _coroutinesPerformer = coroutinesPerformer;
            _gameplayDataProvider = gameplayDataProvider;
            _playerDataProvider = playerDataProvider;
            _playerStateService = playerStateService;
            _progressService = progressService;
            _missionService = missionService;
            _locationTravel = locationTravel;

            _gameMode.MissionEnded += OnMissionEnded;
        }

        public void StartGame(GameplayInputArgs gameplayInputArgs)
        {
            _gameplayInputArgs = gameplayInputArgs;
            _isGameFinished = false;
            _isSwitchingScene = false;
            _playerStateCaptured = false;

            _gameMode.Start(gameplayInputArgs.MissionId);
        }

        public void Dispose()
        {
            _gameMode.MissionEnded -= OnMissionEnded;
        }

        private void OnMissionEnded(MissionResult result)
        {
            if (_isGameFinished)
                return;

            _isGameFinished = true;

            if (result.IsSuccess)
            {
                _progressService.Win();

                if (_gameplayInputArgs != null && string.IsNullOrEmpty(_gameplayInputArgs.MissionId) == false)
                    _missionService.Complete(_gameplayInputArgs.MissionId);
            }
            else
            {
                _progressService.Lose();
            }

            CaptureAndSavePlayerState();
            _playerStateService.RestoreHealth();
            _playerStateService.RefillAmmo();
            _coroutinesPerformer.StartPerform(_playerDataProvider.Save());
            _coroutinesPerformer.StartPerform(_gameplayDataProvider.Save());

            if (_isSwitchingScene)
                return;

            _isSwitchingScene = true;
            _locationTravel.GoToHub();
        }

        private void CaptureAndSavePlayerState()
        {
            if (_playerStateCaptured)
                return;

            _playerStateCaptured = true;
            _gameMode.CapturePlayerState(_playerStateService);
            _coroutinesPerformer.StartPerform(_playerDataProvider.Save());
        }
    }
}
