using _Project.Develop.Runtime.UI.Core;
using TMPro;
using UnityEngine;

public class SequenceView : MonoBehaviour, IView
{
    [SerializeField] private TMP_Text _requiredSequenceText;
    [SerializeField] private TMP_Text _currentSequenceText;
    
    public void SetRequiredText(string requiredText) =>  _requiredSequenceText.text = requiredText;
    public void SetCurrentText(string currentText) =>  _currentSequenceText.text = currentText;
}
