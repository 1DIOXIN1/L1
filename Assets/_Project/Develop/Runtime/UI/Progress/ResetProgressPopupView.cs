using System;
using _Project.Develop.Runtime.UI.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Develop.Runtime.UI.Progress
{
    public class ResetProgressPopupView : PopupViewBase
    {
        public event Action AgreedButtonClicked;
        public event Action CancelButtonClicked;
        
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Button _agreeButton;
        [SerializeField] private Button _cancelButton;

        public void OnEnable()
        {
            _agreeButton.onClick.AddListener(OnAgreedButtonClicked);
            _cancelButton.onClick.AddListener(OnCancelButtonClicked);
        }

        public void OnDisable()
        {
            _agreeButton.onClick.RemoveListener(OnAgreedButtonClicked);
            _cancelButton.onClick.RemoveListener(OnCancelButtonClicked);
        }
        
        public void OnAgreedButtonClicked() => AgreedButtonClicked?.Invoke();
        
        public void OnCancelButtonClicked() => CancelButtonClicked?.Invoke();
        
        public void SetText(string text) => _text.text = text;
    }
}