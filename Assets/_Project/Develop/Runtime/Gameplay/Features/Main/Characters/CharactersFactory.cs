using _Project.Develop.Runtime.Configs.Meta.Characters.Player;
using _Project.Develop.Runtime.Configs.Meta.Enemy;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter;
using _Project.Develop.Runtime.Gameplay.Features.Main.Controllers;
using _Project.Develop.Runtime.Infrastructure.DI;
using _Project.Develop.Runtime.Utilities.AssetsManagement;
using _Project.Develop.Runtime.Utilities.ConfigsManagement;
using _Project.Develop.Runtime.Utilities.InputManagement;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters
{
    public class CharactersFactory
    {
        private readonly ResourcesAssetsLoader _assetsLoader;
        private readonly IInputService _input;
        private readonly DIContainer _container;
        private readonly ConfigsProviderService _configsProviderService;

        private Transform _playerTransform;

        public CharactersFactory(DIContainer container)
        {
            _container = container;

            _assetsLoader = _container.Resolve<ResourcesAssetsLoader>();
            _input = _container.Resolve<IInputService>();
            _configsProviderService = _container.Resolve<ConfigsProviderService>();
        }

        public Player CreatePlayer(Vector3 position)
        {
            var playerConfig = _configsProviderService.GetConfig<PlayerConfig>();
            var playerPrefab = _assetsLoader.Load<GameObject>("Prefabs/Entities/Player");
            GameObject instance = Object.Instantiate(playerPrefab, position, Quaternion.identity);

            Player player = instance.GetComponent<Player>();
            CharacterController characterController = instance.GetComponent<CharacterController>();
            _playerTransform = instance.transform;

            var inventoryBuilder = _container.Resolve<PlayerWeaponInventory>();
            var inventory = inventoryBuilder.CreatePlayerWeaponInventory(_playerTransform, player.FirePoint, instance);
            var mover = new CharacterControllerDirectionalMover(characterController, playerConfig);

            player.Initialize(_input, mover, inventory, playerConfig);
            Debug.Log("Игрок создан");

            return player;
        }

        public Enemy CreateEnemy(Vector3 position)
        {
            return CreateEnemy(position, _playerTransform);
        }

        public Enemy CreateEnemy(Vector3 position, Transform target)
        {
            var enemyConfig = _configsProviderService.GetConfig<EnemyConfig>();
            var enemyPrefab = _assetsLoader.Load<GameObject>("Prefabs/Entities/Enemy");
            GameObject instance = Object.Instantiate(enemyPrefab, position, Quaternion.identity);

            Enemy enemy = instance.GetComponent<Enemy>();
            CharacterController characterController = instance.GetComponent<CharacterController>();

            if (characterController == null)
                characterController = instance.AddComponent<CharacterController>();

            var mover = new CharacterControllerDirectionalMover(characterController, enemyConfig);

            enemy.Initialize(mover, enemyConfig, target);
            Debug.Log("Враг создан");

            return enemy;
        }
    }
}
