using System;
using _Project.Develop.Runtime.Gameplay.Infrastructure.Mission;
using _Project.Develop.Runtime.Meta.Features.Player;
using _Project.Develop.Runtime.Meta.Features.Progress;
using _Project.Develop.Runtime.Utilities.CoroutinesManagement;
using _Project.Develop.Runtime.Utilities.DataManagement.DataProviders;
using _Project.Develop.Runtime.Utilities.InputManagement;
using _Project.Develop.Runtime.Utilities.SceneManagement;

namespace _Project.Develop.Runtime.Gameplay.Infrastructure
{
    public class GameplayCycle : IDisposable
    {
        // TEMP: instant GamePlay reload for test loop. Restore hub flow later.
        private const bool InstantRestartGameplayOnMissionEnd = true;

        private readonly GameMode _gameMode;
        private readonly SceneSwitcherService _sceneSwitcherService;
        private readonly CoroutinesPerformer _coroutinesPerformer;
        private readonly GameplayDataProvider _gameplayDataProvider;
        private readonly PlayerDataProvider _playerDataProvider;
        private readonly PlayerStateService _playerStateService;
        private readonly ProgressService _progressService;
        private readonly IInputService _inputService;

        private GameplayInputArgs _gameplayInputArgs;
        private bool _isGameFinished;
        private bool _isSwitchingScene;
        private bool _playerStateCaptured;

        public GameplayCycle(
            GameMode gameMode,
            IInputService inputService,
            SceneSwitcherService sceneSwitcherService,
            CoroutinesPerformer coroutinesPerformer,
            GameplayDataProvider gameplayDataProvider,
            PlayerDataProvider playerDataProvider,
            PlayerStateService playerStateService,
            ProgressService progressService)
        {
            _gameMode = gameMode;
            _inputService = inputService;
            _sceneSwitcherService = sceneSwitcherService;
            _coroutinesPerformer = coroutinesPerformer;
            _gameplayDataProvider = gameplayDataProvider;
            _playerDataProvider = playerDataProvider;
            _playerStateService = playerStateService;
            _progressService = progressService;

            _inputService.ConfirmPressed += OnConfirmPressed;
            _gameMode.MissionEnded += OnMissionEnded;
        }

        public void StartGame(GameplayInputArgs gameplayInputArgs)
        {
            _gameplayInputArgs = gameplayInputArgs;
            _isGameFinished = false;
            _isSwitchingScene = false;
            _playerStateCaptured = false;

            _gameMode.Start();
        }

        public void Dispose()
        {
            _inputService.ConfirmPressed -= OnConfirmPressed;
            _gameMode.MissionEnded -= OnMissionEnded;
        }

        private void OnConfirmPressed()
        {
            if (InstantRestartGameplayOnMissionEnd)
                return;

            if (_isGameFinished == false)
                return;

            if (_isSwitchingScene)
                return;

            _isSwitchingScene = true;

            _coroutinesPerformer.StartPerform(
                _sceneSwitcherService.ProcessSwitchTo(Scenes.MainMenu));

            _coroutinesPerformer.StartPerform(_gameplayDataProvider.Save());
        }

        private void OnMissionEnded(MissionResult result)
        {
            if (result.IsSuccess)
                _progressService.Win();
            else
                _progressService.Lose();

            CaptureAndSavePlayerState();
            _isGameFinished = true;

            if (InstantRestartGameplayOnMissionEnd == false)
                return;

            // TEMP test loop: keep next run playable.
            _playerStateService.RestoreHealth();
            _playerStateService.RefillAmmo();
            _coroutinesPerformer.StartPerform(_playerDataProvider.Save());

            if (_isSwitchingScene)
                return;

            _isSwitchingScene = true;
            _coroutinesPerformer.StartPerform(
                _sceneSwitcherService.ProcessSwitchTo(Scenes.GamePlay, _gameplayInputArgs));
            _coroutinesPerformer.StartPerform(_gameplayDataProvider.Save());
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
