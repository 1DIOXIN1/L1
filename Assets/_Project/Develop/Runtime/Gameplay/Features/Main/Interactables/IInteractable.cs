using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Interactables
{
    public interface IInteractable
    {
        Transform HintAnchor { get; }
        Transform HierarchyRoot { get; }
        int Priority { get; }
        bool IsAvailable { get; }

        bool CanInteract();

        void Interact();
    }
}
