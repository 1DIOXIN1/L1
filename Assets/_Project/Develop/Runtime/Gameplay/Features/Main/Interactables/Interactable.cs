using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Interactables
{
    public abstract class Interactable : MonoBehaviour, IInteractable
    {
        [SerializeField] private Transform hintAnchor;
        [SerializeField] private int priority;

        private InteractionService _interactions;

        public Transform HintAnchor => hintAnchor != null ? hintAnchor : transform;
        public Transform HierarchyRoot => transform;
        public int Priority => priority;
        public bool IsAvailable => isActiveAndEnabled;

        public virtual void Construct(InteractionSetup setup)
        {
            _interactions = setup.Interactions;
            _interactions.Register(this);
        }

        public abstract bool CanInteract();

        public abstract void Interact();

        private void OnDestroy()
        {
            _interactions?.Unregister(this);
        }
    }
}
