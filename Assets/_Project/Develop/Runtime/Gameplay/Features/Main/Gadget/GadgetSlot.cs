using _Project.Develop.Runtime.Gameplay.Features.Main.Gadget.GadgetsType;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Gadget
{
    public class GadgetSlot
    {
        private IGadget _gadget;
        private SlotGadgetType _slotType;

        public GadgetSlot(IGadget gadget, SlotGadgetType slotType)
        {
            _gadget = gadget;
            _slotType = slotType;
        }

        public IGadget Gadget => _gadget;
        public SlotGadgetType SlotType => _slotType;

        public void SetGadget(IGadget gadget, SlotGadgetType slotType)
        {
            _gadget = gadget;
            _slotType = slotType;
        }

        public void Clear()
        {
            _gadget = null;
            _slotType = SlotGadgetType.None;
        }
    }
}
