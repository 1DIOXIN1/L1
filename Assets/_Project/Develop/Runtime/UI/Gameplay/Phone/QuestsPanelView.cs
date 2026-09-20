using System.Collections.Generic;
using _Project.Develop.Runtime.Gameplay.Infrastructure.Mission;
using _Project.Develop.Runtime.UI.Core;
using TMPro;
using UnityEngine;

namespace _Project.Develop.Runtime.UI.Gameplay.Phone
{
    public class QuestsPanelView : MonoBehaviour, IView
    {
        [SerializeField] private TMP_Text missionTitleText;
        [SerializeField] private TMP_Text emptyStateText;
        [SerializeField] private Transform activePrimaryRoot;
        [SerializeField] private Transform optionalsRoot;
        [SerializeField] private Transform completedPrimariesRoot;
        [SerializeField] private QuestsObjectiveItemView itemPrefab;

        private readonly List<QuestsObjectiveItemView> _spawnedItems = new();

        public void Render(MissionQuestTracker tracker)
        {
            ClearItems();

            bool hasMission = tracker.HasActiveMission;
            emptyStateText.gameObject.SetActive(hasMission == false);
            missionTitleText.gameObject.SetActive(hasMission);

            if (hasMission == false)
                return;

            missionTitleText.text = tracker.MissionDisplayName;

            if (tracker.ActivePrimary.HasValue)
                Spawn(tracker.ActivePrimary.Value, activePrimaryRoot);

            for (int i = 0; i < tracker.Optionals.Count; i++)
                Spawn(tracker.Optionals[i], optionalsRoot);

            for (int i = 0; i < tracker.CompletedPrimaries.Count; i++)
                Spawn(tracker.CompletedPrimaries[i], completedPrimariesRoot);
        }

        private void Spawn(QuestObjectiveUiEntry entry, Transform parent)
        {
            QuestsObjectiveItemView item = Instantiate(itemPrefab, parent);
            item.gameObject.SetActive(true);
            item.Set(entry.Description, entry.IsCompleted);
            _spawnedItems.Add(item);
        }

        private void ClearItems()
        {
            for (int i = 0; i < _spawnedItems.Count; i++)
                Destroy(_spawnedItems[i].gameObject);

            _spawnedItems.Clear();
        }

        private void OnDestroy()
        {
            ClearItems();
        }
    }
}
