using System;
using _Project.Develop.Runtime.Meta.Features.Settings;
using UnityEngine;

namespace _Project.Develop.Runtime.Utilities.InputManagement
{
    public class KeyboardInputService : Controller, IInputService
    {
        public event Action ResetPressed;
        public event Action<char> CharEntered;
        public event Action SelectFirstMode;
        public event Action SelectSecondMode;
        public event Action ConfirmPressed;
        public event Action InteractPressed;
        public event Action<Vector3> Move;
        public event Action Shoot;
        public event Action Reload;
        public event Action UseGadget;
        public event Action SelectPrimarySlot;
        public event Action SelectSecondarySlot;
        public event Action Jump;
        public event Action Crouch;
        public event Action PhonePressed;

        public bool IsShootHeld { get; private set; }
        public bool IsAimHeld { get; private set; }
        public bool IsSprintHeld { get; private set; }
        public Vector2 LookDelta { get; private set; }
        public InputContext CurrentContext => _context;

        private readonly SettingsService _settings;
        private InputContext _context = InputContext.Menu;

        public KeyboardInputService(SettingsService settings)
        {
            _settings = settings;
        }

        public void SetContext(InputContext context) => _context = context;

        protected override void UpdateLogic(float deltaTime)
        {
            switch (_context)
            {
                case InputContext.Menu:
                    ProcessMenuInput();
                    break;
                case InputContext.Gameplay:
                    ProcessGameplayInput();
                    break;
                case InputContext.Cutscene:
                    ProcessCutsceneInput();
                    break;
                case InputContext.Phone:
                    ProcessPhoneInput();
                    break;
                default:
                    ClearGameplayAxes();
                    break;
            }
        }

        private void ProcessCutsceneInput()
        {
            ClearGameplayAxes();

            if (Input.GetKeyDown(KeyCode.Space) ||
                Input.GetKeyDown(KeyCode.Return) ||
                Input.GetKeyDown(KeyCode.KeypadEnter) ||
                Input.GetKeyDown(KeyCode.Escape))
            {
                ConfirmPressed?.Invoke();
            }
        }

        private void ProcessPhoneInput()
        {
            ClearGameplayAxes();

            if (Input.GetKeyDown(KeyCode.N))
                PhonePressed?.Invoke();
        }

        private void ProcessMenuInput()
        {
            ClearGameplayAxes();

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                ConfirmPressed?.Invoke();

            if (Input.GetKeyDown(KeyCode.Alpha1))
                SelectFirstMode?.Invoke();

            if (Input.GetKeyDown(KeyCode.Alpha2))
                SelectSecondMode?.Invoke();

            if (Input.GetKeyDown(KeyCode.R))
                ResetPressed?.Invoke();

            foreach (char c in Input.inputString)
                CharEntered?.Invoke(c);
        }

        private void ProcessGameplayInput()
        {
            float sensitivity = _settings != null ? _settings.MouseSensitivity : 1f;
            LookDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * sensitivity;

            Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            Move?.Invoke(direction);

            if (Input.GetKeyDown(KeyCode.LeftControl))
                Crouch?.Invoke();

            IsSprintHeld = Input.GetKey(KeyCode.LeftShift);

            if (Input.GetKeyDown(KeyCode.Space))
                Jump?.Invoke();

            if (Input.GetKeyDown(KeyCode.Alpha1))
                SelectPrimarySlot?.Invoke();

            if (Input.GetKeyDown(KeyCode.Alpha2))
                SelectSecondarySlot?.Invoke();

            IsShootHeld = Input.GetMouseButton(0);
            IsAimHeld = Input.GetMouseButton(1);

            if (Input.GetKeyDown(KeyCode.Mouse0))
                Shoot?.Invoke();

            if (Input.GetKeyDown(KeyCode.R))
                Reload?.Invoke();

            if (Input.GetKeyDown(KeyCode.G))
                UseGadget?.Invoke();

            if (Input.GetKeyDown(KeyCode.F))
                InteractPressed?.Invoke();

            if (Input.GetKeyDown(KeyCode.N))
                PhonePressed?.Invoke();

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                ConfirmPressed?.Invoke();
        }

        private void ClearGameplayAxes()
        {
            IsShootHeld = false;
            IsAimHeld = false;
            IsSprintHeld = false;
            LookDelta = Vector2.zero;
        }
    }
}
