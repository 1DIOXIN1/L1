using System;
using System.Collections.Generic;
using _Project.Develop.Runtime.UI.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Develop.Runtime.UI.Gameplay.Phone
{
    [Serializable]
    public class MissionTravelButtonBinding
    {
        [SerializeField] private string missionId;
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text label;

        public string MissionId => missionId;
        public Button Button => button;
        public TMP_Text Label => label;
    }

    public class MapPanelView : MonoBehaviour, IView
    {
        public event Action<string> MissionTravelClicked;
        public event Action HubTravelClicked;

        [SerializeField] private MissionTravelButtonBinding[] missionButtons = Array.Empty<MissionTravelButtonBinding>();
        [SerializeField] private Button hubButton;

        private readonly List<UnityEngine.Events.UnityAction> _missionClickHandlers = new();

        public IReadOnlyList<MissionTravelButtonBinding> MissionButtons => missionButtons;

        private void OnEnable()
        {
            _missionClickHandlers.Clear();

            for (int i = 0; i < missionButtons.Length; i++)
            {
                MissionTravelButtonBinding binding = missionButtons[i];
                string missionId = binding.MissionId;
                UnityEngine.Events.UnityAction handler = () => OnMissionClicked(missionId);
                _missionClickHandlers.Add(handler);
                binding.Button.onClick.AddListener(handler);
            }

            hubButton.onClick.AddListener(OnHubClicked);
        }

        private void OnDisable()
        {
            int handlerIndex = 0;
            for (int i = 0; i < missionButtons.Length; i++)
            {
                MissionTravelButtonBinding binding = missionButtons[i];
                binding.Button.onClick.RemoveListener(_missionClickHandlers[handlerIndex]);
                handlerIndex++;
            }

            _missionClickHandlers.Clear();
            hubButton.onClick.RemoveListener(OnHubClicked);
        }

        public void SetMissionEntry(string missionId, string displayName, bool interactable)
        {
            for (int i = 0; i < missionButtons.Length; i++)
            {
                MissionTravelButtonBinding binding = missionButtons[i];
                if (binding.MissionId != missionId)
                    continue;

                binding.Label.text = displayName;
                binding.Button.interactable = interactable;
                binding.Button.gameObject.SetActive(true);
                return;
            }
        }

        private void OnMissionClicked(string missionId) => MissionTravelClicked?.Invoke(missionId);

        private void OnHubClicked() => HubTravelClicked?.Invoke();
    }
}
