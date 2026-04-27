using System.Collections.Generic;
using _Project.Develop.Runtime.Configs.Meta.Player;
using _Project.Develop.Runtime.Configs.Meta.Weapon;
using _Project.Develop.Runtime.Gameplay.Features.Main.Controllers;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon;
using _Project.Develop.Runtime.Infrastructure.DI;
using _Project.Develop.Runtime.Utilities.AssetsManagement;
using _Project.Develop.Runtime.Utilities.InputManagement;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter;
using _Project.Develop.Runtime.Utilities.ConfigsManagement;
using Unity.VisualScripting;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters
{
    public class CharactersFactory
    {
        private ResourcesAssetsLoader _assetsLoader;
        private IInputService _input;
        private DIContainer _container;
        private WeaponFactory _factory;
        private Transform _playerTransform;
        private readonly ConfigsProviderService _configsProviderService;
        
        public CharactersFactory(DIContainer container)
        {
            _container = container;
            
            _assetsLoader= _container.Resolve<ResourcesAssetsLoader>();
            _input = _container.Resolve<IInputService>();
            _factory = _container.Resolve<WeaponFactory>();
            _configsProviderService = _container.Resolve<ConfigsProviderService>();
        }
        
        public Player CreatePlayer(Vector3 position)
        {
            var PlayerConfig = _configsProviderService.GetConfig<PlayerConfig>();
            var playerPrefab = _assetsLoader.Load<GameObject>("Prefabs/Entities/Player"); 
            GameObject instance = Object.Instantiate(playerPrefab, position, Quaternion.identity);
            
            Player player = instance.GetComponent<Player>();
            var characterController = instance.GetComponent<CharacterController>();
            _playerTransform = instance.GetComponent<Transform>();
            var PlayerView = instance.GetComponent<PlayerView>();
            var firepoint = PlayerView.Firepoint;

            var inventoryBuilder = _container.Resolve<PlayerWeaponInventory>();
            var inventory = inventoryBuilder.CreatePlayerWeaponInventory(_playerTransform, firepoint, instance);
            
            var mover = new CharacterControllerDirectionalMover(characterController, PlayerConfig);

            player.Initialize(_input, mover, inventory);
            Debug.Log("Игрок создан");
            return player;
        }
    }
}