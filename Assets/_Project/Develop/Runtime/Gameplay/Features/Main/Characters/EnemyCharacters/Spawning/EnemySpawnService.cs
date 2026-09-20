using System;
using System.Collections.Generic;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Core;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter;
using _Project.Develop.Runtime.Gameplay.Features.Main.Interactables;
using _Project.Develop.Runtime.Gameplay.Features.Main.Stealth;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Spawning
{
    public class EnemySpawnService
    {
        private readonly CharactersFactory _charactersFactory;
        private readonly StealthKillService _stealthKills;
        private readonly EnemyAIService _enemyAIService;
        private readonly Dictionary<EnemyBase, IInteractable> _stealthTargets = new();

        private InteractionService _interactions;

        public EnemySpawnService(
            CharactersFactory charactersFactory,
            StealthKillService stealthKills,
            EnemyAIService enemyAIService)
        {
            _charactersFactory = charactersFactory;
            _stealthKills = stealthKills;
            _enemyAIService = enemyAIService;
            _enemyAIService.EnemyUnregistered += OnEnemyUnregistered;
        }

        public void SpawnFromRegistry(EnemySpawnRegistry registry, Player player, InteractionService interactions)
        {
            if (registry == null)
                throw new InvalidOperationException($"{nameof(EnemySpawnRegistry)} is missing on the gameplay scene.");

            if (registry.SpawnPoints == null || registry.SpawnPoints.Count == 0)
                throw new InvalidOperationException($"{nameof(EnemySpawnRegistry)} has no spawn points configured.");

            if (player == null)
                throw new ArgumentNullException(nameof(player));

            _interactions = interactions;

            foreach (EnemySpawnPoint spawnPoint in registry.SpawnPoints)
            {
                if (spawnPoint == null)
                    throw new InvalidOperationException($"{nameof(EnemySpawnRegistry)} contains a null spawn point.");

                EnemyBase enemy = _charactersFactory.CreateEnemy(spawnPoint);
                if (_stealthKills.HasPresentation(enemy) == false)
                    continue;

                StealthKillTarget target = new StealthKillTarget(enemy, player, _stealthKills);
                _stealthTargets[enemy] = target;
                interactions.Register(target);
            }
        }

        private void OnEnemyUnregistered(EnemyBase enemy)
        {
            if (_stealthTargets.Remove(enemy, out IInteractable target) == false)
                return;

            _interactions.Unregister(target);
        }
    }
}
