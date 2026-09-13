using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Interactables
{
    public abstract class Interactable : MonoBehaviour, IInteractable
    {
        [SerializeField] private Transform hintAnchor;
        [SerializeField] private int priority;

        public Transform HintAnchor => hintAnchor != null ? hintAnchor : transform;
        public Transform HierarchyRoot => transform;
        public int Priority => priority;
        public bool IsAvailable => isActiveAndEnabled;

        public virtual void Construct(InteractionSetup setup)
        {
        }

        public abstract bool CanInteract();

        public abstract void Interact();
    }
}
