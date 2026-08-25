using System;
using _Project.Develop.Runtime.Gameplay.Infrastructure.Mission;
using _Project.Develop.Runtime.Meta.Features.Progress;
using _Project.Develop.Runtime.Utilities.CoroutinesManagement;
using _Project.Develop.Runtime.Utilities.DataManagement.DataProviders;
using _Project.Develop.Runtime.Utilities.InputManagement;
using _Project.Develop.Runtime.Utilities.SceneManagement;

namespace _Project.Develop.Runtime.Gameplay.Infrastructure
{
    public class GameplayCycle : IDisposable
    {
        private readonly GameMode _gameMode;
        private readonly SceneSwitcherService _sceneSwitcherService;
        private readonly CoroutinesPerformer _coroutinesPerformer;
        private readonly GameplayDataProvider _gameplayDataProvider;
        private readonly ProgressService _progressService;
        private readonly IInputService _inputService;

        private bool _isGameFinished;
        private bool _isSwitchingScene;

        public GameplayCycle(
            GameMode gameMode,
            IInputService inputService,
            SceneSwitcherService sceneSwitcherService,
            CoroutinesPerformer coroutinesPerformer,
            GameplayDataProvider gameplayDataProvider,
            ProgressService progressService)
        {
            _gameMode = gameMode;
            _inputService = inputService;
            _sceneSwitcherService = sceneSwitcherService;
            _coroutinesPerformer = coroutinesPerformer;
            _gameplayDataProvider = gameplayDataProvider;
            _progressService = progressService;

            _inputService.ConfirmPressed += OnConfirmPressed;
            _gameMode.MissionEnded += OnMissionEnded;
        }

        public void StartGame(GameplayInputArgs gameplayInputArgs)
        {
            _isGameFinished = false;
            _isSwitchingScene = false;

            _gameMode.Start();
        }

        public void Dispose()
        {
            _inputService.ConfirmPressed -= OnConfirmPressed;
            _gameMode.MissionEnded -= OnMissionEnded;
        }

        private void OnConfirmPressed()
        {
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

            _isGameFinished = true;
        }
    }
}
