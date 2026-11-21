using System;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter;
using _Project.Develop.Runtime.Meta.Features.Player;
using _Project.Develop.Runtime.Utilities.CoroutinesManagement;
using _Project.Develop.Runtime.Utilities.DataManagement.DataProviders;
using _Project.Develop.Runtime.Utilities.InputManagement;
using _Project.Develop.Runtime.Configs.Meta.Gadget.GadgetsConfigs;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters;
using _Project.Develop.Runtime.Gameplay.Features.Main.Gadget.GadgetsType;
using _Project.Develop.Runtime.Infrastructure.DI;
using _Project.Develop.Runtime.Utilities.ConfigsManagement;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Gadget
{
    public class GadgetFactory
    {
        private readonly DIContainer _container;
        private readonly ConfigsProviderService _configsProvider;

        public GadgetFactory(DIContainer container)
        {
            _container = container;
            _configsProvider = container.Resolve<ConfigsProviderService>();
        }

        public MissionSupplies CreateMissionSupplies(Player player, PlayerCamera camera)
        {
            var config = _configsProvider.GetConfig<MissionSuppliesConfig>();
            var emp = new EmpGrenade(config, camera.LookCamera.transform, player.transform,
                _container.Resolve<DamageableRegistry>());

            var inventory = new MissionSupplyInventory(config.CapacityPerType, _container.Resolve<PlayerStateService>(),
                _container.Resolve<PlayerDataProvider>(), _container.Resolve<CoroutinesPerformer>());

            return new MissionSupplies(config, inventory, _container.Resolve<IInputService>(), player, emp);
        }

        public IGadget CreateGadget(
            GadgetType type,
            Transform usePoint,
            IDamageble attacker)
        {
            switch (type)
            {
                case GadgetType.Grenade:
                    return CreateGrenade(usePoint, attacker);

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private IGadget CreateGrenade(
            Transform usePoint,
            IDamageble attacker)
        {
            var config = _configsProvider.GetConfig<GrenadeConfig>();
            return new Grenade(config, usePoint, attacker);
        }
    }
}
