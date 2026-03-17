using System;

namespace _Project.Develop.Runtime.Utilities.InputManagement
{
    public interface IInputService
    {
        event Action SelectFirstMode;
        event Action SelectSecondMode;
        event Action ConfirmPressed;
        event Action ResetPressed;
        event Action<char> CharEntered;
        void Update(float deltaTime);
    }
}