using _Project.Develop.Runtime.Configs.Meta.Weapon;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Weapon
{
    public class WeaponSlot
    {
        private IWeapon _weapon;
        private SlotWeaponType _slotType;

        public WeaponSlot(IWeapon weapon, SlotWeaponType slotType)
        {
            _weapon = weapon;
            _slotType = slotType;
        }
        
        public IWeapon Weapon => _weapon;
        public SlotWeaponType SlotType => _slotType;
        
        public void SetWeapon(IWeapon weapon, SlotWeaponType slotType)
        {
            _slotType = slotType;
            _weapon= weapon;
        }

        public void Clear()
        {
            _weapon = null;
            _slotType = SlotWeaponType.None;
        }
    }
}