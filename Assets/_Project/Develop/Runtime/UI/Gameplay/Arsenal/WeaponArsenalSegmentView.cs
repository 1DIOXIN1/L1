using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon;
using _Project.Develop.Runtime.UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Develop.Runtime.UI.Gameplay.Arsenal
{
    public sealed class WeaponArsenalSegmentView : MonoBehaviour, IView
    {
        [SerializeField] private Image background;
        [SerializeField] private Image icon;

        public Image Background => background;
        public Image Icon => icon;
        public IWeapon Weapon { get; set; }

        public void ApplyLayout(float fillAmount, float rotationDegrees, float iconDistance, float iconSize)
        {
            RectTransform root = (RectTransform)transform;
            root.localRotation = Quaternion.Euler(0f, 0f, -rotationDegrees);

            if (background != null)
            {
                background.type = Image.Type.Filled;
                background.fillMethod = Image.FillMethod.Radial360;
                background.fillOrigin = (int)Image.Origin360.Top;
                background.fillClockwise = true;
                background.fillAmount = fillAmount;
            }

            if (icon == null)
                return;

            RectTransform iconRect = icon.rectTransform;
            iconRect.sizeDelta = new Vector2(iconSize, iconSize);

            float halfStepDegrees = fillAmount * 180f;
            float midAngle = halfStepDegrees * Mathf.Deg2Rad;
            iconRect.anchoredPosition = new Vector2(Mathf.Sin(midAngle), Mathf.Cos(midAngle)) * iconDistance;
            iconRect.localRotation = Quaternion.Euler(0f, 0f, rotationDegrees);
        }
    }
}
