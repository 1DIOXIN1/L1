using System;
using System.Collections.Generic;
using _Project.Develop.Runtime.Configs.Meta.Characters.Player;
using _Project.Develop.Runtime.Configs.Meta.Enemy;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.AttackBehaviors;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Core;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Spawning;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon;
using _Project.Develop.Runtime.Gameplay.Infrastructure;
using _Project.Develop.Runtime.Infrastructure.DI;
using _Project.Develop.Runtime.Meta.Features.Player;
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
        private readonly EnemyAttackBehaviorFactory _attackBehaviorFactory;
        private readonly PlayerStateService _playerStateService;

        private Transform _playerTransform;

        public CharactersFactory(DIContainer container)
        {
            _container = container;

            _assetsLoader = _container.Resolve<ResourcesAssetsLoader>();
            _input = _container.Resolve<IInputService>();
            _configsProviderService = _container.Resolve<ConfigsProviderService>();
            _enemyAIService = _container.Resolve<EnemyAIService>();
            _attackBehaviorFactory = _container.Resolve<EnemyAttackBehaviorFactory>();
            _playerStateService = _container.Resolve<PlayerStateService>();
        }

        public Player CreatePlayer(PlayerSpawnPoint spawnPoint)
        {
            if (spawnPoint == null)
                throw new InvalidOperationException($"{nameof(PlayerSpawnPoint)} is missing on the gameplay scene.");

            var playerConfig = _configsProviderService.GetConfig<PlayerConfig>();
            var playerPrefab = _assetsLoader.Load<GameObject>("Prefabs/Entities/Player");
            GameObject instance = Object.Instantiate(playerPrefab, spawnPoint.Position, spawnPoint.Rotation);

            Player player = instance.GetComponent<Player>();
            CharacterController characterController = instance.GetComponent<CharacterController>();
            _playerTransform = instance.transform;

            var inventoryBuilder = _container.Resolve<PlayerWeaponInventory>();
            var inventory = inventoryBuilder.CreatePlayerWeaponInventory(_playerTransform, player.FirePoint, instance);
            var gadgetInventoryBuilder = _container.Resolve<PlayerGadgetInventory>();
            var gadgetInventory = gadgetInventoryBuilder.CreatePlayerGadgetInventory(player.FirePoint, instance);

            var motor = new PlayerMotor(characterController, _playerTransform, player.ViewTransform, playerConfig);
            var combat = new PlayerCombatController(inventory, gadgetInventory);
            var gameMode = _container.Resolve<GameMode>();

            player.Initialize(_input, motor, combat, playerConfig, _playerStateService.Health);
            gameMode.RegisterPlayer(player, inventory);
            player.SetDeathHandler(gameMode.TriggerDefeat);

            return player;
        }

        public EnemyBase CreateEnemy(EnemySpawnPoint spawnPoint)
        {
            if (spawnPoint == null)
                throw new ArgumentNullException(nameof(spawnPoint));

            if (spawnPoint.HasValidPatrolPoints() == false)
                throw new InvalidOperationException(
                    $"{nameof(EnemySpawnPoint)} '{spawnPoint.name}' has no valid patrol points.");

            return CreateEnemy(spawnPoint.SpawnPosition, spawnPoint.EnemyType, spawnPoint.PatrolPoints);
        }

        public EnemyBase CreateEnemy(Vector3 position, EnemyType type, IReadOnlyList<Transform> patrolPoints)
        {
            if (patrolPoints == null || patrolPoints.Count == 0)
                throw new ArgumentException("Patrol points are required.", nameof(patrolPoints));

            if (_playerTransform == null)
                throw new InvalidOperationException("Player must be spawned before enemies.");

            var enemyConfig = _configsProviderService.GetConfig<EnemyConfig>();
            GameObject prefab = enemyConfig.GetPrefab(type);

            if (prefab == null)
                throw new InvalidOperationException($"Prefab for enemy type {type} is not configured in {nameof(EnemyConfig)}.");

            GameObject instance = Object.Instantiate(prefab, position, Quaternion.identity);
            EnemyBase enemy = instance.GetComponent<EnemyBase>();

            if (enemy == null)
                throw new InvalidOperationException($"Prefab '{prefab.name}' for {type} has no {nameof(EnemyBase)} component.");

            CharacterController characterController = instance.GetComponent<CharacterController>();

            if (characterController != null)
                characterController.enabled = false;

            var bulletPrefab = _assetsLoader.Load<GameObject>("Prefabs/Weapons/Bullets/Bullet");
            enemy.Initialize(
                _enemyAIService,
                enemyConfig,
                _playerTransform,
                bulletPrefab,
                patrolPoints,
                _attackBehaviorFactory);

            return enemy;
        }
    }
}
