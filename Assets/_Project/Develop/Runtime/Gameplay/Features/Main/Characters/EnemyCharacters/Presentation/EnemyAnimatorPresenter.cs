using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Core;
using UnityEngine;
using UnityEngine.AI;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Presentation
{
    public sealed class EnemyAnimatorPresenter
    {
        private const float MovingSpeedThreshold = 0.05f;

        private readonly Animator _animator;
        private readonly NavMeshAgent _agent;
        private readonly bool _hasSpeed;
        private readonly bool _hasIsMoving;
        private readonly bool _hasIsAlerted;
        private readonly bool _hasAlertLevel;
        private readonly bool _hasState;
        private readonly bool _hasAttack;
        private readonly bool _hasAttackFire;
        private readonly bool _hasHit;
        private readonly bool _hasDie;

        public EnemyAnimatorPresenter(Animator animator, NavMeshAgent agent)
        {
            _animator = animator;
            _agent = agent;

            _hasSpeed = HasParameter(EnemyAnimatorParameters.SpeedHash);
            _hasIsMoving = HasParameter(EnemyAnimatorParameters.IsMovingHash);
            _hasIsAlerted = HasParameter(EnemyAnimatorParameters.IsAlertedHash);
            _hasAlertLevel = HasParameter(EnemyAnimatorParameters.AlertLevelHash);
            _hasState = HasParameter(EnemyAnimatorParameters.StateHash);
            _hasAttack = HasParameter(EnemyAnimatorParameters.AttackHash);
            _hasAttackFire = HasParameter(EnemyAnimatorParameters.AttackFireHash);
            _hasHit = HasParameter(EnemyAnimatorParameters.HitHash);
            _hasDie = HasParameter(EnemyAnimatorParameters.DieHash);
        }

        public void Tick(EnemyContext context)
        {
            if (_animator.isActiveAndEnabled == false)
                return;

            float speed = 0f;
            if (_agent.enabled && _agent.isOnNavMesh)
                speed = _agent.velocity.magnitude;

            if (_hasSpeed)
                _animator.SetFloat(EnemyAnimatorParameters.SpeedHash, speed);

            if (_hasIsMoving)
                _animator.SetBool(EnemyAnimatorParameters.IsMovingHash, speed > MovingSpeedThreshold);

            if (_hasIsAlerted)
                _animator.SetBool(EnemyAnimatorParameters.IsAlertedHash, context.Awareness.IsAlerted);

            if (_hasAlertLevel)
                _animator.SetFloat(EnemyAnimatorParameters.AlertLevelHash, context.Awareness.Meter);

            if (_hasState)
                _animator.SetInteger(EnemyAnimatorParameters.StateHash, (int)context.Enemy.StateMachine.CurrentStateId);
        }

        public void PlayAttackStarted()
        {
            if (_hasAttack)
                _animator.SetTrigger(EnemyAnimatorParameters.AttackHash);
        }

        public void PlayAttackFired()
        {
            if (_hasAttackFire)
                _animator.SetTrigger(EnemyAnimatorParameters.AttackFireHash);
        }

        public void PlayAttackStopped()
        {
            if (_hasAttack)
                _animator.ResetTrigger(EnemyAnimatorParameters.AttackHash);

            if (_hasAttackFire)
                _animator.ResetTrigger(EnemyAnimatorParameters.AttackFireHash);
        }

        public void PlayHit()
        {
            if (_hasHit)
                _animator.SetTrigger(EnemyAnimatorParameters.HitHash);
        }

        public void PlayDie()
        {
            if (_hasDie)
                _animator.SetTrigger(EnemyAnimatorParameters.DieHash);
        }

        private bool HasParameter(int hash)
        {
            if (_animator.runtimeAnimatorController == null)
                return false;

            AnimatorControllerParameter[] parameters = _animator.parameters;
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].nameHash == hash)
                    return true;
            }

            return false;
        }
    }
}
