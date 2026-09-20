using _Project.Develop.Runtime.UI.Core;
using TMPro;
using UnityEngine;

namespace _Project.Develop.Runtime.UI.Gameplay.Phone
{
    public class QuestsObjectiveItemView : MonoBehaviour
    {
        [SerializeField] private TMP_Text descriptionText;

        public void Set(string description, bool isCompleted)
        {
            descriptionText.text = description;
            descriptionText.fontStyle = isCompleted
                ? FontStyles.Strikethrough
                : FontStyles.Normal;

            Color color = descriptionText.color;
            color.a = isCompleted ? 0.55f : 1f;
            descriptionText.color = color;
        }
    }
}
