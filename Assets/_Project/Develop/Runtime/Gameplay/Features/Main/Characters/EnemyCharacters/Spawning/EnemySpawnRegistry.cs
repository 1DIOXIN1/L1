using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Spawning
{
    public class EnemySpawnRegistry : MonoBehaviour
    {
        [SerializeField] private EnemySpawnPoint[] spawnPoints = Array.Empty<EnemySpawnPoint>();

        public IReadOnlyList<EnemySpawnPoint> SpawnPoints => spawnPoints;

        private void OnValidate()
        {
            if (spawnPoints == null || spawnPoints.Length == 0)
                spawnPoints = GetComponentsInChildren<EnemySpawnPoint>(true);
        }
    }
}
