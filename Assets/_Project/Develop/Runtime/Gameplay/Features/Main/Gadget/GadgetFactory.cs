using System;
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
        private readonly ConfigsProviderService _configsProvider;

        public GadgetFactory(DIContainer container)
        {
            _configsProvider = container.Resolve<ConfigsProviderService>();
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
