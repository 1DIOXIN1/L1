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
        event Action Reload;
        event Action UseGadget;
        event Action SelectSecondarySlot;
        event Action SelectPrimarySlot;
        event Action Jump;
        event Action Crouch;

        bool IsShootHeld { get; }
        bool IsSprintHeld { get; }
        InputContext CurrentContext { get; }

        void Update(float deltaTime);
        void SetContext(InputContext context);
    }
}
