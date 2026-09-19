using _Project.Develop.Runtime.UI.Core;
using _Project.Develop.Runtime.Utilities.InputManagement;
using UnityEngine;

namespace _Project.Develop.Runtime.UI.Gameplay.Phone
{
    public class PhonePresenter : IPresenter
    {
        private readonly PhoneView _view;
        private readonly IInputService _input;

        private bool _isOpen;

        public PhonePresenter(PhoneView view, IInputService input)
        {
            _view = view;
            _input = input;
        }

        public void Initialize()
        {
            _view.SetVisible(false);
            _view.ShowTab(PhoneTab.None);

            _input.PhonePressed += OnPhonePressed;
            _view.SettingsClicked += OnSettingsClicked;
            _view.MapClicked += OnMapClicked;
            _view.QuestsClicked += OnQuestsClicked;
            _view.MessageClicked += OnMessageClicked;
        }

        public void Dispose()
        {
            _input.PhonePressed -= OnPhonePressed;
            _view.SettingsClicked -= OnSettingsClicked;
            _view.MapClicked -= OnMapClicked;
            _view.QuestsClicked -= OnQuestsClicked;
            _view.MessageClicked -= OnMessageClicked;

            if (_isOpen)
                Close();
        }

        private void OnPhonePressed()
        {
            if (_isOpen)
                Close();
            else
                Open();
        }

        private void Open()
        {
            _isOpen = true;
            _view.SetVisible(true);
            _input.SetContext(InputContext.Phone);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void Close()
        {
            _isOpen = false;
            _view.ShowTab(PhoneTab.None);
            _view.SetVisible(false);
            _input.SetContext(InputContext.Gameplay);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnSettingsClicked()
        {
        }

        private void OnMapClicked()
        {
        }

        private void OnQuestsClicked()
        {
        }

        private void OnMessageClicked()
        {
        }
    }
}
