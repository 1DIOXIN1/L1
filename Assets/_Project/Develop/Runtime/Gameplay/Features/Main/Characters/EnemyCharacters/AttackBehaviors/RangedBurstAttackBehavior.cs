using _Project.Develop.Runtime.Configs.Meta.Enemy.AttackBehaviors;
using _Project.Develop.Runtime.Gameplay.Features.Main.Impact;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.WeaponFireType;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.AttackBehaviors
{
    public sealed class RangedBurstAttackBehavior : IEnemyAttackBehavior, IAnimFireReceiver
    {
        private readonly EnemyBase _enemy;
        private readonly RangedBurstBehaviorConfig _config;
        private readonly IEnemyAttackPresentation _presentation;
        private readonly HitImpactService _hitImpacts;
        private float _attackCooldown;
        private float _burstTimer;
        private float _animFireTimeout;
        private int _burstShotsLeft;
        private bool _awaitingAnimFire;
        private bool _waitingAttackEnd;

        public RangedBurstAttackBehavior(
            EnemyBase enemy,
            RangedBurstBehaviorConfig config,
            IEnemyAttackPresentation presentation,
            HitImpactService hitImpacts)
        {
            _enemy = enemy;
            _config = config;
            _presentation = presentation;
            _hitImpacts = hitImpacts;
        }

        private bool IsAttackInProgress =>
            _awaitingAnimFire || _burstShotsLeft > 0 || _waitingAttackEnd;

        public void Tick(float deltaTime)
        {
            _attackCooldown -= deltaTime;
            float distance = _enemy.DistanceToPlayer();
            float shootDistance = (_config.MinAttackDistance + _config.MaxAttackDistance) * 0.5f;

            if (IsAttackInProgress)
            {
                _enemy.StopAgent();
                _enemy.LookAtPlayer(deltaTime);

                bool outOfRange = distance > _config.MaxAttackDistance ||
                                  distance < _config.MinAttackDistance;
                if (outOfRange)
                {
                    CancelAttack();
                    _attackCooldown = _config.AttackCooldown;
                }
                else
                {
                    TickAttack(deltaTime);
                    return;
                }
            }

            if (distance > _config.MaxAttackDistance)
            {
                _enemy.ChasePlayer(shootDistance);
                _enemy.LookAtPlayer(deltaTime);
                return;
            }

            if (distance < _config.MinAttackDistance)
            {
                _enemy.MoveAwayFromPlayer(_config.MinAttackDistance + 0.5f);
                _enemy.LookAtPlayer(deltaTime);
                return;
            }

            _enemy.StopAgent();
            _enemy.LookAtPlayer(deltaTime);

            if (_attackCooldown > 0f)
                return;

            BeginAttack();
        }

        public void Reset()
        {
            _attackCooldown = 0f;
            CancelAttack();
            _enemy.StopAgent();
        }

        public void OnAnimFire()
        {
            if (_awaitingAnimFire == false)
                return;

            _awaitingAnimFire = false;
            FireNextShot();
        }

        private void TickAttack(float deltaTime)
        {
            if (_awaitingAnimFire)
            {
                _animFireTimeout -= deltaTime;
                if (_animFireTimeout <= 0f)
                    OnAnimFire();
                return;
            }

            if (_burstShotsLeft > 0)
            {
                _burstTimer -= deltaTime;

                if (_burstTimer <= 0f)
                    FireNextShot();

                return;
            }

            if (_waitingAttackEnd && _enemy.AnimatorPresenter.IsPlayingAttack() == false)
                CompleteAttackCycle();
        }

        private void BeginAttack()
        {
            _presentation.NotifyAttackStarted();
            _awaitingAnimFire = true;
            _waitingAttackEnd = false;
            _animFireTimeout = _config.AnimFireTimeout;
            _burstShotsLeft = _config.BurstShots;
        }

        private void CancelAttack()
        {
            _burstTimer = 0f;
            _animFireTimeout = 0f;
            _burstShotsLeft = 0;
            _awaitingAnimFire = false;
            _waitingAttackEnd = false;
            _presentation.NotifyAttackStopped();
        }

        private void FireNextShot()
        {
            if (_burstShotsLeft <= 0)
                return;

            TryShoot();
            _burstShotsLeft--;

            if (_burstShotsLeft > 0)
            {
                _burstTimer = _config.BurstInterval;
                return;
            }

            _waitingAttackEnd = true;
        }

        private void CompleteAttackCycle()
        {
            _waitingAttackEnd = false;
            _attackCooldown = _config.AttackCooldown;
            _presentation.NotifyAttackStopped();
        }

        private void TryShoot()
        {
            if (Random.value > _config.Accuracy)
                return;

            if (_enemy.Context.ProjectilePrefab == null)
                return;

            Vector3 origin = _enemy.GetAttackOrigin();
            GameObject projectileObject = Object.Instantiate(
                _enemy.Context.ProjectilePrefab,
                origin,
                Quaternion.LookRotation(_enemy.GetDirectionToPlayer()));

            if (projectileObject.TryGetComponent(out ProjectileShoot projectile))
            {
                projectile.Initialize(
                    _enemy.GetDirectionToPlayer(),
                    _config.ProjectileSpeed,
                    _config.Damage,
                    _config.ProjectileLifeTime,
                    _enemy,
                    _hitImpacts);
            }

            _presentation.NotifyAttackFired();
        }
    }
}
