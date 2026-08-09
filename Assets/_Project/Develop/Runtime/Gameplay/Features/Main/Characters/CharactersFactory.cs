using System.Collections.Generic;
using System.Linq;
using _Project.Develop.Runtime.Configs.Meta.Characters.Player;
using _Project.Develop.Runtime.Configs.Meta.Enemy;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Core;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Spawning;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter;
using _Project.Develop.Runtime.Gameplay.Features.Main.Controllers;
using _Project.Develop.Runtime.Gameplay.Infrastructure;
using _Project.Develop.Runtime.Infrastructure.DI;
using _Project.Develop.Runtime.Utilities.AssetsManagement;
using _Project.Develop.Runtime.Utilities.ConfigsManagement;
using _Project.Develop.Runtime.Utilities.InputManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters
{
    public class CharactersFactory
    {
        private readonly ResourcesAssetsLoader _assetsLoader;
        private readonly IInputService _input;
        private readonly DIContainer _container;
        private readonly ConfigsProviderService _configsProviderService;
        private readonly EnemyAIService _enemyAIService;

        private Transform _playerTransform;

        public CharactersFactory(DIContainer container)
        {
            _container = container;

            _assetsLoader = _container.Resolve<ResourcesAssetsLoader>();
            _input = _container.Resolve<IInputService>();
            _configsProviderService = _container.Resolve<ConfigsProviderService>();
            _enemyAIService = _container.Resolve<EnemyAIService>();
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
            var gadgetInventoryBuilder = _container.Resolve<PlayerGadgetInventory>();
            var gadgetInventory = gadgetInventoryBuilder.CreatePlayerGadgetInventory(player.FirePoint, instance);
            var mover = new CharacterControllerDirectionalMover(characterController, playerConfig);
            var gameMode = _container.Resolve<GameMode>();

            player.Initialize(_input, mover, inventory, gadgetInventory, playerConfig);
            player.SetDeathHandler(gameMode.TriggerDefeat);

            return player;
        }

        public EnemyBase CreateEnemy(EnemySpawnPoint spawnPoint)
        {
            return CreateEnemy(spawnPoint.SpawnPosition, spawnPoint.EnemyType, spawnPoint.PatrolPoints);
        }

        public EnemyBase CreateEnemy(Vector3 position, EnemyType type, Transform[] patrolPoints = null)
        {
            var enemyConfig = _configsProviderService.GetConfig<EnemyConfig>();
            GameObject prefab = ResolvePrefab(enemyConfig, type);

            if (prefab == null)
                throw new System.InvalidOperationException($"Prefab for enemy type {type} is not configured.");

            GameObject instance = Object.Instantiate(prefab, position, Quaternion.identity);
            EnemyBase enemy = instance.GetComponent<EnemyBase>();

            if (enemy == null)
                throw new System.InvalidOperationException($"Prefab '{prefab.name}' for {type} has no {nameof(EnemyBase)} component.");

            CharacterController characterController = instance.GetComponent<CharacterController>();

            if (characterController != null)
                characterController.enabled = false;

            var bulletPrefab = _assetsLoader.Load<GameObject>("Prefabs/Weapons/Bullets/Bullet");
            IReadOnlyList<Transform> points = ResolvePatrolPoints(position, patrolPoints);

            enemy.Initialize(_enemyAIService, enemyConfig, _playerTransform, bulletPrefab, points);

            return enemy;
        }

        private GameObject ResolvePrefab(EnemyConfig enemyConfig, EnemyType type)
        {
            GameObject prefab = enemyConfig.GetPrefab(type);
            if (prefab != null)
                return prefab;

            return type switch
            {
                EnemyType.Melee => _assetsLoader.Load<GameObject>("Prefabs/Entities/Melee"),
                EnemyType.Ranger => _assetsLoader.Load<GameObject>("Prefabs/Entities/Ranger"),
                _ => enemyConfig.CookerPrefab
            };
        }

        public void SpawnEnemiesFromScene()
        {
            EnemySpawnPoint[] spawnPoints = Object.FindObjectsOfType<EnemySpawnPoint>();

            if (spawnPoints.Length == 0)
            {
                CreateFallbackEnemies();
                return;
            }

            foreach (EnemySpawnPoint spawnPoint in spawnPoints)
                CreateEnemy(spawnPoint);
        }

        private void CreateFallbackEnemies()
        {
            CreateEnemy(new Vector3(5f, 0f, 2f), EnemyType.Ranger, CreateDefaultPatrolPoints(new Vector3(5f, 0f, 2f)));
            CreateEnemy(new Vector3(-3f, 0f, 4f), EnemyType.Cooker, CreateDefaultPatrolPoints(new Vector3(-3f, 0f, 4f)));
            CreateEnemy(new Vector3(1f, 0f, -4f), EnemyType.Melee, CreateDefaultPatrolPoints(new Vector3(1f, 0f, -4f)));
        }

        private static Transform[] CreateDefaultPatrolPoints(Vector3 center)
        {
            var points = new Transform[3];
            Vector3[] offsets =
            {
                new(3f, 0f, 0f),
                new(-2f, 0f, 3f),
                new(-2f, 0f, -3f)
            };

            for (int i = 0; i < offsets.Length; i++)
            {
                var pointObject = new GameObject($"PatrolPoint_{i}");
                pointObject.transform.position = center + offsets[i];
                points[i] = pointObject.transform;
            }

            return points;
        }

        private static IReadOnlyList<Transform> ResolvePatrolPoints(Vector3 spawnPosition, Transform[] patrolPoints)
        {
            if (patrolPoints != null && patrolPoints.Any(point => point != null))
                return patrolPoints;

            return CreateDefaultPatrolPoints(spawnPosition);
        }
    }
}
