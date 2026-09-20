using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Interactables
{
    public sealed class StealItemInteractable : Interactable
    {
        [SerializeField] private string itemId = "test_case";

        private StealItemService _stealItemService;
        private bool _stolen;

        public string ItemId => itemId;

        public override void Construct(InteractionSetup setup)
        {
            base.Construct(setup);
            _stealItemService = setup.StealItems;
        }

        public override bool CanInteract()
        {
            return _stolen == false && _stealItemService.IsActive(itemId);
        }

        public override void Interact()
        {
            if (CanInteract() == false)
                return;

            _stolen = true;
            _stealItemService.NotifyStolen(itemId);
            gameObject.SetActive(false);
        }
    }
}
