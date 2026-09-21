using System.Collections.Generic;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.WeaponsType;
using _Project.Develop.Runtime.UI.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Develop.Runtime.UI.Gameplay.Arsenal
{
    public sealed class WeaponArsenalView : MonoBehaviour, IView
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform wheel;
        [SerializeField] private RectTransform segmentsRoot;
        [SerializeField] private Image centerBackground;
        [SerializeField] private TMP_Text weaponNameText;
        [SerializeField] private TMP_Text ammoText;
        [SerializeField] private WeaponArsenalSegmentView segmentTemplate;
        [SerializeField] private WeaponArsenalVisualConfig visualConfig;

        private readonly List<WeaponArsenalSegmentView> _segments = new();
        private readonly List<IWeapon> _weapons = new();

        private int _hoveredIndex = -1;
        private WeaponType? _equippedType;

        public bool HasHoveredWeapon => _hoveredIndex >= 0 && _hoveredIndex < _weapons.Count;
        public IWeapon HoveredWeapon => _weapons[_hoveredIndex];

        private void Awake()
        {
            if (segmentTemplate != null)
                segmentTemplate.gameObject.SetActive(false);

            ApplyStaticVisuals();
            SetVisible(false);
        }

        public void SetVisible(bool visible)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = visible ? 1f : 0f;
                canvasGroup.blocksRaycasts = visible;
                canvasGroup.interactable = visible;
            }

            gameObject.SetActive(visible);
        }

        public void Rebuild(IReadOnlyList<IWeapon> weapons, WeaponType? equippedType)
        {
            ApplyStaticVisuals();

            _equippedType = equippedType;
            _weapons.Clear();
            for (int i = 0; i < weapons.Count; i++)
                _weapons.Add(weapons[i]);

            ClearSegments();

            int count = _weapons.Count;
            if (count == 0 || segmentTemplate == null || visualConfig == null)
            {
                SetHoveredIndex(-1);
                return;
            }

            float fill = 1f / count;
            float step = 360f / count;
            Sprite segmentSprite = visualConfig.SegmentSprite;

            for (int i = 0; i < count; i++)
            {
                WeaponArsenalSegmentView segment = Instantiate(segmentTemplate, segmentsRoot);
                segment.gameObject.SetActive(true);
                segment.name = $"Segment_{i}";
                segment.Weapon = _weapons[i];
                segment.ApplyLayout(fill, i * step, visualConfig.IconDistance, visualConfig.IconSize);

                if (segment.Background != null)
                {
                    if (segmentSprite != null)
                        segment.Background.sprite = segmentSprite;
                    segment.Background.color = visualConfig.IdleSegmentColor;
                }

                if (segment.Icon != null)
                {
                    segment.Icon.sprite = _weapons[i].HudIconActive;
                    segment.Icon.enabled = _weapons[i].HudIconActive != null;
                }

                _segments.Add(segment);
            }

            int initial = 0;
            if (equippedType.HasValue)
            {
                for (int i = 0; i < _weapons.Count; i++)
                {
                    if (_weapons[i].Type == equippedType.Value)
                    {
                        initial = i;
                        break;
                    }
                }
            }

            SetHoveredIndex(initial);
        }

        public void UpdateHoverFromScreenPosition(Vector2 screenPosition)
        {
            if (_weapons.Count == 0 || visualConfig == null)
            {
                SetHoveredIndex(-1);
                return;
            }

            Vector2 center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            Vector2 delta = screenPosition - center;
            float deadZone = visualConfig.DeadZonePixels;
            if (delta.sqrMagnitude < deadZone * deadZone)
                return;

            float angle = Mathf.Atan2(delta.x, delta.y) * Mathf.Rad2Deg;
            if (angle < 0f)
                angle += 360f;

            float step = 360f / _weapons.Count;
            int index = Mathf.FloorToInt(angle / step) % _weapons.Count;
            SetHoveredIndex(index);
        }

        private void ApplyStaticVisuals()
        {
            if (visualConfig == null)
                return;

            if (wheel != null)
                wheel.sizeDelta = new Vector2(visualConfig.WheelSize, visualConfig.WheelSize);

            if (centerBackground != null)
            {
                centerBackground.rectTransform.sizeDelta =
                    new Vector2(visualConfig.CenterSize, visualConfig.CenterSize);
                centerBackground.color = visualConfig.CenterBackgroundColor;
                if (visualConfig.SegmentSprite != null)
                    centerBackground.sprite = visualConfig.SegmentSprite;
            }
        }

        private void SetHoveredIndex(int index)
        {
            _hoveredIndex = index;
            RefreshVisuals();
            RefreshCenter();
        }

        private void RefreshVisuals()
        {
            if (visualConfig == null)
                return;

            for (int i = 0; i < _segments.Count; i++)
            {
                WeaponArsenalSegmentView segment = _segments[i];
                bool hovered = i == _hoveredIndex;
                bool equipped = _equippedType.HasValue &&
                                segment.Weapon != null &&
                                segment.Weapon.Type == _equippedType.Value;

                if (segment.Background != null)
                {
                    if (hovered)
                        segment.Background.color = visualConfig.HoverSegmentColor;
                    else if (equipped)
                        segment.Background.color = visualConfig.EquippedSegmentColor;
                    else
                        segment.Background.color = visualConfig.IdleSegmentColor;
                }
            }
        }

        private void RefreshCenter()
        {
            if (_hoveredIndex < 0 || _hoveredIndex >= _weapons.Count)
            {
                if (weaponNameText != null)
                    weaponNameText.text = string.Empty;
                if (ammoText != null)
                    ammoText.text = string.Empty;
                return;
            }

            IWeapon weapon = _weapons[_hoveredIndex];

            if (weaponNameText != null)
                weaponNameText.text = weapon.Type.ToString();
            if (ammoText != null)
                ammoText.text = weapon.Ammo.ToString();
        }

        private void ClearSegments()
        {
            for (int i = 0; i < _segments.Count; i++)
            {
                if (_segments[i] != null)
                    Destroy(_segments[i].gameObject);
            }

            _segments.Clear();
        }
    }
}
