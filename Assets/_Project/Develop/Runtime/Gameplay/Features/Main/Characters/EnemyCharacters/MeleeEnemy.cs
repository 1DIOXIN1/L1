using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Core;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.WeaponFireType;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters
{
    public class MeleeEnemy : EnemyBase
    {
        private float _attackCooldown;
        private float _dashCooldown;
        private float _recoveryTimer;
        private bool _isDashing;

        protected override EnemyType Type => EnemyType.Melee;

        public override void TickCombat(float deltaTime)
        {
            _attackCooldown -= deltaTime;
            _dashCooldown -= deltaTime;
            _recoveryTimer -= deltaTime;

            if (_recoveryTimer > 0f)
            {
                StopAgent();
                LookAtPlayer(deltaTime);
                return;
            }

            float distance = DistanceToPlayer();

            if (_isDashing)
            {
                LookAtPlayer(deltaTime);

                if (Context.Agent.pathPending == false
                    && Context.Agent.remainingDistance <= Context.Agent.stoppingDistance + 0.15f)
                {
                    _isDashing = false;
                    Context.Agent.speed = Context.Preset.ChaseSpeed;
                }

                return;
            }

            if (distance > Context.Preset.AttackRange)
            {
                if (distance > Context.Preset.DashTriggerDistance && _dashCooldown <= 0f)
                {
                    StartDash();
                    return;
                }

                ChasePlayer(Context.Preset.AttackRange * 0.85f);
                LookAtPlayer(deltaTime);
                return;
            }

            StopAgent();
            LookAtPlayer(deltaTime);

            if (_attackCooldown > 0f)
                return;

            OverlapShoot.Shoot(GetAttackOrigin(), Context.Preset.AttackRange, Context.Preset.Damage, gameObject);
            _attackCooldown = Context.Preset.AttackCooldown;
            _recoveryTimer = Context.Preset.PostAttackPause;
        }

        public override void ResetCombat()
        {
            _attackCooldown = 0f;
            _dashCooldown = 0f;
            _recoveryTimer = 0f;
            _isDashing = false;
            Context.Agent.speed = Context.Preset.ChaseSpeed;
            StopAgent();
        }

        private void StartDash()
        {
            if (Context.Player == null)
                return;

            _isDashing = true;
            _dashCooldown = Context.Preset.DashCooldown;
            Context.Agent.isStopped = false;
            Context.Agent.speed = Context.Preset.DashSpeed;
            Context.Agent.stoppingDistance = Context.Preset.AttackRange * 0.85f;
            Context.Agent.SetDestination(Context.Player.position);
        }
    }
}
