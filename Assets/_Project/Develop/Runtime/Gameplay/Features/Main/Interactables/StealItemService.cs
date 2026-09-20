using System;
using System.Collections.Generic;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Interactables
{
    public sealed class StealItemService
    {
        private readonly HashSet<string> _activeTargets = new();
        private readonly HashSet<string> _stolenItemIds = new();

        public event Action<string> ItemStolen;

        public void Activate(string itemId) => _activeTargets.Add(itemId);

        public void Deactivate(string itemId) => _activeTargets.Remove(itemId);

        public bool IsActive(string itemId) => _activeTargets.Contains(itemId);

        public bool WasStolen(string itemId) => _stolenItemIds.Contains(itemId);

        public void NotifyStolen(string itemId)
        {
            if (_stolenItemIds.Add(itemId) == false)
                return;

            ItemStolen?.Invoke(itemId);
        }
    }
}
