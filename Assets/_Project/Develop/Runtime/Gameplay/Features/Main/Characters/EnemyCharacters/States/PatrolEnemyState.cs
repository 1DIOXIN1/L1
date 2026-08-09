using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Core
{
    public sealed class PatrolEnemyState : IEnemyState
    {
        public EnemyStateId Id => EnemyStateId.Patrol;

        public void Enter(EnemyContext context)
        {
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
            if (context.Enemy.CanSeePlayer())
            {
                context.Enemy.EnterCombatAsSpotter();
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
