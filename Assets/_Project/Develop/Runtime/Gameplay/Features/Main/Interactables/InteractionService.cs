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
        private readonly List<IInteractable> _interactables = new();

        private Player _player;
        private PlayerCamera _playerCamera;
        private IInteractable _currentFocus;

        public event Action<IInteractable> FocusChanged;

        public InteractionService(IInputService input, InteractionConfig config)
        {
            _input = input ?? throw new ArgumentNullException(nameof(input));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _input.InteractPressed += OnInteractPressed;
        }

        public IInteractable CurrentFocus => _currentFocus;

        public void BindPlayer(Player player, PlayerCamera playerCamera)
        {
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _playerCamera = playerCamera ?? throw new ArgumentNullException(nameof(playerCamera));
            ClearFocus();
        }

        public void Register(IInteractable interactable)
        {
            if (_interactables.Contains(interactable))
                return;

            _interactables.Add(interactable);
        }

        public void RegisterFrom(InteractableRegistry registry)
        {
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
            SetFocus(FindBestInteractable());
        }

        private IInteractable FindBestInteractable()
        {
            if (_player.IsControlLocked)
                return null;

            Camera lookCamera = _playerCamera.LookCamera;
            if (lookCamera == null || lookCamera.isActiveAndEnabled == false)
                return null;

            Transform cameraTransform = lookCamera.transform;
            Vector3 origin = cameraTransform.position;
            float maxDistanceSqr = _config.MaxDistanceSqr;
            float maxScreenRadiusSqr = _config.MaxScreenRadiusSqr;

            IInteractable best = null;
            float bestScreenDistSqr = float.MaxValue;
            float bestDistanceSqr = float.MaxValue;
            int bestPriority = int.MinValue;

            for (int i = 0; i < _interactables.Count; i++)
            {
                IInteractable interactable = _interactables[i];
                if (interactable.IsAvailable == false || interactable.CanInteract() == false)
                    continue;

                Vector3 target = interactable.HintAnchor.position;
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

                if (_config.CheckOcclusion && IsOccluded(origin, target, interactable.HierarchyRoot))
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

        private bool IsOccluded(Vector3 origin, Vector3 target, Transform hierarchyRoot)
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

            if (BelongsToHierarchy(hierarchyRoot, hitTransform))
                return false;

            return true;
        }

        private static bool BelongsToHierarchy(Transform root, Transform hit)
        {
            return hit == root
                   || hit.IsChildOf(root)
                   || root.IsChildOf(hit);
        }

        private void SetFocus(IInteractable focus)
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
            if (_player.IsControlLocked)
                return;

            if (_currentFocus == null || _currentFocus.CanInteract() == false)
                return;

            _currentFocus.Interact();
        }
    }
}
