using System;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Interactables
{
    public sealed class WorkbenchInteractable : Interactable
    {
        public event Action OpenRequested;

        public override bool CanInteract() => OpenRequested != null;

        public override void Interact() => OpenRequested?.Invoke();
    }
}
