using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Core;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.WeaponFireType;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters
{
    public class MeleeEnemy : EnemyBase
    {
        private float _attackCooldown;
        private float _dashCooldown;
        private bool _isDashing;

        protected override EnemyType Type => EnemyType.Melee;

        public override void TickCombat(float deltaTime)
        {
            _attackCooldown -= deltaTime;
            _dashCooldown -= deltaTime;

            float distance = DistanceToPlayer();

            if (_isDashing == false
                && distance > Context.Preset.DashTriggerDistance
                && _dashCooldown <= 0f)
            {
                StartDash();
                return;
            }

            if (_isDashing)
            {
                if (Context.Agent.remainingDistance <= Context.Agent.stoppingDistance)
                    _isDashing = false;

                return;
            }

            if (distance > Context.Preset.AttackRange)
                return;

            if (_attackCooldown > 0f)
                return;

            OverlapShoot.Shoot(GetAttackOrigin(), Context.Preset.AttackRange, Context.Preset.Damage, gameObject);
            _attackCooldown = Context.Preset.AttackCooldown;
        }

        public override void ResetCombat()
        {
            _attackCooldown = 0f;
            _dashCooldown = 0f;
            _isDashing = false;
            Context.Agent.speed = Context.Preset.ChaseSpeed;
        }

        private void StartDash()
        {
            if (Context.Player == null)
                return;

            _isDashing = true;
            _dashCooldown = Context.Preset.DashCooldown;
            Context.Agent.speed = Context.Preset.DashSpeed;
            Context.Agent.stoppingDistance = Context.Preset.AttackRange * 0.5f;
            Context.Agent.SetDestination(Context.Player.position);
        }
    }
}
