using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Core;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.WeaponFireType;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters
{
    public class RangerEnemy : EnemyBase
    {
        private float _attackCooldown;
        private float _burstTimer;
        private int _burstShotsLeft;

        protected override EnemyType Type => EnemyType.Ranger;

        public override void TickCombat(float deltaTime)
        {
            _attackCooldown -= deltaTime;
            float distance = DistanceToPlayer();

            if (distance > Context.Preset.MaxAttackDistance)
                return;

            if (distance < Context.Preset.MinAttackDistance)
                return;

            if (_burstShotsLeft > 0)
            {
                _burstTimer -= deltaTime;

                if (_burstTimer <= 0f)
                {
                    TryShoot();
                    _burstShotsLeft--;
                    _burstTimer = Context.Preset.BurstInterval;
                }

                return;
            }

            if (_attackCooldown > 0f)
                return;

            _burstShotsLeft = Context.Preset.BurstShots;
            _attackCooldown = Context.Preset.AttackCooldown;
        }

        public override void ResetCombat()
        {
            _attackCooldown = 0f;
            _burstTimer = 0f;
            _burstShotsLeft = 0;
        }

        private void TryShoot()
        {
            if (Random.value > Context.Preset.Accuracy)
                return;

            if (Context.ProjectilePrefab == null)
                return;

            Vector3 origin = GetAttackOrigin() + transform.forward * 0.75f;
            GameObject projectileObject = Object.Instantiate(
                Context.ProjectilePrefab,
                origin,
                Quaternion.LookRotation(GetDirectionToPlayer()));

            if (projectileObject.TryGetComponent(out ProjectileShoot projectile))
            {
                projectile.Initialize(
                    GetDirectionToPlayer(),
                    Context.Preset.ProjectileSpeed,
                    Context.Preset.Damage,
                    Context.Preset.ProjectileLifeTime,
                    gameObject);
            }
        }
    }
}
