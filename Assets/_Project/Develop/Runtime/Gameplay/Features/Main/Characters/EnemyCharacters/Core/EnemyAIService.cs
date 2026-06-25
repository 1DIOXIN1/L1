using System.Collections.Generic;
using _Project.Develop.Runtime.Configs.Meta.Enemy;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Core
{
    public class EnemyAIService
    {
        private readonly EnemyConfig _config;
        private readonly List<EnemyBase> _enemies = new();

        public EnemyAIService(EnemyConfig config)
        {
            _config = config;
        }

        public void Register(EnemyBase enemy)
        {
            if (_enemies.Contains(enemy) == false)
                _enemies.Add(enemy);
        }

        public void Unregister(EnemyBase enemy)
        {
            _enemies.Remove(enemy);
        }

        public void TriggerInfiltration(EnemyBase source)
        {
            if (source == null)
                return;

            source.EnterInfiltration();

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
