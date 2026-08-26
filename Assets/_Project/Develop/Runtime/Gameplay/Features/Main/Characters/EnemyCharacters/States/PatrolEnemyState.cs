using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Detection;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Core
{
    public sealed class PatrolEnemyState : IEnemyState
    {
        public EnemyStateId Id => EnemyStateId.Patrol;

        public void Enter(EnemyContext context)
        {
            context.IsAlarmResponder = false;
            context.Agent.isStopped = false;
            context.Agent.speed = context.Preset.PatrolSpeed;
            context.Agent.stoppingDistance = context.Config.PatrolPointReachDistance;
            context.PatrolWaitTimer = 0f;
            MoveToCurrentPatrolPoint(context);
        }

        public void Exit(EnemyContext context)
        {
        }

        public void Tick(EnemyContext context, float deltaTime)
        {
            bool canSee = context.Enemy.CanSeePlayer();
            context.Awareness.TickSight(canSee, deltaTime);

            if (canSee || context.Awareness.Phase != DetectionPhase.Calm)
            {
                if (canSee && context.Player != null)
                {
                    context.InvestigatePosition = context.Player.position;
                    context.HasInvestigateTarget = true;
                }

                context.Enemy.EnterDetecting();
                return;
            }

            RotateToMovement(context, deltaTime);

            if (context.PatrolPoints == null || context.PatrolPoints.Count == 0)
                return;

            if (context.Agent.pathPending)
                return;

            if (context.Agent.remainingDistance > context.Agent.stoppingDistance)
                return;

            context.PatrolWaitTimer -= deltaTime;
            if (context.PatrolWaitTimer > 0f)
                return;

            context.PatrolPointIndex = (context.PatrolPointIndex + 1) % context.PatrolPoints.Count;
            context.PatrolWaitTimer = context.Config.PatrolPointWaitTime;
            MoveToCurrentPatrolPoint(context);
        }

        private static void MoveToCurrentPatrolPoint(EnemyContext context)
        {
            Transform point = context.PatrolPoints[context.PatrolPointIndex];
            if (point != null)
                context.Agent.SetDestination(point.position);
        }

        private static void RotateToMovement(EnemyContext context, float deltaTime)
        {
            Vector3 direction = context.Agent.velocity;
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.05f && context.Agent.hasPath)
            {
                direction = context.Agent.steeringTarget - context.Enemy.transform.position;
                direction.y = 0f;
            }

            if (direction.sqrMagnitude > 0.05f)
                context.Enemy.RotateTowards(direction, deltaTime);
        }
    }
}
