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

        [Serializable]
        private class SupplyArtwork
        {
            public SupplyType type;
            public Sprite icon;
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
        [SerializeField] private Material hologramMaterial;
        [SerializeField] private Material monitorIconMaterial;
        [SerializeField] private CanvasGroup monitorControls;
        [SerializeField] private SupplyArtwork[] supplyArtwork = Array.Empty<SupplyArtwork>();

        [SerializeField] private GameObject statsPanel;
        [SerializeField] private StatRow[] stats;
        [SerializeField] private ActionRow[] actions;

        private readonly List<WorkbenchRowView> _rows = new();
        private Vector2 _previewRestPosition;
        private Color _previewTint;
        private bool _showSupplies;

        public event Action<bool> TabSelected;
        public event Action<int> ItemSelected;
        public event Action<int> ActionClicked;
        public event Action Closed;

        private void Awake()
        {
            _previewRestPosition = preview.rectTransform.anchoredPosition;
            _previewTint = preview.color;
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

        public void SetInteractive(bool interactive)
        {
            if (monitorControls != null)
            {
                monitorControls.interactable = interactive;
                monitorControls.blocksRaycasts = interactive;
            }
        }

        private void Update()
        {
            if (hologramMaterial == null || preview.material != hologramMaterial)
                return;

            float phase = Time.unscaledTime;
            RectTransform image = preview.rectTransform;
            image.anchoredPosition = _previewRestPosition + Vector2.up * (4f * Mathf.Sin(phase * 1.45f));
            image.localRotation = Quaternion.Euler(0f, 0f, 1.8f * Mathf.Sin(phase * 0.82f));
        }

        public void SetCatalog(bool supplies, int count, int metal)
        {
            _showSupplies = supplies;
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

        public Sprite GetSupplyIcon(WorkbenchSupply supply)
        {
            foreach (SupplyArtwork art in supplyArtwork)
                if (art.type == supply.type && art.icon != null)
                    return art.icon;

            return supply.icon;
        }

        public void SetItem(int index, string label, string detail, Sprite icon, bool selected)
        {
            WorkbenchRowView row = _rows[index];
            row.Display(label, detail, icon, selected);
            if (monitorIconMaterial != null)
                row.SetIconStyle(_showSupplies ? null : monitorIconMaterial, _showSupplies ? Color.white : rowTemplate.IconTint);
        }

        public void SetDetail(string group, string name, Sprite icon, string description, bool showStats)
        {
            category.text = group;
            title.text = name;
            preview.sprite = icon;
            if (hologramMaterial != null)
            {
                preview.material = showStats ? hologramMaterial : null;
                preview.color = showStats ? _previewTint : Color.white;
                preview.rectTransform.anchoredPosition = _previewRestPosition;
                preview.rectTransform.localRotation = Quaternion.identity;
            }
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
