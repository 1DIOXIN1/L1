using System;
using _Project.Develop.Runtime.Gameplay.Features.Main.Interactables;

namespace _Project.Develop.Runtime.Gameplay.Infrastructure.Mission
{
    public class StealItemObjective : IMissionObjective
    {
        private readonly string _itemId;
        private readonly StealItemService _stealItemService;

        public StealItemObjective(string itemId, StealItemService stealItemService)
        {
            if (string.IsNullOrEmpty(itemId))
                throw new ArgumentException("Steal item id is required.", nameof(itemId));

            _itemId = itemId;
            _stealItemService = stealItemService ?? throw new ArgumentNullException(nameof(stealItemService));
        }

        public bool IsComplete { get; private set; }
        public event Action Completed;

        public void Start()
        {
            _stealItemService.Activate(_itemId);

            if (_stealItemService.WasStolen(_itemId))
            {
                Complete();
                return;
            }

            _stealItemService.ItemStolen += OnItemStolen;
        }

        public void Stop()
        {
            _stealItemService.ItemStolen -= OnItemStolen;
            _stealItemService.Deactivate(_itemId);
        }

        private void OnItemStolen(string stolenItemId)
        {
            if (stolenItemId != _itemId)
                return;

            _stealItemService.ItemStolen -= OnItemStolen;
            Complete();
        }

        private void Complete()
        {
            if (IsComplete)
                return;

            IsComplete = true;
            Completed?.Invoke();
        }
    }
}
