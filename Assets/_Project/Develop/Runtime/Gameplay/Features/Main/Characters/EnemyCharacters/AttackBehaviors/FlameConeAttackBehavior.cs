using System;
using _Project.Develop.Runtime.Configs.Meta.Enemy.AttackBehaviors;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.WeaponFireType;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.AttackBehaviors
{
    public sealed class FlameConeAttackBehavior : IEnemyAttackBehavior
    {
        private readonly EnemyBase _enemy;
        private readonly FlameConeBehaviorConfig _config;
        private readonly IEnemyAttackPresentation _presentation;

        private Transform _flameMuzzle;
        private ParticleSystem _flameParticlesInstance;
        private float _flameTimer;
        private float _flameDamageTimer;
        private bool _isFlameActive;

        public FlameConeAttackBehavior(
            EnemyBase enemy,
            FlameConeBehaviorConfig config,
            IEnemyAttackPresentation presentation)
        {
            _enemy = enemy;
            _config = config;
            _presentation = presentation ?? throw new ArgumentNullException(nameof(presentation));
            EnsureFlameParticles();
        }

        public void Tick(float deltaTime)
        {
            float distance = _enemy.DistanceToPlayer();
            float minDistance = _config.MinAttackDistance;
            float maxDistance = _config.MaxAttackDistance;
            float engageDistance = Mathf.Min(_config.FlameRange * 0.65f, maxDistance * 0.85f);
            engageDistance = Mathf.Max(engageDistance, minDistance + 0.35f);

            if (distance > maxDistance)
            {
                ResetFlame();
                _enemy.ChasePlayer(engageDistance);
                _enemy.LookAtPlayer(deltaTime);
                return;
            }

            if (distance < minDistance)
            {
                ResetFlame();
                _enemy.MoveAwayFromPlayer(minDistance + 0.5f);
                _enemy.LookAtPlayer(deltaTime);
                return;
            }

            if (_enemy.HasLineOfSightToPlayer() == false)
            {
                ResetFlame();
                _enemy.ChasePlayer(engageDistance);
                _enemy.LookAtPlayer(deltaTime);
                return;
            }

            _enemy.StopAgent();
            _enemy.LookAtPlayer(deltaTime);
            AlignFlameToShotDirection();

            _flameTimer += deltaTime;

            if (_flameTimer < _config.FlameWarmupTime)
                return;

            SetFlameActive(true);
            _flameDamageTimer += deltaTime;

            if (_flameDamageTimer >= 1f)
            {
                FlameConeShoot.Shoot(
                    _enemy.GetAttackOrigin(),
                    _enemy.GetDirectionToPlayer(),
                    _config.FlameRange,
                    _config.FlameConeAngle,
                    _config.FlameDamagePerSecond,
                    _enemy);

                _presentation.NotifyAttackFired();
                _flameDamageTimer = 0f;
            }
        }

        public void Reset()
        {
            ResetFlame();
            _enemy.StopAgent();
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

            if (isActive)
            {
                _presentation.NotifyAttackStarted();
                AlignFlameToShotDirection();
                _flameParticlesInstance?.Play(true);
            }
            else
            {
                _presentation.NotifyAttackStopped();
                _flameParticlesInstance?.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

        private void AlignFlameToShotDirection()
        {
            if (_flameMuzzle == null)
                return;

            Vector3 direction = _enemy.GetDirectionToPlayer();
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.001f)
                direction = _enemy.transform.forward;

            _flameMuzzle.position = _enemy.GetAttackOrigin();
            _flameMuzzle.rotation = Quaternion.LookRotation(direction.normalized);
        }

        private void EnsureFlameParticles()
        {
            if (_flameParticlesInstance != null)
                return;

            ParticleSystem source = _presentation.FlameParticlesSource;
            if (source == null)
                return;

            Transform sourceTransform = source.transform;

            if (source.gameObject.scene.IsValid() && source.transform.IsChildOf(_enemy.transform))
            {
                _flameParticlesInstance = source;
                AttachParticleToMuzzle(_flameParticlesInstance.transform);
            }
            else
            {
                _flameMuzzle = CreateMuzzle();
                _flameParticlesInstance = Object.Instantiate(source, _flameMuzzle);
                Transform instanceTransform = _flameParticlesInstance.transform;
                instanceTransform.localPosition = Vector3.zero;
                instanceTransform.localRotation = Quaternion.identity;
                instanceTransform.localScale = sourceTransform.localScale;
            }

            ConfigureParticleInstance(_flameParticlesInstance);
        }

        private void AttachParticleToMuzzle(Transform particleTransform)
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
            muzzle.SetParent(_enemy.transform, false);
            return muzzle;
        }

        private static void ConfigureParticleInstance(ParticleSystem particleSystem)
        {
            ParticleSystem[] systems = particleSystem.GetComponentsInChildren<ParticleSystem>(true);

            for (int i = 0; i < systems.Length; i++)
            {
                ParticleSystem system = systems[i];
                ParticleSystem.MainModule main = system.main;
                main.playOnAwake = false;
                main.simulationSpace = ParticleSystemSimulationSpace.Local;
                main.emitterVelocityMode = ParticleSystemEmitterVelocityMode.Transform;

                ParticleSystem.VelocityOverLifetimeModule velocity = system.velocityOverLifetime;
                velocity.enabled = false;

                ParticleSystem.InheritVelocityModule inheritVelocity = system.inheritVelocity;
                inheritVelocity.enabled = false;

                ParticleSystem.CollisionModule collision = system.collision;
                collision.enabled = false;

                system.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }
    }
}
