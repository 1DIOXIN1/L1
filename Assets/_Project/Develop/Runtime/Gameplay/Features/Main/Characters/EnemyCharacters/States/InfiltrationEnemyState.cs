using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Detection;
using UnityEngine;
using UnityEngine.AI;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Core
{
    public sealed class InfiltrationEnemyState : IEnemyState
    {
        private const float SearchStoppingDistance = 1.5f;

        public EnemyStateId Id => EnemyStateId.Infiltration;

        public void Enter(EnemyContext context)
        {
            context.IsSearchingLastKnown = false;
            context.Agent.isStopped = false;
            context.Agent.speed = context.Preset.ChaseSpeed;
        }

        public void Exit(EnemyContext context)
        {
            context.Enemy.ResetCombat();
            context.IsAlarmResponder = false;
            context.IsSearchingLastKnown = false;
            context.Agent.isStopped = true;

            if (context.Agent.hasPath)
                context.Agent.ResetPath();
        }

        public void Tick(EnemyContext context, float deltaTime)
        {
            if (context.Player == null)
                return;

            bool canSee = context.Enemy.CanSeePlayer();

            if (canSee)
            {
                RememberPlayerPosition(context, context.Player.position);
                context.IsSearchingLastKnown = false;
            }

            context.Awareness.TickSight(canSee, deltaTime);

            if (context.IsAlarmResponder)
            {
                TickResponder(context, canSee, deltaTime);
                return;
            }

            if (canSee && context.Awareness.IsAlerted)
            {
                context.Enemy.TickCombat(deltaTime);
                TickSpotterAlarm(context, deltaTime);
                return;
            }

            TickSearchLastKnown(context, canSee, deltaTime);
        }

        private static void TickResponder(EnemyContext context, bool canSee, float deltaTime)
        {
            if (canSee)
            {
                context.IsAlarmResponder = false;
                context.IsSpotter = true;
                context.SpotterTimer = 0f;
                context.AlarmSpreadTriggered = false;
                context.IsSearchingLastKnown = false;
                context.Awareness.ForceAlerted();
                RememberPlayerPosition(context, context.Player.position);
                context.Enemy.TickCombat(deltaTime);
                return;
            }

            if (context.HasLastKnownPlayerPosition == false)
                RememberPlayerPosition(context, context.AlarmSourcePosition);

            TickSearchLastKnown(context, canSee: false, deltaTime);
        }

        private static void TickSearchLastKnown(EnemyContext context, bool canSee, float deltaTime)
        {
            if (context.IsSearchingLastKnown == false)
            {
                context.IsSearchingLastKnown = true;
                context.Enemy.ResetCombat();
            }

            if (context.HasLastKnownPlayerPosition == false)
            {
                ReturnToPatrol(context);
                return;
            }

            NavMeshAgent agent = context.Agent;
            agent.isStopped = false;
            agent.speed = context.Preset.ChaseSpeed;
            agent.stoppingDistance = SearchStoppingDistance;
            agent.SetDestination(context.LastKnownPlayerPosition);

            Vector3 toTarget = context.LastKnownPlayerPosition - context.Enemy.transform.position;
            toTarget.y = 0f;
            if (toTarget.sqrMagnitude > 0.05f)
                context.Enemy.RotateTowards(toTarget, deltaTime);

            if (agent.pathPending)
                return;

            if (agent.remainingDistance > agent.stoppingDistance + 0.35f)
                return;

            if (canSee == false)
                ReturnToPatrol(context);
        }

        private static void TickSpotterAlarm(EnemyContext context, float deltaTime)
        {
            if (context.IsSpotter == false || context.AlarmSpreadTriggered)
                return;

            if (context.Enemy.CanSeePlayer() == false)
                return;

            context.SpotterTimer += deltaTime;

            if (context.SpotterTimer < context.Config.InfiltrationTriggerTime)
                return;

            context.AlarmSpreadTriggered = true;
            context.AIService.SpreadAlarm(context.Enemy);
        }

        private static void RememberPlayerPosition(EnemyContext context, Vector3 position)
        {
            context.LastKnownPlayerPosition = position;
            context.HasLastKnownPlayerPosition = true;
        }

        private static void ReturnToPatrol(EnemyContext context)
        {
            context.Awareness.ResetToCalm();
            context.IsSearchingLastKnown = false;
            context.IsSpotter = false;
            context.IsAlarmResponder = false;
            context.InfiltrationTriggered = false;
            context.AlarmSpreadTriggered = false;
            context.SpotterTimer = 0f;
            context.Enemy.StateMachine.ChangeState(EnemyStateId.Patrol);
        }
    }
}
