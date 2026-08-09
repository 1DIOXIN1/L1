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
        public event Action StartGameButtonClicked;
        public event Action ExitGameButtonClicked;

        [field: SerializeField] public IconTextListView WalletView { get; private set; }
        [field: SerializeField] public ProgressItemListView ProgressItemlistView { get; private set; }

        [SerializeField] private Button _resetProgressButton;
        [SerializeField] private Button _startGameButton;
        [SerializeField] private Button _exitGameButton;

        private void OnEnable()
        {
            if (_resetProgressButton != null)
                _resetProgressButton.onClick.AddListener(OnResetProgressButtonClicked);

            if (_startGameButton != null)
                _startGameButton.onClick.AddListener(OnStartGameButtonClicked);

            if (_exitGameButton != null)
                _exitGameButton.onClick.AddListener(OnExitGameButtonClicked);
        }

        private void OnDisable()
        {
            if (_resetProgressButton != null)
                _resetProgressButton.onClick.RemoveListener(OnResetProgressButtonClicked);

            if (_startGameButton != null)
                _startGameButton.onClick.RemoveListener(OnStartGameButtonClicked);

            if (_exitGameButton != null)
                _exitGameButton.onClick.RemoveListener(OnExitGameButtonClicked);
        }

        private void OnResetProgressButtonClicked() => ResetProgressButtonClicked?.Invoke();

        private void OnStartGameButtonClicked() => StartGameButtonClicked?.Invoke();

        private void OnExitGameButtonClicked() => ExitGameButtonClicked?.Invoke();
    }
}
