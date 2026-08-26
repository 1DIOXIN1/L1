using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Core;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Detection;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Noise
{
    public sealed class NoiseService
    {
        private readonly EnemyAIService _enemyAIService;

        public NoiseService(EnemyAIService enemyAIService)
        {
            _enemyAIService = enemyAIService;
        }

        public void Emit(Vector3 position, float hearingRadius, NoiseStimulus stimulus)
        {
            if (hearingRadius <= 0f)
                return;

            float radiusSqr = hearingRadius * hearingRadius;
            var enemies = _enemyAIService.Enemies;

            for (int i = 0; i < enemies.Count; i++)
            {
                EnemyBase enemy = enemies[i];
                if (enemy == null || enemy.IsAlive == false)
                    continue;

                float distanceSqr = (enemy.transform.position - position).sqrMagnitude;
                if (distanceSqr > radiusSqr)
                    continue;

                enemy.ApplyStimulus(new NoiseStimulus(
                    position,
                    stimulus.SuspicionAmount,
                    stimulus.Type));
            }
        }
    }
}
