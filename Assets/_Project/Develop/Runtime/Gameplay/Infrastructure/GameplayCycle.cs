using _Project.Develop.Runtime.Utilities;
using _Project.Develop.Runtime.Utilities.InputManagement;
using _Project.Develop.Runtime.Utilities.SceneManagement;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Infrastructure
{
    public class GameplayCycle
    {
        private readonly GameMode _gameMode;
        private readonly SceneSwitcherService _sceneSwitcherService;
        private readonly CoroutinesPerformer _coroutinesPerformer;
        private readonly IInputService _inputService;

        private GameplayInputArgs _gameplayInputArgs;

        private bool _isGameFinished;
        private bool _isWin;

        private bool _isSwitchingScene;

        public GameplayCycle(
            GameMode gameMode,
            IInputService inputService,
            SceneSwitcherService sceneSwitcherService,
            CoroutinesPerformer coroutinesPerformer)
        {
            _gameMode = gameMode;
            _inputService = inputService;
            _sceneSwitcherService = sceneSwitcherService;
            _coroutinesPerformer = coroutinesPerformer;

            _inputService.ConfirmPressed += OnConfirmPressed;
            _gameMode.Win += OnGameModeWin;
            _gameMode.Defeat += OnGameModeDefeat;
        }

        public void StartGame(GameplayInputArgs gameplayInputArgs)
        {
            _gameplayInputArgs = gameplayInputArgs;

            _isGameFinished = false;
            _isWin = false;
            _isSwitchingScene = false;

            _gameMode.Start();
        }

        private void OnConfirmPressed()
        {
            if (!_isGameFinished)
                return;

            if (_isSwitchingScene)
                return;

            _isSwitchingScene = true;

            if (_isWin)
            {
                Debug.Log("Switching to MainMenu");

                _coroutinesPerformer.StartPerform(
                    _sceneSwitcherService.ProcessSwitchTo(Scenes.MainMenu));
            }
            else
            {
                Debug.Log("Restart Gameplay");

                _coroutinesPerformer.StartPerform(
                    _sceneSwitcherService.ProcessSwitchTo(
                        Scenes.GamePlay,
                        _gameplayInputArgs));
            }
        }

        private void OnGameModeWin()
        {
            Debug.Log("Win");

            _isWin = true;
            _isGameFinished = true;
        }

        private void OnGameModeDefeat()
        {
            Debug.Log("Defeat");

            _isGameFinished = true;
        }
    }
}