using System;
using _Project.Develop.Runtime.UI.CommonViews;
using _Project.Develop.Runtime.UI.Core;
using _Project.Develop.Runtime.UI.Progress;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Develop.Runtime.UI.MainMenu
{
    public class MainMenuScreenView : MonoBehaviour, IView
    {
        public event Action ResetProgressButtonClicked;
        
        [field: SerializeField] public IconTextListView WalletView { get; private set; }
        [field: SerializeField] public ProgressItemListView ProgressItemlistView { get; private set; }
        
        [SerializeField] private Button _resetProgressButton;

        private void OnEnable()
        {
            _resetProgressButton.onClick.AddListener(OnResetProgressButtonClicked);
        }

        private void OnDisable()
        {
            _resetProgressButton.onClick.RemoveListener(OnResetProgressButtonClicked);
        }
        
        private void OnResetProgressButtonClicked() => ResetProgressButtonClicked?.Invoke();
        
    }
}