using UnityEngine;

namespace _Project.Develop.Runtime.UI.Gameplay.Arsenal
{
    [CreateAssetMenu(
        menuName = "Configs/UI/WeaponArsenalVisualConfig",
        fileName = "WeaponArsenalVisualConfig")]
    public sealed class WeaponArsenalVisualConfig : ScriptableObject
    {
        [SerializeField] private float wheelSize = 420f;
        [SerializeField] private float iconDistance = 130f;
        [SerializeField] private float iconSize = 64f;
        [SerializeField] private float centerSize = 160f;
        [SerializeField] private float deadZonePixels = 48f;
        [SerializeField] private Sprite segmentSprite;
        [SerializeField] private Color idleSegmentColor = new(0.22f, 0.26f, 0.32f, 0.8f);
        [SerializeField] private Color hoverSegmentColor = new(0.75f, 0.78f, 0.82f, 0.92f);
        [SerializeField] private Color equippedSegmentColor = new(0.35f, 0.4f, 0.48f, 0.85f);
        [SerializeField] private Color centerBackgroundColor = new(0.12f, 0.14f, 0.18f, 0.95f);

        public float WheelSize => wheelSize;
        public float IconDistance => iconDistance;
        public float IconSize => iconSize;
        public float CenterSize => centerSize;
        public float DeadZonePixels => deadZonePixels;
        public Sprite SegmentSprite => segmentSprite;
        public Color IdleSegmentColor => idleSegmentColor;
        public Color HoverSegmentColor => hoverSegmentColor;
        public Color EquippedSegmentColor => equippedSegmentColor;
        public Color CenterBackgroundColor => centerBackgroundColor;
    }
}
