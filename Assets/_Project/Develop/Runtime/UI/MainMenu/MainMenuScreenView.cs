using System;
using _Project.Develop.Runtime.UI.CommonViews;
using _Project.Develop.Runtime.UI.Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Develop.Runtime.UI.MainMenu
{
    public class MainMenuScreenView : MonoBehaviour, IView
    {
        public event Action OpenTestPopupButtonClicked;
        
        [field: SerializeField] public IconTextListView  WalletView { get; private set; }
        
        [SerializeField] private Button _openTestPopupButton;

        private void OnEnable()
        {
            _openTestPopupButton.onClick.AddListener(OnOpenTestPopupButtonClicked);
        }

        private void OnDisable()
        {
            _openTestPopupButton.onClick.RemoveListener(OnOpenTestPopupButtonClicked);
        }
        
        private void OnOpenTestPopupButtonClicked() => OpenTestPopupButtonClicked?.Invoke();
        
    }
}