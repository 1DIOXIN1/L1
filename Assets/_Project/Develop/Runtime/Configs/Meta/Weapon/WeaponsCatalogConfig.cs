using System;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.WeaponsType;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Develop.Runtime.Configs.Meta.Weapon
{
    [CreateAssetMenu(menuName = "Configs/Core/Gameplay/Weapon/WeaponsCatalogConfig", fileName = "WeaponsCatalogConfig")]
    public class WeaponsCatalogConfig : ScriptableObject
    {
        [SerializeField] private WeaponConfig[] _configs = Array.Empty<WeaponConfig>();

        public WeaponConfig GetWeapon(WeaponType type)
        {
            foreach (WeaponConfig config in _configs)
            {
                if (config != null && config.Type == type)
                    return config;
            }

            throw new InvalidOperationException($"Weapon config for {type} is not configured.");
        }
    }
}
