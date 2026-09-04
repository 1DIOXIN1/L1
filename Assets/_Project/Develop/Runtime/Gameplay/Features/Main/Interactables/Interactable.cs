using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Interactables
{
    public abstract class Interactable : MonoBehaviour
    {
        [SerializeField] private Transform promptAnchor;
        [SerializeField] private int priority;

        public Transform PromptAnchor => promptAnchor != null ? promptAnchor : transform;
        public int Priority => priority;

        public virtual void Construct(InteractionSetup setup)
        {
        }

        public abstract bool CanInteract();

        public abstract void Interact();
    }
}
