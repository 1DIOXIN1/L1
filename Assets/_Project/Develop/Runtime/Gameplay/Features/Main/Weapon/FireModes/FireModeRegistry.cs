using System;
using System.Collections.Generic;
using _Project.Develop.Runtime.Configs.Meta.Weapon;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.FireModes
{
    public sealed class FireModeRegistry
    {
        private readonly Dictionary<WeaponFireMode, IFireMode> _fireModes = new()
        {
            { WeaponFireMode.Projectile, new ProjectileFireMode() },
            { WeaponFireMode.Hitscan, new HitscanFireMode() }
        };

        public IFireMode Resolve(WeaponFireMode fireMode)
        {
            if (_fireModes.TryGetValue(fireMode, out IFireMode mode))
                return mode;

            throw new InvalidOperationException($"Fire mode {fireMode} is not registered.");
        }
    }
}
