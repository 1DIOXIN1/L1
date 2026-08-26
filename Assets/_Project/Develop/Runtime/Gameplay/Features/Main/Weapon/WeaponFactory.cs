using System.Collections.Generic;
using _Project.Develop.Runtime.Configs.Meta.Weapon;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.FireModes;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.WeaponsType;
using _Project.Develop.Runtime.Infrastructure.DI;
using _Project.Develop.Runtime.Utilities.AssetsManagement;
using _Project.Develop.Runtime.Utilities.ConfigsManagement;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Weapon
{
    public class WeaponFactory
    {
        private readonly ConfigsProviderService _configsProvider;
        private readonly FireModeRegistry _fireModeRegistry;
        private readonly ResourcesAssetsLoader _resourcesAssetsLoader;
        private readonly GameObject _bulletPrefab;

        public WeaponFactory(DIContainer container)
        {
            _configsProvider = container.Resolve<ConfigsProviderService>();
            _fireModeRegistry = container.Resolve<FireModeRegistry>();
            _resourcesAssetsLoader = container.Resolve<ResourcesAssetsLoader>();
            _bulletPrefab = _resourcesAssetsLoader.Load<GameObject>("Prefabs/Weapons/Bullets/Bullet");
        }

        public IWeapon CreateWeapon(WeaponType type, Transform firePoint, GameObject owner, int? initialAmmo = null)
        {
            WeaponsCatalogConfig catalog = _configsProvider.GetConfig<WeaponsCatalogConfig>();
            WeaponConfig config = catalog.GetWeapon(type);
            IFireMode fireMode = _fireModeRegistry.Resolve(config.FireMode);
            GameObject bulletPrefab = config.FireMode == WeaponFireMode.Projectile ? _bulletPrefab : null;

            return new Weapon(config, fireMode, firePoint, owner, bulletPrefab, initialAmmo);
        }
    }
}
