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
        private readonly bool _hasPowerOff;

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
            _hasPowerOff = HasParameter(EnemyAnimatorParameters.PowerOffHash);
        }

        public void Tick(EnemyContext context)
        {
            if (_animator == null || _animator.isActiveAndEnabled == false)
                return;

            float speed = 0f;
            if (_agent.enabled && _agent.isOnNavMesh)
                speed = _agent.velocity.magnitude;

            if (_hasSpeed)
                _animator.SetFloat(EnemyAnimatorParameters.SpeedHash, speed);

            if (_hasIsMoving)
            {
                bool isMoving = speed > MovingSpeedThreshold && IsPlayingAttack() == false;
                _animator.SetBool(EnemyAnimatorParameters.IsMovingHash, isMoving);
            }

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

        public bool IsPlayingAttack()
        {
            if (_animator == null || _animator.isActiveAndEnabled == false)
                return false;

            AnimatorStateInfo current = _animator.GetCurrentAnimatorStateInfo(0);
            if (current.IsName("Attack"))
                return true;

            if (_animator.IsInTransition(0) == false)
                return false;

            return _animator.GetNextAnimatorStateInfo(0).IsName("Attack");
        }

        public void PlayHit()
        {
            if (_hasHit)
                _animator.SetTrigger(EnemyAnimatorParameters.HitHash);
        }

        public void PlayPowerOff()
        {
            if (_hasPowerOff)
                _animator.SetTrigger(EnemyAnimatorParameters.PowerOffHash);
        }

        private bool HasParameter(int hash)
        {
            if (_animator == null || _animator.runtimeAnimatorController == null)
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
