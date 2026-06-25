using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Core
{
    public sealed class DetectingEnemyState : IEnemyState
    {
        public EnemyStateId Id => EnemyStateId.Detecting;

        public void Enter(EnemyContext context)
        {
            context.DetectingTimer = 0f;
            context.InfiltrationTriggered = false;
            context.Agent.isStopped = true;
            context.Agent.ResetPath();
        }

        public void Exit(EnemyContext context)
        {
        }

        public void Tick(EnemyContext context, float deltaTime)
        {
            if (context.Enemy.IsAlive == false)
                return;

            context.Enemy.LookAtPlayer();

            if (context.Enemy.CanSeePlayer() == false)
            {
                context.Enemy.StateMachine.ChangeState(EnemyStateId.Patrol);
                return;
            }

            context.DetectingTimer += deltaTime;

            if (context.DetectingTimer < context.Config.InfiltrationTriggerTime)
                return;

            context.InfiltrationTriggered = true;
            context.AIService.TriggerInfiltration(context.Enemy);
        }
    }
}
