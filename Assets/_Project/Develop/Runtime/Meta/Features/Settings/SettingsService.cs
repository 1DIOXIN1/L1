using _Project.Develop.Runtime.Utilities.CoroutinesManagement;
using _Project.Develop.Runtime.Utilities.DataManagement;
using _Project.Develop.Runtime.Utilities.DataManagement.DataProviders;
using UnityEngine;

namespace _Project.Develop.Runtime.Meta.Features.Settings
{
    public class SettingsService : IDataReader<SettingsData>, IDataWriter<SettingsData>
    {
        public const float MinMouseSensitivity = 0.1f;
        public const float MaxMouseSensitivity = 2f;

        private readonly SettingsDataProvider _settingsDataProvider;
        private readonly CoroutinesPerformer _coroutinesPerformer;

        private float _masterVolume = 1f;
        private float _mouseSensitivity = 1f;
        private bool _isDirty;

        public SettingsService(
            SettingsDataProvider settingsDataProvider,
            CoroutinesPerformer coroutinesPerformer)
        {
            _settingsDataProvider = settingsDataProvider;
            _coroutinesPerformer = coroutinesPerformer;

            _settingsDataProvider.RegisterReader(this);
            _settingsDataProvider.RegisterWriter(this);
        }

        public float MasterVolume => _masterVolume;
        public float MouseSensitivity => _mouseSensitivity;

        public void SetMasterVolume(float value)
        {
            float clamped = Mathf.Clamp01(value);
            if (Mathf.Approximately(_masterVolume, clamped))
                return;

            _masterVolume = clamped;
            _isDirty = true;
            Apply();
        }

        public void SetMouseSensitivity(float value)
        {
            float clamped = Mathf.Clamp(value, MinMouseSensitivity, MaxMouseSensitivity);
            if (Mathf.Approximately(_mouseSensitivity, clamped))
                return;

            _mouseSensitivity = clamped;
            _isDirty = true;
            Apply();
        }

        public void Apply()
        {
            AudioListener.volume = _masterVolume;
        }

        public void SaveIfDirty()
        {
            if (_isDirty == false)
                return;

            _isDirty = false;
            _coroutinesPerformer.StartPerform(_settingsDataProvider.Save());
        }

        public void WriteTo(SettingsData data)
        {
            data.MasterVolume = _masterVolume;
            data.MouseSensitivity = _mouseSensitivity;
        }

        public void ReadFrom(SettingsData data)
        {
            _masterVolume = Mathf.Clamp01(data.MasterVolume);
            _mouseSensitivity = Mathf.Clamp(data.MouseSensitivity, MinMouseSensitivity, MaxMouseSensitivity);
            _isDirty = false;
            Apply();
        }
    }
}
