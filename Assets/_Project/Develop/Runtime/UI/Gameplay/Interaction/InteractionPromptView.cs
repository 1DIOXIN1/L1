using _Project.Develop.Runtime.UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Develop.Runtime.UI.Gameplay.Interaction
{
    public sealed class InteractionPromptView : MonoBehaviour, IView
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private CanvasGroup canvasGroup;

        public void SetVisible(bool visible)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = visible ? 1f : 0f;
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;
                return;
            }

            gameObject.SetActive(visible);
        }

        public void SetWorldPose(Vector3 position, Quaternion rotation)
        {
            transform.SetPositionAndRotation(position, rotation);
        }

        public void SetIcon(Sprite sprite)
        {
            if (iconImage == null)
                return;

            iconImage.sprite = sprite;
            iconImage.enabled = sprite != null;
        }
    }
}
