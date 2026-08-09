using System;
using _Project.Develop.Runtime.Configs.Meta.Weapon.WeaponsConfigs;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.WeaponsType;
using _Project.Develop.Runtime.Infrastructure.DI;
using _Project.Develop.Runtime.Utilities.AssetsManagement;
using _Project.Develop.Runtime.Utilities.ConfigsManagement;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Weapon
{
    public class WeaponFactory
    {
        private readonly DIContainer _container;
        private readonly ConfigsProviderService _configsProvider;
        private ResourcesAssetsLoader _resourcesAssetsLoader;
        private GameObject _bulletPrefab;
        
        public WeaponFactory(DIContainer container)
        {
            _container = container;
            _configsProvider = _container.Resolve <ConfigsProviderService>();
            _resourcesAssetsLoader = _container.Resolve <ResourcesAssetsLoader>();
            _bulletPrefab = _resourcesAssetsLoader.Load<GameObject>("Prefabs/Weapons/Bullets/Bullet");
        }

        public IWeapon CreateWeapon(
            WeaponType type,
            Vector3 spawnPosition,
            Transform firePoint,
            GameObject owner)
        {
            switch (type)
            {
                case WeaponType.Smg:
                    return CreateSmg(spawnPosition, firePoint, owner);
                
                case WeaponType.Usp:
                    return CreateUsp(spawnPosition, firePoint, owner);
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private IWeapon CreateSmg(
            Vector3 spawnPosition,
            Transform firePoint,
            GameObject owner)
        {
            //var WeaponPrefab = _resourcesAssetsLoader.Load<GameObject>("");
            //Object instance = Object.Instantiate(WeaponPrefab,position,Quaternion.identity);
            
            var smgConfig = _configsProvider.GetConfig<SmgConfig>();
            Debug.Log("СМГ создано!");
            return new Smg(smgConfig, firePoint, owner, _bulletPrefab);
        }

        private IWeapon CreateUsp(
            Vector3 spawnPosition,
            Transform firePoint,
            GameObject owner)
        {
            var uspConfig = _configsProvider.GetConfig<UspConfig>();
            Debug.Log("ЮСП СОЗДАН!");
            
            return new Usp(uspConfig, firePoint, owner, _bulletPrefab);
        }
    }
}