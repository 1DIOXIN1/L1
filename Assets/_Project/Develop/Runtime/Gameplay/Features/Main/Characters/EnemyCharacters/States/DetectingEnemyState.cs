using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Detection;
using UnityEngine;
using UnityEngine.AI;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Core
{
    public sealed class DetectingEnemyState : IEnemyState
    {
        private const float InvestigateStoppingDistance = 1.25f;

        public EnemyStateId Id => EnemyStateId.Detecting;

        public void Enter(EnemyContext context)
        {
            context.Agent.isStopped = false;
            context.Agent.speed = context.Preset.ChaseSpeed;
            context.Agent.stoppingDistance = InvestigateStoppingDistance;
        }

        public void Exit(EnemyContext context)
        {
            context.HasInvestigateTarget = false;

            if (context.Agent.hasPath)
                context.Agent.ResetPath();
        }

        public void Tick(EnemyContext context, float deltaTime)
        {
            bool canSee = context.Enemy.CanSeePlayer();

            if (canSee && context.Player != null)
                SetInvestigateTarget(context, context.Player.position);

            context.Awareness.TickSight(canSee, deltaTime);

            if (context.Awareness.Phase == DetectionPhase.Alerted)
            {
                context.Enemy.EnterCombatAsSpotter();
                return;
            }

            if (context.Awareness.Phase == DetectionPhase.Calm)
            {
                context.Enemy.StateMachine.ChangeState(EnemyStateId.Patrol);
                return;
            }

            if (canSee)
            {
                context.Enemy.StopAgent();
                context.Enemy.LookAtPlayer(deltaTime);
                return;
            }

            TickInvestigate(context, deltaTime);
        }

        private static void TickInvestigate(EnemyContext context, float deltaTime)
        {
            if (context.HasInvestigateTarget == false)
            {
                context.Enemy.StopAgent();
                return;
            }

            NavMeshAgent agent = context.Agent;
            agent.isStopped = false;
            agent.speed = context.Preset.ChaseSpeed;
            agent.stoppingDistance = InvestigateStoppingDistance;
            agent.SetDestination(context.InvestigatePosition);

            Vector3 toTarget = context.InvestigatePosition - context.Enemy.transform.position;
            toTarget.y = 0f;
            if (toTarget.sqrMagnitude > 0.05f)
                context.Enemy.RotateTowards(toTarget, deltaTime);

            if (agent.pathPending)
                return;

            if (agent.remainingDistance > agent.stoppingDistance + 0.35f)
                return;

            context.Enemy.StopAgent();
        }

        private static void SetInvestigateTarget(EnemyContext context, Vector3 position)
        {
            context.InvestigatePosition = position;
            context.HasInvestigateTarget = true;
        }
    }
}
