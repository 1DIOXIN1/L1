using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Core
{
    public sealed class InfiltrationEnemyState : IEnemyState
    {
        public EnemyStateId Id => EnemyStateId.Infiltration;

        public void Enter(EnemyContext context)
        {
            context.Agent.isStopped = false;
            context.Agent.speed = context.Preset.ChaseSpeed;
        }

        public void Exit(EnemyContext context)
        {
            context.Enemy.ResetCombat();
            context.Agent.isStopped = true;

            if (context.Agent.hasPath)
                context.Agent.ResetPath();
        }

        public void Tick(EnemyContext context, float deltaTime)
        {
            if (context.Player == null)
                return;

            context.Enemy.TickCombat(deltaTime);
        }
    }
}
