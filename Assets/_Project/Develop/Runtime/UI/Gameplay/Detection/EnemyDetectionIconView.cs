using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Detection;
using _Project.Develop.Runtime.UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Develop.Runtime.UI.Gameplay.Detection
{
    public sealed class EnemyDetectionIconView : MonoBehaviour, IView
    {
        [SerializeField] private Image _calmImage;
        [SerializeField] private Image _excitedImage;
        [SerializeField] private Image _noticedImage;
        [SerializeField] private Image _lostImage;

        public void SetPhase(DetectionPhase phase, float meter)
        {
            bool showCalm = phase == DetectionPhase.Calm || phase == DetectionPhase.Suspicious;
            bool showExcited = phase == DetectionPhase.Suspicious;
            bool showNoticed = phase == DetectionPhase.Alerted;
            bool showLost = phase == DetectionPhase.Lost;

            SetImageActive(_calmImage, showCalm);
            SetImageActive(_excitedImage, showExcited);
            SetImageActive(_noticedImage, showNoticed);
            SetImageActive(_lostImage, showLost);

            if (showExcited && _excitedImage != null)
                _excitedImage.fillAmount = meter;
        }

        public void SetWorldPose(Vector3 position, Quaternion rotation)
        {
            transform.SetPositionAndRotation(position, rotation);
        }

        private static void SetImageActive(Image image, bool active)
        {
            if (image == null)
                return;

            image.enabled = active && image.sprite != null;
        }
    }
}
