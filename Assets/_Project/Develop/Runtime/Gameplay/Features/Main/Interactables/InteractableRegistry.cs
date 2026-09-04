using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Interactables
{
    public sealed class InteractableRegistry : MonoBehaviour
    {
        [SerializeField] private Interactable[] interactables = Array.Empty<Interactable>();

        public IReadOnlyList<Interactable> Interactables => interactables;

        private void OnValidate()
        {
            if (interactables == null || interactables.Length == 0)
                interactables = GetComponentsInChildren<Interactable>(true);
        }
    }
}
