using System;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter;
using _Project.Develop.Runtime.Gameplay.Features.Main.Interactables;
using _Project.Develop.Runtime.Gameplay.Features.Main.Stealth;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Spawning
{
    public class EnemySpawnService
    {
        private readonly CharactersFactory _charactersFactory;
        private readonly StealthKillService _stealthKills;
        private readonly InteractionService _interactions;

        public EnemySpawnService(
            CharactersFactory charactersFactory,
            StealthKillService stealthKills,
            InteractionService interactions)
        {
            _charactersFactory = charactersFactory;
            _stealthKills = stealthKills;
            _interactions = interactions;
        }

        public void SpawnFromRegistry(EnemySpawnRegistry registry, Player player)
        {
            if (registry == null)
                throw new InvalidOperationException($"{nameof(EnemySpawnRegistry)} is missing on the gameplay scene.");

            if (registry.SpawnPoints == null || registry.SpawnPoints.Count == 0)
                throw new InvalidOperationException($"{nameof(EnemySpawnRegistry)} has no spawn points configured.");

            if (player == null)
                throw new ArgumentNullException(nameof(player));

            foreach (EnemySpawnPoint spawnPoint in registry.SpawnPoints)
            {
                if (spawnPoint == null)
                    throw new InvalidOperationException($"{nameof(EnemySpawnRegistry)} contains a null spawn point.");

                EnemyBase enemy = _charactersFactory.CreateEnemy(spawnPoint);
                if (_stealthKills.HasPresentation(enemy))
                    _interactions.Register(new StealthKillTarget(enemy, player, _stealthKills));
            }
        }
    }
}
