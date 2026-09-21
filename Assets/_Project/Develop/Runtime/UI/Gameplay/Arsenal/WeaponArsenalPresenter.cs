using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.WeaponsType;
using _Project.Develop.Runtime.Meta.Features.Player;
using _Project.Develop.Runtime.UI.Core;
using _Project.Develop.Runtime.Utilities.InputManagement;
using UnityEngine;

namespace _Project.Develop.Runtime.UI.Gameplay.Arsenal
{
    public sealed class WeaponArsenalPresenter : IPresenter
    {
        private readonly WeaponArsenalView _view;
        private readonly IInputService _input;
        private readonly PlayerStateService _playerStateService;

        private WeaponInventory _inventory;
        private bool _isOpen;
        private InputContext _contextBeforeOpen;

        public WeaponArsenalPresenter(
            WeaponArsenalView view,
            IInputService input,
            PlayerStateService playerStateService)
        {
            _view = view;
            _input = input;
            _playerStateService = playerStateService;
        }

        public void Initialize()
        {
            _view.SetVisible(false);
        }

        public void AttachInventory(WeaponInventory inventory)
        {
            _inventory = inventory;
        }

        public void Tick()
        {
            if (_inventory == null)
                return;

            if (_input.CurrentContext == InputContext.Phone ||
                _input.CurrentContext == InputContext.Cutscene ||
                _input.CurrentContext == InputContext.Menu)
            {
                if (_isOpen)
                    Close(false);
                return;
            }

            if (_isOpen == false)
            {
                if (_input.IsArsenalHeld && _input.CurrentContext == InputContext.Gameplay)
                    Open();
                return;
            }

            _view.UpdateHoverFromScreenPosition(Input.mousePosition);

            if (_input.IsArsenalHeld == false)
                Close(true);
        }

        public void Dispose()
        {
            if (_isOpen)
                Close(false);

            _inventory = null;
        }

        private void Open()
        {
            if (_inventory.Weapons.Count == 0)
                return;

            _isOpen = true;
            _contextBeforeOpen = _input.CurrentContext;
            _input.SetContext(InputContext.Arsenal);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            _view.Rebuild(_inventory.Weapons, _inventory.CurrentWeaponType);
            _view.SetVisible(true);
        }

        private void Close(bool applySelection)
        {
            if (_isOpen == false)
                return;

            if (applySelection && _view.HasHoveredWeapon)
            {
                WeaponType type = _view.HoveredWeapon.Type;
                _inventory.Equip(type);
                _playerStateService.SetSelectedWeaponType(type);
            }

            _view.SetVisible(false);
            _isOpen = false;

            _input.SetContext(_contextBeforeOpen == InputContext.Arsenal
                ? InputContext.Gameplay
                : _contextBeforeOpen);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
