using System.Collections;
using _Project.Develop.Runtime.Gameplay.Infrastructure;
using _Project.Develop.Runtime.Utilities.CoroutinesManagement;
using _Project.Develop.Runtime.Utilities.DataManagement.DataProviders;
using _Project.Develop.Runtime.Utilities.InputManagement;
using _Project.Develop.Runtime.Utilities.SceneManagement;
using UnityEngine;

namespace _Project.Develop.Runtime.UI.MainMenu
{
    public class MainMenuNavigationService
    {
        private readonly CoroutinesPerformer _coroutinesPerformer;
        private readonly SceneSwitcherService _sceneSwitcher;
        private readonly PlayerDataProvider _playerDataProvider;
        private readonly IInputService _inputService;

        public MainMenuNavigationService(
            CoroutinesPerformer coroutinesPerformer,
            SceneSwitcherService sceneSwitcher,
            PlayerDataProvider playerDataProvider,
            IInputService inputService)
        {
            _coroutinesPerformer = coroutinesPerformer;
            _sceneSwitcher = sceneSwitcher;
            _playerDataProvider = playerDataProvider;
            _inputService = inputService;
        }

        public void StartGame(GameplayType gameplayType = GameplayType.Numbers)
        {
            if (_inputService is Controller controller)
                controller.Disable();

            _coroutinesPerformer.StartPerform(StartGameRoutine(gameplayType));
        }

        public void ExitGame()
        {
            if (_inputService is Controller controller)
                controller.Disable();

            _coroutinesPerformer.StartPerform(ExitGameRoutine());
        }

        private IEnumerator StartGameRoutine(GameplayType gameplayType)
        {
            yield return _playerDataProvider.Save();
            yield return _sceneSwitcher.ProcessSwitchTo(Scenes.GamePlay, new GameplayInputArgs(gameplayType));
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
