using System.Collections.Generic;
using _Project.Develop.Runtime.Gameplay.Features.Main.Gadget.GadgetsType;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Gadget
{
    public class GadgetInventory
    {
        private SlotGadgetType? _currentSlot;
        private IGadget _currentGadget;

        private readonly Dictionary<SlotGadgetType, GadgetSlot> _slots;

        public GadgetInventory(Dictionary<SlotGadgetType, GadgetSlot> gadgetSlots)
        {
            _slots = new Dictionary<SlotGadgetType, GadgetSlot>(gadgetSlots);
        }

        public IGadget CurrentGadget => _currentGadget;

        public void EquipGadget(SlotGadgetType slotType)
        {
            if (!_slots.TryGetValue(slotType, out GadgetSlot slot))
                return;

            if (slot.Gadget == null)
                return;

            _currentSlot = slot.SlotType;
            _currentGadget = slot.Gadget;
        }

        public void UseCurrentGadget()
        {
            if (_currentGadget == null)
                return;

            _currentGadget.Use();
        }

        public void RemoveGadgetOutInventory(SlotGadgetType slotType)
        {
            if (!_slots.TryGetValue(slotType, out GadgetSlot slot))
                return;

            slot.Clear();

            if (_currentSlot.HasValue && _currentSlot.Value == slotType)
            {
                _currentSlot = null;
                _currentGadget = null;
            }
        }
    }
}
