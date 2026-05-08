using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.WeaponFireType;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters
{
    public class Enemy : EnemyBase
    {
        [SerializeField] private LineRenderer laserRenderer;
        private float _attackCooldown;
        private float _burstTimer;
        private int _burstShotsLeft;
        private float _laserTimer;
        private float _laserDamageTimer;
        private float _dashCooldown;
        private float _comboTimer;
        private int _comboHitsLeft;

        protected override void CombatTick(float deltaTime)
        {
            switch (Preset.Type)
            {
                case EnemyType.Teacher:
                    TeacherCombat(deltaTime);
                    break;
                case EnemyType.Cutter:
                    CutterCombat(deltaTime);
                    break;
                default:
                    GuardCombat(deltaTime);
                    break;
            }
        }

        protected override void OnStateEntered(EnemyAIState state)
        {
            if (state != EnemyAIState.Combat)
                ResetAttackState();
        }

        private void GuardCombat(float deltaTime)
        {
            _attackCooldown -= deltaTime;
            float distance = DistanceToTarget();
            
            if (distance == Preset.MinAttackDistance)
            {
                Vector3 dir = (transform.position - Target.position).normalized;
                MoveTo(transform.position + dir * (Preset.MinAttackDistance - distance), deltaTime);
                return;
            }
            
            else if (distance > Preset.MaxAttackDistance)
            {
                MoveTo(Target.position, deltaTime);
                return;
            }

            if (_burstShotsLeft > 0)
            {
                _burstTimer -= deltaTime;

                if (_burstTimer <= 0f)
                {
                    Debug.Log("Attacking  projectile");
                    
                    TryShootProjectileWithAccuracy();
                    _burstShotsLeft--;
                    _burstTimer = Preset.BurstInterval;
                }

                return;
            }

            if (_attackCooldown > 0f)
                return;

            _burstShotsLeft = Preset.BurstShots;
            _attackCooldown = Preset.AttackCooldown;
        }

        private void TeacherCombat(float deltaTime)
        {
            _attackCooldown -= deltaTime;
            Vector3 origin = GetAttackOrigin();
            Vector3 targetPos = Target.position + Vector3.up * Preset.AttackOriginHeight;

            float distance = DistanceToTarget();
            
            if (distance == Preset.MinAttackDistance)
            {
                Vector3 dir = (transform.position - Target.position).normalized;
                MoveTo(transform.position + dir * (Preset.MinAttackDistance - distance), deltaTime);
                return;
            }
            
            else if (distance > Preset.MaxAttackDistance)
            {
                ResetLaser();
                MoveTo(Target.position, deltaTime);
                return;
            }

            if (_attackCooldown > 0f)
                return;

            _laserTimer += deltaTime;

            if (_laserTimer < Preset.LaserAimTime)
                return;

            _laserDamageTimer += deltaTime;

            if (_laserDamageTimer >= 1f)
            {
                DrawLaser(origin, targetPos, 0.1f);
                TryRaycastLaserDamage(Preset.LaserDamagePerSecond);
                _laserDamageTimer = 0f;
            }

            if (_laserTimer < Preset.LaserPowerShotTime)
                return;
            
            DrawLaser(origin, targetPos, 0.1f);
            TryRaycastLaserDamage(Preset.LaserPowerShotDamage);
            _attackCooldown = Preset.AttackCooldown;
            ResetLaser();
        }

        private void CutterCombat(float deltaTime)
        {
            _dashCooldown -= deltaTime;
            _attackCooldown -= deltaTime;

            if (DistanceToTarget() > Preset.DashStartDistance && _dashCooldown <= 0f)
            {
                SetMoveSpeed(Preset.DashSpeed);
                MoveTo(Target.position, deltaTime);
                _dashCooldown = Preset.DashCooldown;
                return;
            }

            if (IsTargetInsideAttackDistance() == false)
            {
                MoveTo(Target.position, deltaTime);
                return;
            }

            if (_attackCooldown > 0f)
                return;

            if (_comboHitsLeft <= 0)
            {
                _comboHitsLeft = Preset.ComboHits;
                _comboTimer = 0f;
            }

            _comboTimer -= deltaTime;

            if (_comboTimer > 0f)
                return;
                
            TryOverlapMeleeDamage(Preset.Damage);
            _comboHitsLeft--;

            if (_comboHitsLeft <= 0)
                _attackCooldown = Preset.AttackCooldown;
            else
                _comboTimer = Preset.ComboDuration / Preset.ComboHits;
        }

        private void TryShootProjectileWithAccuracy()
        {
            if (Random.value > Preset.Accuracy)
                return;

            if (ProjectilePrefab == null)
            {
                TryDamageTarget(Preset.Damage);
                return;
            }

            Vector3 origin = GetAttackOrigin() + transform.forward * 0.75f;
            GameObject projectileObject = Object.Instantiate(ProjectilePrefab, origin, Quaternion.LookRotation(GetDirectionToTarget()));

            if (projectileObject.TryGetComponent(out ProjectileShoot projectile))
                projectile.Initialize(GetDirectionToTarget(), Preset.ProjectileSpeed, Preset.Damage, Preset.ProjectileLifeTime, gameObject);
        }

        private bool TryRaycastLaserDamage(int damage)
        {
            return RaycastShoot.Shoot(GetAttackOrigin(), GetDirectionToTarget(), Preset.MaxAttackDistance, damage, gameObject);
        }

        private bool TryOverlapMeleeDamage(int damage)
        {
            return OverlapShoot.Shoot(GetAttackOrigin(), Preset.MaxAttackDistance, damage, gameObject);
        }

        private void ResetAttackState()
        {
            _attackCooldown = 0f;
            _burstTimer = 0f;
            _burstShotsLeft = 0;
            _dashCooldown = 0f;
            _comboTimer = 0f;
            _comboHitsLeft = 0;
            ResetLaser();
        }

        private void ResetLaser()
        {
            _laserTimer = 0f;
            _laserDamageTimer = 0f;
        }
        private void DrawLaser(Vector3 origin, Vector3 target, float duration)
        {
            if (laserRenderer == null)
                return;

            laserRenderer.positionCount = 2;
            laserRenderer.SetPosition(0, origin);
            laserRenderer.SetPosition(1, target);
            
            Invoke(nameof(HideLaser), duration);
        }

        private void HideLaser()
        {
            if (laserRenderer == null)
                return;

            laserRenderer.positionCount = 0;
        }
    }
}
