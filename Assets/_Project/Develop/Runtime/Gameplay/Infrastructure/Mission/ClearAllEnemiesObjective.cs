using System;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Core;

namespace _Project.Develop.Runtime.Gameplay.Infrastructure.Mission
{
    public class ClearAllEnemiesObjective : IMissionObjective
    {
        private readonly EnemyAIService _enemyAIService;

        public ClearAllEnemiesObjective(EnemyAIService enemyAIService)
        {
            _enemyAIService = enemyAIService;
        }

        public bool IsComplete { get; private set; }
        public event Action Completed;

        public void Start()
        {
            if (_enemyAIService.RegisteredCount == 0)
                return;

            if (_enemyAIService.AliveCount == 0)
            {
                Complete();
                return;
            }

            _enemyAIService.AllEnemiesEliminated += OnAllEnemiesEliminated;
        }

        public void Stop()
        {
            _enemyAIService.AllEnemiesEliminated -= OnAllEnemiesEliminated;
        }

        private void OnAllEnemiesEliminated()
        {
            _enemyAIService.AllEnemiesEliminated -= OnAllEnemiesEliminated;
            Complete();
        }

        private void Complete()
        {
            if (IsComplete)
                return;

            IsComplete = true;
            Completed?.Invoke();
        }
    }
}
