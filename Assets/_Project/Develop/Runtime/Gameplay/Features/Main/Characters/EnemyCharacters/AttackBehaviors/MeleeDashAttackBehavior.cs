using System;
using _Project.Develop.Runtime.Configs.Meta.Enemy.AttackBehaviors;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.WeaponFireType;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.AttackBehaviors
{
    public sealed class MeleeDashAttackBehavior : IEnemyAttackBehavior
    {
        private readonly EnemyBase _enemy;
        private readonly MeleeDashBehaviorConfig _config;
        private readonly IEnemyAttackPresentation _presentation;
        private float _attackCooldown;
        private float _dashCooldown;
        private float _recoveryTimer;
        private bool _isDashing;

        public MeleeDashAttackBehavior(
            EnemyBase enemy,
            MeleeDashBehaviorConfig config,
            IEnemyAttackPresentation presentation)
        {
            _enemy = enemy;
            _config = config;
            _presentation = presentation ?? throw new ArgumentNullException(nameof(presentation));
        }

        public void Tick(float deltaTime)
        {
            _attackCooldown -= deltaTime;
            _dashCooldown -= deltaTime;
            _recoveryTimer -= deltaTime;

            if (_recoveryTimer > 0f)
            {
                _enemy.StopAgent();
                _enemy.LookAtPlayer(deltaTime);
                return;
            }

            float distance = _enemy.DistanceToPlayer();

            if (_isDashing)
            {
                _enemy.LookAtPlayer(deltaTime);

                if (_enemy.Context.Agent.pathPending == false
                    && _enemy.Context.Agent.remainingDistance <= _enemy.Context.Agent.stoppingDistance + 0.15f)
                {
                    _isDashing = false;
                    _enemy.Context.Agent.speed = _enemy.Context.Preset.ChaseSpeed;
                }

                return;
            }

            if (distance > _config.AttackRange)
            {
                if (distance > _config.DashTriggerDistance && _dashCooldown <= 0f)
                {
                    StartDash();
                    return;
                }

                _enemy.ChasePlayer(_config.AttackRange * 0.85f);
                _enemy.LookAtPlayer(deltaTime);
                return;
            }

            _enemy.StopAgent();
            _enemy.LookAtPlayer(deltaTime);

            if (_attackCooldown > 0f)
                return;

            _presentation.NotifyAttackStarted();
            OverlapShoot.Shoot(
                _enemy.GetAttackOrigin(),
                _config.AttackRange,
                _config.Damage,
                _enemy.gameObject);
            _presentation.NotifyAttackFired();

            _attackCooldown = _config.AttackCooldown;
            _recoveryTimer = _config.PostAttackPause;
        }

        public void Reset()
        {
            _attackCooldown = 0f;
            _dashCooldown = 0f;
            _recoveryTimer = 0f;
            _isDashing = false;
            _presentation.NotifyAttackStopped();
            _enemy.Context.Agent.speed = _enemy.Context.Preset.ChaseSpeed;
            _enemy.StopAgent();
        }

        private void StartDash()
        {
            if (_enemy.Context.Player == null)
                return;

            _isDashing = true;
            _dashCooldown = _config.DashCooldown;
            _presentation.NotifyAttackStarted();
            _enemy.Context.Agent.isStopped = false;
            _enemy.Context.Agent.speed = _config.DashSpeed;
            _enemy.Context.Agent.stoppingDistance = _config.AttackRange * 0.85f;
            _enemy.Context.Agent.SetDestination(_enemy.Context.Player.position);
        }
    }
}
