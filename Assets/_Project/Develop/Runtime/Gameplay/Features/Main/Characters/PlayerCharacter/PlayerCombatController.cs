using _Project.Develop.Runtime.Configs.Meta.Weapon;
using _Project.Develop.Runtime.Gameplay.Features.Main.Gadget;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter
{
    public sealed class PlayerCombatController
    {
        private readonly WeaponInventory _weaponInventory;
        private readonly GadgetInventory _gadgetInventory;

        public PlayerCombatController(WeaponInventory weaponInventory, GadgetInventory gadgetInventory)
        {
            _weaponInventory = weaponInventory;
            _gadgetInventory = gadgetInventory;
        }

        public void OnShootPressed()
        {
            IWeapon weapon = _weaponInventory.CurrentWeapon;
            if (weapon != null && weapon.IsAutomatic == false)
                weapon.Shoot();
        }

        public void Reload()
        {
            _weaponInventory.CurrentWeapon?.Reload();
        }

        public void SelectPrimary()
        {
            _weaponInventory.EquipWeapon(SlotWeaponType.PrimarySlot);
        }

        public void SelectSecondary()
        {
            _weaponInventory.EquipWeapon(SlotWeaponType.SecondarySlot);
        }

        public void UseGadget()
        {
            _gadgetInventory.UseCurrentGadget();
        }

        public void Tick(float deltaTime, bool isShootHeld)
        {
            IWeapon weapon = _weaponInventory.CurrentWeapon;
            if (weapon == null)
                return;

            weapon.Tick(deltaTime);

            if (weapon.IsAutomatic && isShootHeld)
                weapon.Shoot();
        }
    }
}
