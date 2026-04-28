using _Project.Develop.Runtime.Configs.Meta.Enemy;
using _Project.Develop.Runtime.Gameplay.Features.Main.Controllers;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters
{
    public abstract class EnemyBase : Character
    {
        protected EnemyConfig Config { get; private set; }
        private CharacterControllerDirectionalMover Mover { get; set; }
        protected Transform Target { get; private set; }

        private bool _isInitialized;

        public void Initialize(
            CharacterControllerDirectionalMover mover,
            EnemyConfig config,
            Transform target)
        {
            Mover = mover;
            Config = config;
            Target = target;

            InitializeHealth(config);
            _isInitialized = true;

            OnInitialized();
        }

        private void Update()
        {
            if (_isInitialized == false || IsDead)
                return;

            Tick(Time.deltaTime);
        }

        protected abstract void Tick(float deltaTime);

        protected virtual void OnInitialized()
        {
        }

        protected bool HasTarget()
        {
            return Target != null;
        }

        protected float DistanceToTarget()
        {
            if (HasTarget() == false)
                return float.MaxValue;

            return Vector3.Distance(transform.position, Target.position);
        }

        protected void MoveTo(Vector3 position, float deltaTime)
        {
            Vector3 direction = position - transform.position;
            direction.y = 0f;

            Mover.SetDirectional(direction, deltaTime);
        }

        protected bool TryDamageTarget()
        {
            if (HasTarget() == false)
                return false;

            if (Target.TryGetComponent(out IDamageble damageble) == false)
                return false;

            damageble.TakeDamage(Config.Damage);
            return true;
        }
    }
}
