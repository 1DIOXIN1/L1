using _Project.Develop.Runtime.UI.Core;
using TMPro;
using UnityEngine;

namespace _Project.Develop.Runtime.UI.Progress
{
    public class ProgressItemView : MonoBehaviour, IView
    {
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _valueText;

        public void SetName(string name) => _nameText.text = name;
        
        public void SetValueText(int value) => _valueText.text = value.ToString();
    }
}