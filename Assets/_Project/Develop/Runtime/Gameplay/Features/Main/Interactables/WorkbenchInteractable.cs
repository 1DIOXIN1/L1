using System;
using _Project.Develop.Runtime.UI.Gameplay.Workbench;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Interactables
{
    public sealed class WorkbenchInteractable : Interactable
    {
        [SerializeField] private WorkbenchView monitorView;
        [SerializeField] private Transform monitorCameraPose;
        [SerializeField] private Animator fabricator;

        public WorkbenchView MonitorView => monitorView;
        public Transform MonitorCameraPose => monitorCameraPose;

        public event Action OpenRequested;

        public void PlayFabrication()
        {
            if (fabricator != null)
                fabricator.SetTrigger("Craft");
        }

        public override bool CanInteract() => OpenRequested != null;

        public override void Interact() => OpenRequested?.Invoke();
    }
}
