using System;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Spawning
{
    public class EnemySpawnService
    {
        private readonly CharactersFactory _charactersFactory;

        public EnemySpawnService(CharactersFactory charactersFactory)
        {
            _charactersFactory = charactersFactory;
        }

        public void SpawnFromRegistry(EnemySpawnRegistry registry)
        {
            if (registry == null)
                throw new InvalidOperationException($"{nameof(EnemySpawnRegistry)} is missing on the gameplay scene.");

            if (registry.SpawnPoints == null || registry.SpawnPoints.Count == 0)
                throw new InvalidOperationException($"{nameof(EnemySpawnRegistry)} has no spawn points configured.");

            foreach (EnemySpawnPoint spawnPoint in registry.SpawnPoints)
            {
                if (spawnPoint == null)
                    throw new InvalidOperationException($"{nameof(EnemySpawnRegistry)} contains a null spawn point.");

                _charactersFactory.CreateEnemy(spawnPoint);
            }
        }
    }
}
