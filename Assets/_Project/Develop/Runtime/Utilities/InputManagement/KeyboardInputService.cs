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

        protected override void UpdateLogic(float deltaTime)
        {
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