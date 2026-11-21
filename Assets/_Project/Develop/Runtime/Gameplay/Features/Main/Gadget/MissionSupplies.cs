using System;
using _Project.Develop.Runtime.Configs.Meta.Gadget.GadgetsConfigs;
using _Project.Develop.Runtime.Configs.Meta.Weapon;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter;
using _Project.Develop.Runtime.Utilities.InputManagement;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Gadget
{
    public sealed class MissionSupplies : IDisposable
    {
        private readonly MissionSuppliesConfig _config;
        private readonly MissionSupplyInventory _inventory;
        private readonly EmpGrenadeAim _aim;
        private readonly IInputService _input;
        private readonly Player _player;
        private readonly EmpGrenade _emp;

        public MissionSupplies(MissionSuppliesConfig config, MissionSupplyInventory inventory,
            IInputService input, Player player, EmpGrenade emp)
        {
            _config = config;
            _inventory = inventory;
            _input = input;
            _player = player;
            _emp = emp;
            _aim = new EmpGrenadeAim(emp);
        }

        public int Count(SupplyType type) => _inventory.Count(type);

        public bool Tick(float deltaTime)
        {
            _emp.Tick(deltaTime);

            if (_player.IsControlLocked || _input.CurrentContext != InputContext.Gameplay)
            {
                _aim.Cancel();
                return false;
            }

            if (_input.MedkitPressed && Count(SupplyType.Medkit) > 0 && _player.TryHeal(_config.MedkitHealing))
                _inventory.Consume(SupplyType.Medkit);

            if (_input.EnergyDrinkPressed && Count(SupplyType.EnergyDrink) > 0 &&
                _player.TryPreventStaminaDrain(_config.EnergyDuration))
                _inventory.Consume(SupplyType.EnergyDrink);

            if (Count(SupplyType.EmpGrenade) == 0)
                _aim.Cancel();
            else if (_aim.TryThrow(_input))
                _inventory.Consume(SupplyType.EmpGrenade);

            return _inventory.SaveIfChanged();
        }

        public void Dispose() => _emp.Dispose();
    }
}
