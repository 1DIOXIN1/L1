using _Project.Develop.Runtime.UI.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Develop.Runtime.UI.Gameplay
{
    public class GameplayScreenView : MonoBehaviour, IView
    {
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Slider staminaSlider;
        [SerializeField] private TMP_Text currentAmmoText;
        [SerializeField] private TMP_Text maxAmmoText;
        [SerializeField] private TMP_Text weaponNameText;
        [SerializeField] private Image weaponIconImage;
        [SerializeField] private GameObject sequenceViewRoot;

        private void Awake()
        {
            if (sequenceViewRoot != null)
                sequenceViewRoot.SetActive(false);
        }

        public void SetHealth(float normalized)
        {
            if (healthSlider != null)
                healthSlider.value = Mathf.Clamp01(normalized);
        }

        public void SetStamina(float normalized)
        {
            if (staminaSlider != null)
                staminaSlider.value = Mathf.Clamp01(normalized);
        }

        public void SetAmmo(int currentMagazine, int reserveAmmo)
        {
            if (currentAmmoText != null)
                currentAmmoText.text = currentMagazine.ToString();

            if (maxAmmoText != null)
                maxAmmoText.text = reserveAmmo.ToString();
        }

        public void SetWeaponName(string weaponName)
        {
            if (weaponNameText != null)
                weaponNameText.text = weaponName;
        }

        public void SetWeaponIcon(Sprite icon)
        {
            if (weaponIconImage == null)
                return;

            weaponIconImage.sprite = icon;
            weaponIconImage.enabled = icon != null;
        }
    }
}
