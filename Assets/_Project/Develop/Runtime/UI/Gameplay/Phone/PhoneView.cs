using System;
using _Project.Develop.Runtime.UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Develop.Runtime.UI.Gameplay.Phone
{
    public class PhoneView : MonoBehaviour, IView
    {
        public event Action SettingsClicked;
        public event Action MapClicked;
        public event Action QuestsClicked;
        public event Action MessageClicked;

        [SerializeField] private Button settingsButton;
        [SerializeField] private Button mapButton;
        [SerializeField] private Button questsButton;
        [SerializeField] private Button messageButton;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject mapPanel;
        [SerializeField] private GameObject questsPanel;
        [SerializeField] private GameObject messagePanel;
        [SerializeField] private SettingsPanelView settingsPanelView;

        public bool IsVisible => gameObject.activeSelf;
        public SettingsPanelView SettingsPanelView => settingsPanelView;

        private void OnEnable()
        {
            if (settingsButton != null)
                settingsButton.onClick.AddListener(OnSettingsClicked);

            if (mapButton != null)
                mapButton.onClick.AddListener(OnMapClicked);

            if (questsButton != null)
                questsButton.onClick.AddListener(OnQuestsClicked);

            if (messageButton != null)
                messageButton.onClick.AddListener(OnMessageClicked);
        }

        private void OnDisable()
        {
            if (settingsButton != null)
                settingsButton.onClick.RemoveListener(OnSettingsClicked);

            if (mapButton != null)
                mapButton.onClick.RemoveListener(OnMapClicked);

            if (questsButton != null)
                questsButton.onClick.RemoveListener(OnQuestsClicked);

            if (messageButton != null)
                messageButton.onClick.RemoveListener(OnMessageClicked);
        }

        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }

        public void ShowTab(PhoneTab tab)
        {
            SetPanelActive(settingsPanel, tab == PhoneTab.Settings);
            SetPanelActive(mapPanel, tab == PhoneTab.Map);
            SetPanelActive(questsPanel, tab == PhoneTab.Quests);
            SetPanelActive(messagePanel, tab == PhoneTab.Message);
        }

        private static void SetPanelActive(GameObject panel, bool active)
        {
            if (panel != null)
                panel.SetActive(active);
        }

        private void OnSettingsClicked() => SettingsClicked?.Invoke();

        private void OnMapClicked() => MapClicked?.Invoke();

        private void OnQuestsClicked() => QuestsClicked?.Invoke();

        private void OnMessageClicked() => MessageClicked?.Invoke();
    }
}
