using System;
using UnityEngine;

namespace _Project.Develop.Runtime.Utilities.InputManagement
{
    public interface IInputService
    {
        event Action SelectFirstMode;
        event Action SelectSecondMode;
        event Action ConfirmPressed;
        event Action ResetPressed;
        event Action<char> CharEntered;
        event Action<Vector3> Move;
        event Action Shoot;
        event Action SelectSecondarySlot;
        event Action SelectPrimarySlot;
        event Action Jump;
        event Action Sprint;
        event Action Crouch;
        
        void Update(float deltaTime);
    }
}