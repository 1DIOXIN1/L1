using System;
using System.Collections.Generic;
using _Project.Develop.Runtime.Configs.Core.Gameplay;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter;
using _Project.Develop.Runtime.Utilities.InputManagement;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Interactables
{
    public sealed class InteractionService
    {
        private readonly IInputService _input;
        private readonly InteractionConfig _config;
        private readonly List<Interactable> _interactables = new();

        private Player _player;
        private Interactable _currentFocus;

        public event Action<Interactable> FocusChanged;

        public InteractionService(IInputService input, InteractionConfig config)
        {
            _input = input;
            _config = config;
            _input.InteractPressed += OnInteractPressed;
        }

        public Interactable CurrentFocus => _currentFocus;

        public void BindPlayer(Player player)
        {
            _player = player;
            ClearFocus();
        }

        public void Register(Interactable interactable)
        {
            if (interactable == null || _interactables.Contains(interactable))
                return;

            _interactables.Add(interactable);
        }

        public void RegisterFrom(InteractableRegistry registry)
        {
            if (registry == null)
                return;

            IReadOnlyList<Interactable> interactables = registry.Interactables;
            for (int i = 0; i < interactables.Count; i++)
                Register(interactables[i]);
        }

        public void Tick()
        {
            UpdateFocus();
        }

        private void UpdateFocus()
        {
            Interactable best = FindBestInteractable();
            SetFocus(best);
        }

        private Interactable FindBestInteractable()
        {
            if (_player == null || _player.IsControlLocked || _config == null)
                return null;

            Camera lookCamera = _player.LookCamera;
            if (lookCamera == null || lookCamera.isActiveAndEnabled == false)
                return null;

            Transform cameraTransform = lookCamera.transform;
            Vector3 origin = cameraTransform.position;
            float maxDistanceSqr = _config.MaxDistanceSqr;
            float maxScreenRadiusSqr = _config.MaxScreenRadiusSqr;

            Interactable best = null;
            float bestScreenDistSqr = float.MaxValue;
            float bestDistanceSqr = float.MaxValue;
            int bestPriority = int.MinValue;

            for (int i = 0; i < _interactables.Count; i++)
            {
                Interactable interactable = _interactables[i];
                if (interactable == null || interactable.isActiveAndEnabled == false)
                    continue;

                if (interactable.CanInteract() == false)
                    continue;

                Vector3 target = interactable.PromptAnchor.position;
                Vector3 toTarget = target - origin;
                float distanceSqr = toTarget.sqrMagnitude;
                if (distanceSqr > maxDistanceSqr || distanceSqr < 0.0001f)
                    continue;

                Vector3 viewport = lookCamera.WorldToViewportPoint(target);
                if (viewport.z <= 0f)
                    continue;

                float screenDistSqr = new Vector2(viewport.x - 0.5f, viewport.y - 0.5f).sqrMagnitude;
                if (screenDistSqr > maxScreenRadiusSqr)
                    continue;

                if (_config.CheckOcclusion && IsOccluded(origin, target, interactable))
                    continue;

                int priority = interactable.Priority;
                if (IsBetterCandidate(
                        priority,
                        screenDistSqr,
                        distanceSqr,
                        bestPriority,
                        bestScreenDistSqr,
                        bestDistanceSqr) == false)
                {
                    continue;
                }

                best = interactable;
                bestScreenDistSqr = screenDistSqr;
                bestDistanceSqr = distanceSqr;
                bestPriority = priority;
            }

            return best;
        }

        private static bool IsBetterCandidate(
            int priority,
            float screenDistSqr,
            float distanceSqr,
            int bestPriority,
            float bestScreenDistSqr,
            float bestDistanceSqr)
        {
            if (priority != bestPriority)
                return priority > bestPriority;

            const float screenEpsilon = 0.0001f;
            if (screenDistSqr < bestScreenDistSqr - screenEpsilon)
                return true;

            if (screenDistSqr > bestScreenDistSqr + screenEpsilon)
                return false;

            return distanceSqr < bestDistanceSqr;
        }

        private bool IsOccluded(Vector3 origin, Vector3 target, Interactable interactable)
        {
            if (Physics.Linecast(
                    origin,
                    target,
                    out RaycastHit hit,
                    _config.OcclusionMask,
                    QueryTriggerInteraction.Ignore) == false)
            {
                return false;
            }

            Transform hitTransform = hit.transform;
            if (BelongsToHierarchy(_player.transform, hitTransform))
                return false;

            if (BelongsToHierarchy(interactable.transform, hitTransform))
                return false;

            return true;
        }

        private static bool BelongsToHierarchy(Transform root, Transform hit)
        {
            if (root == null || hit == null)
                return false;

            return hit == root
                   || hit.IsChildOf(root)
                   || root.IsChildOf(hit);
        }

        private void SetFocus(Interactable focus)
        {
            if (_currentFocus == focus)
                return;

            _currentFocus = focus;
            FocusChanged?.Invoke(_currentFocus);
        }

        private void ClearFocus()
        {
            SetFocus(null);
        }

        private void OnInteractPressed()
        {
            if (_player == null || _player.IsControlLocked)
                return;

            if (_currentFocus == null || _currentFocus.CanInteract() == false)
                return;

            _currentFocus.Interact();
        }
    }
}
