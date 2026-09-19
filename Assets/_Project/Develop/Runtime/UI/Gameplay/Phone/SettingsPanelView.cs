using System;
using _Project.Develop.Runtime.UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Develop.Runtime.UI.Gameplay.Phone
{
    public class SettingsPanelView : MonoBehaviour, IView
    {
        public event Action<float> VolumeChanged;
        public event Action<float> SensitivityChanged;
        public event Action MainMenuClicked;

        [SerializeField] private Slider volumeSlider;
        [SerializeField] private Slider sensitivitySlider;
        [SerializeField] private Button mainMenuButton;

        private bool _suppressEvents;

        private void OnEnable()
        {
            if (volumeSlider != null)
                volumeSlider.onValueChanged.AddListener(OnVolumeChanged);

            if (sensitivitySlider != null)
                sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);

            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        }

        private void OnDisable()
        {
            if (volumeSlider != null)
                volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);

            if (sensitivitySlider != null)
                sensitivitySlider.onValueChanged.RemoveListener(OnSensitivityChanged);

            if (mainMenuButton != null)
                mainMenuButton.onClick.RemoveListener(OnMainMenuClicked);
        }

        public void SetValues(float volume, float sensitivity)
        {
            _suppressEvents = true;

            if (volumeSlider != null)
                volumeSlider.SetValueWithoutNotify(volume);

            if (sensitivitySlider != null)
                sensitivitySlider.SetValueWithoutNotify(sensitivity);

            _suppressEvents = false;
        }

        private void OnVolumeChanged(float value)
        {
            if (_suppressEvents)
                return;

            VolumeChanged?.Invoke(value);
        }

        private void OnSensitivityChanged(float value)
        {
            if (_suppressEvents)
                return;

            SensitivityChanged?.Invoke(value);
        }

        private void OnMainMenuClicked() => MainMenuClicked?.Invoke();
    }
}
