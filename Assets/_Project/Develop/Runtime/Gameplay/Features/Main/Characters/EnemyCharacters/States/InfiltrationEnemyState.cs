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
            context.Agent.stoppingDistance = context.Preset.AttackRange * 0.8f;
        }

        public void Exit(EnemyContext context)
        {
            context.Enemy.ResetCombat();
        }

        public void Tick(EnemyContext context, float deltaTime)
        {
            if (context.Player == null)
                return;

            context.Agent.SetDestination(context.Player.position);
            context.Enemy.LookAtPlayer();
            context.Enemy.TickCombat(deltaTime);
        }
    }
}
