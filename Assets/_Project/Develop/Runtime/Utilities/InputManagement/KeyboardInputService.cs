using System;
using Unity.VisualScripting;
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
        public event Action SelectPrimarySlot;
        public event Action SelectSecondarySlot;
        public event Action Jump;
        public event Action Sprint;
        public event Action Crouch;
        
        private Vector3 _direction;
        
        protected override void UpdateLogic(float deltaTime)
        {
            _direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            
            Move?.Invoke(_direction);

            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                Crouch?.Invoke();
            }
            
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                Sprint?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SelectPrimarySlot?.Invoke();
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SelectSecondarySlot?.Invoke();
            }
            
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Shoot?.Invoke();
            }
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ConfirmPressed?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SelectFirstMode?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SelectSecondMode?.Invoke();
            }
            
            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetPressed?.Invoke();
            }

            foreach (char c in Input.inputString)
            {
                CharEntered?.Invoke(c);
            }
        }
    }
}
