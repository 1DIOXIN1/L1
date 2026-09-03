using System;
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
        public event Action<Vector3> Move;
        public event Action Shoot;
        public event Action Reload;
        public event Action UseGadget;
        public event Action SelectPrimarySlot;
        public event Action SelectSecondarySlot;
        public event Action Jump;
        public event Action Crouch;

        public bool IsShootHeld { get; private set; }
        public bool IsSprintHeld { get; private set; }
        public InputContext CurrentContext => _context;

        private InputContext _context = InputContext.Menu;

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
            }
        }

        private void ProcessCutsceneInput()
        {
            IsShootHeld = false;
            IsSprintHeld = false;

            if (Input.GetKeyDown(KeyCode.Space) ||
                Input.GetKeyDown(KeyCode.Return) ||
                Input.GetKeyDown(KeyCode.KeypadEnter) ||
                Input.GetKeyDown(KeyCode.Escape))
            {
                ConfirmPressed?.Invoke();
            }
        }

        private void ProcessMenuInput()
        {
            IsShootHeld = false;
            IsSprintHeld = false;

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

            if (Input.GetKeyDown(KeyCode.Mouse0))
                Shoot?.Invoke();

            if (Input.GetKeyDown(KeyCode.R))
                Reload?.Invoke();

            if (Input.GetKeyDown(KeyCode.G))
                UseGadget?.Invoke();

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                ConfirmPressed?.Invoke();
        }
    }
}
