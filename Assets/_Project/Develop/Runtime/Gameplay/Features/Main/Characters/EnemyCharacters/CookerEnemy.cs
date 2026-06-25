using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Core;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.WeaponFireType;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters
{
    public class CookerEnemy : EnemyBase
    {
        [SerializeField] private ParticleSystem flameParticles;

        private float _flameTimer;
        private float _flameDamageTimer;
        private bool _isFlameActive;

        protected override EnemyType Type => EnemyType.Cooker;

        public override void TickCombat(float deltaTime)
        {
            float distance = DistanceToPlayer();

            if (distance < Context.Preset.MinAttackDistance)
            {
                ResetFlame();
                return;
            }

            if (distance > Context.Preset.MaxAttackDistance || HasLineOfSightToPlayer() == false)
            {
                ResetFlame();
                return;
            }

            _flameTimer += deltaTime;

            if (_flameTimer < Context.Preset.FlameWarmupTime)
                return;

            SetFlameActive(true);
            _flameDamageTimer += deltaTime;

            if (_flameDamageTimer >= 1f)
            {
                FlameConeShoot.Shoot(
                    GetAttackOrigin(),
                    transform.forward,
                    Context.Preset.FlameRange,
                    Context.Preset.FlameConeAngle,
                    Context.Preset.FlameDamagePerSecond,
                    gameObject);

                _flameDamageTimer = 0f;
            }
        }

        public override void ResetCombat()
        {
            ResetFlame();
        }

        private bool HasLineOfSightToPlayer()
        {
            return CanSeePlayer();
        }

        private void ResetFlame()
        {
            _flameTimer = 0f;
            _flameDamageTimer = 0f;
            SetFlameActive(false);
        }

        private void SetFlameActive(bool isActive)
        {
            if (_isFlameActive == isActive)
                return;

            _isFlameActive = isActive;

            if (flameParticles == null)
                return;

            if (isActive)
                flameParticles.Play();
            else
                flameParticles.Stop();
        }
    }
}
