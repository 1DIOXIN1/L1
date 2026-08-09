using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Core;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.WeaponFireType;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters
{
    public class CookerEnemy : EnemyBase
    {
        [SerializeField] private ParticleSystem flameParticles;

        private Transform _flameMuzzle;
        private ParticleSystem _flameParticlesInstance;
        private float _flameTimer;
        private float _flameDamageTimer;
        private bool _isFlameActive;

        protected override EnemyType Type => EnemyType.Cooker;

        private void Awake()
        {
            EnsureFlameParticles();
        }

        public override void TickCombat(float deltaTime)
        {
            float distance = DistanceToPlayer();
            float minDistance = Context.Preset.MinAttackDistance;
            float maxDistance = Context.Preset.MaxAttackDistance;
            float idealDistance = (minDistance + maxDistance) * 0.5f;

            if (distance > maxDistance)
            {
                ResetFlame();
                ChasePlayer(idealDistance);
                LookAtPlayer(deltaTime);
                return;
            }

            if (distance < minDistance)
            {
                ResetFlame();
                MoveAwayFromPlayer(minDistance + 0.5f);
                LookAtPlayer(deltaTime);
                return;
            }

            if (HasLineOfSightToPlayer() == false)
            {
                ResetFlame();
                ChasePlayer(idealDistance);
                LookAtPlayer(deltaTime);
                return;
            }

            StopAgent();
            LookAtPlayer(deltaTime);
            AlignFlameToShotDirection();

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
            StopAgent();
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
            EnsureFlameParticles();

            if (_flameParticlesInstance == null)
                return;

            if (isActive)
            {
                AlignFlameToShotDirection();
                _flameParticlesInstance.Play(true);
            }
            else
                _flameParticlesInstance.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        private void AlignFlameToShotDirection()
        {
            if (_flameMuzzle == null)
                return;

            Vector3 direction = transform.forward;
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.001f)
                direction = Vector3.forward;

            _flameMuzzle.position = GetAttackOrigin();
            _flameMuzzle.rotation = Quaternion.LookRotation(direction.normalized);
        }

        private void EnsureFlameParticles()
        {
            if (_flameParticlesInstance != null)
                return;

            if (flameParticles == null)
                return;

            Transform sourceTransform = flameParticles.transform;

            if (flameParticles.gameObject.scene.IsValid()
                && flameParticles.transform.IsChildOf(transform))
            {
                _flameParticlesInstance = flameParticles;
                AttachParticleToMuzzle(_flameParticlesInstance.transform, sourceTransform);
            }
            else
            {
                _flameMuzzle = CreateMuzzle();
                _flameParticlesInstance = Instantiate(flameParticles, _flameMuzzle);
                ApplyPrefabLocalTransform(sourceTransform, _flameParticlesInstance.transform);
            }

            ConfigureParticleInstance(_flameParticlesInstance);
        }

        private void AttachParticleToMuzzle(Transform particleTransform, Transform sourceTransform)
        {
            Vector3 localPosition = particleTransform.localPosition;
            Quaternion localRotation = particleTransform.localRotation;
            Vector3 localScale = particleTransform.localScale;

            _flameMuzzle = CreateMuzzle();
            particleTransform.SetParent(_flameMuzzle, false);
            particleTransform.localPosition = localPosition;
            particleTransform.localRotation = localRotation;
            particleTransform.localScale = localScale;
        }

        private Transform CreateMuzzle()
        {
            Transform muzzle = new GameObject("FlameMuzzle").transform;
            muzzle.SetParent(transform, false);
            return muzzle;
        }

        private static void ApplyPrefabLocalTransform(Transform source, Transform target)
        {
            target.localPosition = source.localPosition;
            target.localRotation = source.localRotation;
            target.localScale = source.localScale;
        }

        private static void ConfigureParticleInstance(ParticleSystem particleSystem)
        {
            ParticleSystem.MainModule main = particleSystem.main;
            main.playOnAwake = false;
            main.simulationSpace = ParticleSystemSimulationSpace.Local;
            main.emitterVelocityMode = ParticleSystemEmitterVelocityMode.Transform;
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}
