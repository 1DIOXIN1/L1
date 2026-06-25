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
            TickSpotterAlarm(context, deltaTime);
        }

        private static void TickSpotterAlarm(EnemyContext context, float deltaTime)
        {
            if (context.IsSpotter == false || context.AlarmSpreadTriggered)
                return;

            if (context.Enemy.CanSeePlayer() == false)
            {
                context.IsSpotter = false;
                context.SpotterTimer = 0f;
                context.InfiltrationTriggered = false;
                context.Enemy.StateMachine.ChangeState(EnemyStateId.Patrol);
                return;
            }

            context.SpotterTimer += deltaTime;

            if (context.SpotterTimer < context.Config.InfiltrationTriggerTime)
                return;

            context.AlarmSpreadTriggered = true;
            context.AIService.SpreadAlarm(context.Enemy);
        }
    }
}
