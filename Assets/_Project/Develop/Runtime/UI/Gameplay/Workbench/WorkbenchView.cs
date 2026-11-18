using _Project.Develop.Runtime.Configs.Meta.Weapon;
using System;
using System.Collections.Generic;
using _Project.Develop.Runtime.UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Develop.Runtime.UI.Gameplay.Workbench
{
    public sealed class WorkbenchView : MonoBehaviour, IView
    {
        [Serializable]
        private class ActionRow
        {
            public Button button;
            public Text label;
            public Text status;
        }

        [Serializable]
        private class StatRow
        {
            public Text value;
            public RectTransform fill;
        }

        [SerializeField] private Text headingLabel;
        [SerializeField] private Text weaponsLabel;
        [SerializeField] private Text suppliesLabel;
        [SerializeField] private Text magazineCapacityLabel;
        [SerializeField] private Text maximumSpreadLabel;
        [SerializeField] private Text closeLabel;

        [SerializeField] private Button weaponsTab;
        [SerializeField] private Button suppliesTab;
        [SerializeField] private Button close;
        [SerializeField] private WorkbenchRowView rowTemplate;

        [SerializeField] private Text balance;
        [SerializeField] private Text category;
        [SerializeField] private Text title;
        [SerializeField] private Image preview;
        [SerializeField] private Text details;

        [SerializeField] private GameObject statsPanel;
        [SerializeField] private StatRow[] stats;
        [SerializeField] private ActionRow[] actions;

        private readonly List<WorkbenchRowView> _rows = new();

        public event Action<bool> TabSelected;
        public event Action<int> ItemSelected;
        public event Action<int> ActionClicked;
        public event Action Closed;

        private void Awake()
        {
            headingLabel.text = WorkbenchText.Heading;
            weaponsLabel.text = WorkbenchText.Weapons;
            suppliesLabel.text = WorkbenchText.Supplies;
            magazineCapacityLabel.text = WorkbenchText.MagazineCapacity;
            maximumSpreadLabel.text = WorkbenchText.MaximumSpread;
            closeLabel.text = WorkbenchText.Close;

            weaponsTab.onClick.AddListener(() => TabSelected?.Invoke(false));
            suppliesTab.onClick.AddListener(() => TabSelected?.Invoke(true));
            close.onClick.AddListener(() => Closed?.Invoke());

            for (int i = 0; i < actions.Length; i++)
            {
                int index = i;
                actions[i].button.onClick.AddListener(() => ActionClicked?.Invoke(index));
            }
        }

        public void SetVisible(bool visible) => gameObject.SetActive(visible);

        public void SetCatalog(bool supplies, int count, int metal)
        {
            weaponsTab.interactable = supplies;
            suppliesTab.interactable = !supplies;
            balance.text = string.Format(WorkbenchText.Balance, metal);

            while (_rows.Count < count)
            {
                int index = _rows.Count;
                WorkbenchRowView row = Instantiate(rowTemplate, rowTemplate.transform.parent);
                row.Button.onClick.AddListener(() => ItemSelected?.Invoke(index));
                _rows.Add(row);
            }

            for (int i = 0; i < _rows.Count; i++)
                _rows[i].gameObject.SetActive(i < count);
        }

        public void SetItem(int index, string label, string detail, Sprite icon, bool selected) =>
            _rows[index].Display(label, detail, icon, selected);

        public void SetDetail(string group, string name, Sprite icon, string description, bool showStats)
        {
            category.text = group;
            title.text = name;
            preview.sprite = icon;
            details.text = description;

            statsPanel.SetActive(showStats);
        }

        public void SetStat(int index, string value, float fraction)
        {
            stats[index].value.text = value;
            stats[index].fill.anchorMax = new Vector2(Mathf.Clamp01(fraction), 1f);
        }

        public void SetAction(int index, string caption, string status, bool available)
        {
            ActionRow row = actions[index];
            row.button.gameObject.SetActive(true);
            row.button.interactable = available;

            row.label.text = caption;
            row.status.text = status;
        }

        public void SetActionCount(int count)
        {
            for (int i = count; i < actions.Length; i++)
                actions[i].button.gameObject.SetActive(false);
        }
    }
}
