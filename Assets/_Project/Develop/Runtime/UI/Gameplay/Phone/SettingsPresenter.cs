using System.Collections;
using _Project.Develop.Runtime.Meta.Features.Settings;
using _Project.Develop.Runtime.UI.Core;
using _Project.Develop.Runtime.Utilities.CoroutinesManagement;
using _Project.Develop.Runtime.Utilities.DataManagement.DataProviders;
using _Project.Develop.Runtime.Utilities.InputManagement;
using _Project.Develop.Runtime.Utilities.SceneManagement;

namespace _Project.Develop.Runtime.UI.Gameplay.Phone
{
    public class SettingsPresenter : IPresenter
    {
        private readonly SettingsPanelView _view;
        private readonly SettingsService _settings;
        private readonly SettingsDataProvider _settingsDataProvider;
        private readonly PlayerDataProvider _playerDataProvider;
        private readonly GameplayDataProvider _gameplayDataProvider;
        private readonly SceneSwitcherService _sceneSwitcher;
        private readonly CoroutinesPerformer _coroutinesPerformer;
        private readonly IInputService _input;

        private bool _isSwitching;

        public SettingsPresenter(
            SettingsPanelView view,
            SettingsService settings,
            SettingsDataProvider settingsDataProvider,
            PlayerDataProvider playerDataProvider,
            GameplayDataProvider gameplayDataProvider,
            SceneSwitcherService sceneSwitcher,
            CoroutinesPerformer coroutinesPerformer,
            IInputService input)
        {
            _view = view;
            _settings = settings;
            _settingsDataProvider = settingsDataProvider;
            _playerDataProvider = playerDataProvider;
            _gameplayDataProvider = gameplayDataProvider;
            _sceneSwitcher = sceneSwitcher;
            _coroutinesPerformer = coroutinesPerformer;
            _input = input;
        }

        public void Initialize()
        {
            _view.SetValues(_settings.MasterVolume, _settings.MouseSensitivity);

            _view.VolumeChanged += OnVolumeChanged;
            _view.SensitivityChanged += OnSensitivityChanged;
            _view.MainMenuClicked += OnMainMenuClicked;
        }

        public void Dispose()
        {
            _view.VolumeChanged -= OnVolumeChanged;
            _view.SensitivityChanged -= OnSensitivityChanged;
            _view.MainMenuClicked -= OnMainMenuClicked;

            _settings.SaveIfDirty();
        }

        public void Show()
        {
            _view.SetValues(_settings.MasterVolume, _settings.MouseSensitivity);
        }

        public void FlushSave()
        {
            _settings.SaveIfDirty();
        }

        private void OnVolumeChanged(float value)
        {
            _settings.SetMasterVolume(value);
        }

        private void OnSensitivityChanged(float value)
        {
            _settings.SetMouseSensitivity(value);
        }

        private void OnMainMenuClicked()
        {
            if (_isSwitching)
                return;

            _isSwitching = true;
            _settings.SaveIfDirty();

            if (_input is Controller controller)
                controller.Disable();

            _coroutinesPerformer.StartPerform(ReturnToMainMenuRoutine());
        }

        private IEnumerator ReturnToMainMenuRoutine()
        {
            yield return _settingsDataProvider.Save();
            yield return _playerDataProvider.Save();
            yield return _gameplayDataProvider.Save();
            yield return _sceneSwitcher.ProcessSwitchTo(Scenes.MainMenu);
        }
    }
}
