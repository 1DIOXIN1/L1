using UnityEngine;
using UnityEngine.UI;

namespace _Project.Develop.Runtime.UI.Gameplay.Workbench
{
    public sealed class WorkbenchRowView : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image icon;
        [SerializeField] private Text title;
        [SerializeField] private Text subtitle;
        [SerializeField] private GameObject selection;

        public Button Button => button;

        public void Display(string label, string detail, Sprite sprite, bool selected)
        {
            title.text = label;
            subtitle.text = detail;
            icon.sprite = sprite;
            selection.SetActive(selected);
        }
    }
}
