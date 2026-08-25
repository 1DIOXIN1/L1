using System;
using System.Collections.Generic;
using _Project.Develop.Runtime.Configs.Meta.Enemy;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Core
{
    public class EnemyAIService
    {
        private readonly EnemyConfig _config;
        private readonly List<EnemyBase> _enemies = new();

        private bool _spawnCompleted;
        private int _registeredCount;

        public event Action AllEnemiesEliminated;

        public EnemyAIService(EnemyConfig config)
        {
            _config = config;
        }

        public int RegisteredCount => _registeredCount;

        public int AliveCount
        {
            get
            {
                int count = 0;
                for (int i = 0; i < _enemies.Count; i++)
                {
                    if (_enemies[i] != null && _enemies[i].IsAlive)
                        count++;
                }

                return count;
            }
        }

        public IReadOnlyList<EnemyBase> Enemies => _enemies;

        public void Register(EnemyBase enemy)
        {
            if (_enemies.Contains(enemy))
                return;

            _enemies.Add(enemy);
            _registeredCount++;
        }

        public void Unregister(EnemyBase enemy)
        {
            _enemies.Remove(enemy);

            if (_spawnCompleted && _registeredCount > 0 && AliveCount == 0)
                AllEnemiesEliminated?.Invoke();
        }

        public void MarkSpawnComplete()
        {
            _spawnCompleted = true;

            if (_registeredCount > 0 && AliveCount == 0)
                AllEnemiesEliminated?.Invoke();
        }

        public void SpreadAlarm(EnemyBase source)
        {
            if (source == null)
                return;

            foreach (EnemyBase enemy in _enemies)
            {
                if (enemy == null || enemy == source || enemy.IsAlive == false)
                    continue;

                float distance = Vector3.Distance(source.transform.position, enemy.transform.position);
                if (distance > _config.InfiltrationSpreadRadius)
                    continue;

                enemy.EnterInfiltration();
            }
        }
    }
}
