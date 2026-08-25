using System;
using _Project.Develop.Runtime.Configs.Meta.Enemy.AttackBehaviors;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.WeaponFireType;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.AttackBehaviors
{
    public sealed class RangedBurstAttackBehavior : IEnemyAttackBehavior
    {
        private readonly EnemyBase _enemy;
        private readonly RangedBurstBehaviorConfig _config;
        private readonly IEnemyAttackPresentation _presentation;
        private float _attackCooldown;
        private float _burstTimer;
        private int _burstShotsLeft;

        public RangedBurstAttackBehavior(
            EnemyBase enemy,
            RangedBurstBehaviorConfig config,
            IEnemyAttackPresentation presentation)
        {
            _enemy = enemy;
            _config = config;
            _presentation = presentation ?? throw new ArgumentNullException(nameof(presentation));
        }

        public void Tick(float deltaTime)
        {
            _attackCooldown -= deltaTime;
            float distance = _enemy.DistanceToPlayer();
            float shootDistance = (_config.MinAttackDistance + _config.MaxAttackDistance) * 0.5f;

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

            if (_burstShotsLeft > 0)
            {
                _burstTimer -= deltaTime;

                if (_burstTimer <= 0f)
                {
                    TryShoot();
                    _burstShotsLeft--;
                    _burstTimer = _config.BurstInterval;
                }

                return;
            }

            if (_attackCooldown > 0f)
                return;

            _presentation.NotifyAttackStarted();
            _burstShotsLeft = _config.BurstShots;
            _attackCooldown = _config.AttackCooldown;
        }

        public void Reset()
        {
            _attackCooldown = 0f;
            _burstTimer = 0f;
            _burstShotsLeft = 0;
            _presentation.NotifyAttackStopped();
            _enemy.StopAgent();
        }

        private void TryShoot()
        {
            if (UnityEngine.Random.value > _config.Accuracy)
                return;

            if (_enemy.Context.ProjectilePrefab == null)
                return;

            Vector3 origin = _enemy.GetAttackOrigin() + _enemy.transform.forward * 0.75f;
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
                    _enemy.gameObject);
            }

            _presentation.NotifyAttackFired();
        }
    }
}
