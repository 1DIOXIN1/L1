using _Project.Develop.Runtime.UI.Core;
using _Project.Develop.Runtime.Utilities.InputManagement;
using UnityEngine;

namespace _Project.Develop.Runtime.UI.Gameplay.Phone
{
    public class PhonePresenter : IPresenter
    {
        private readonly PhoneView _view;
        private readonly IInputService _input;
        private readonly SettingsPresenter _settingsPresenter;
        private readonly MapPresenter _mapPresenter;
        private readonly QuestsPresenter _questsPresenter;

        private bool _isOpen;

        public PhonePresenter(
            PhoneView view,
            IInputService input,
            SettingsPresenter settingsPresenter,
            MapPresenter mapPresenter,
            QuestsPresenter questsPresenter)
        {
            _view = view;
            _input = input;
            _settingsPresenter = settingsPresenter;
            _mapPresenter = mapPresenter;
            _questsPresenter = questsPresenter;
        }

        public void Initialize()
        {
            _view.SetVisible(false);
            _view.ShowTab(PhoneTab.None);

            _settingsPresenter.Initialize();
            _mapPresenter.Initialize();
            _questsPresenter.Initialize();

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

            _settingsPresenter.Dispose();
            _mapPresenter.Dispose();
            _questsPresenter.Dispose();

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
            _settingsPresenter.FlushSave();
            _view.ShowTab(PhoneTab.None);
            _view.SetVisible(false);
            _input.SetContext(InputContext.Gameplay);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnSettingsClicked()
        {
            _view.ShowTab(PhoneTab.Settings);
            _settingsPresenter.Show();
        }

        private void OnMapClicked()
        {
            _view.ShowTab(PhoneTab.Map);
            _mapPresenter.Show();
        }

        private void OnQuestsClicked()
        {
            _view.ShowTab(PhoneTab.Quests);
            _questsPresenter.Show();
        }

        private void OnMessageClicked()
        {
            _view.ShowTab(PhoneTab.Message);
        }
    }
}
