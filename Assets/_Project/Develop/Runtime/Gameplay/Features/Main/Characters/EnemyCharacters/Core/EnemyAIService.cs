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
        public event Action<EnemyBase> EnemyRegistered;
        public event Action<EnemyBase> EnemyUnregistered;

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
            EnemyRegistered?.Invoke(enemy);
        }

        public void Unregister(EnemyBase enemy)
        {
            if (_enemies.Remove(enemy) == false)
                return;

            EnemyUnregistered?.Invoke(enemy);

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

            Vector3 alarmSourcePosition = source.transform.position;

            foreach (EnemyBase enemy in _enemies)
            {
                if (enemy == null || enemy == source || enemy.IsAlive == false)
                    continue;

                float distance = Vector3.Distance(alarmSourcePosition, enemy.transform.position);
                if (distance > _config.InfiltrationSpreadRadius)
                    continue;

                enemy.EnterInfiltrationAsAlarmResponder(alarmSourcePosition);
            }
        }
    }
}
