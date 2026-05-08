using _Project.Develop.Runtime.Configs.Meta.Enemy;
using _Project.Develop.Runtime.Gameplay.Features.Main.Controllers;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters
{
    public abstract class EnemyBase : Character
    {
        private CharacterControllerDirectionalMover _mover;
        private Vector3 _startPosition;
        private Vector3 _lastKnownTargetPosition;
        private Vector3 _suspiciousPosition;
        private bool _isInitialized;
        private float _detectionTimer;
        private float _lostTargetTimer;
        private float _searchTimer;
        private float _helpCallTimer;
        
        private EnemyConfig Config {get; set;}
        protected EnemyPreset Preset {get; private set;}
        protected Transform Target {get; private set;}
        protected GameObject ProjectilePrefab { get; private set; }
        private EnemyAIState State {get; set;}

        public void Initialize(
            CharacterControllerDirectionalMover mover,
            EnemyConfig config,
            EnemyType type,
            Transform target,
            GameObject projectilePrefab)
        {
            _mover = mover;
            Config = config;
            Preset = config.GetPreset(type);
            Target = target;
            ProjectilePrefab = projectilePrefab;
            _startPosition = transform.position;

            InitializeHealth(Preset);
            EnterState(EnemyAIState.Patrol);
            _isInitialized = true;
        }

        private void Update()
        {
            if (_isInitialized == false || IsDead)
                return;

            TickState(Time.deltaTime);
        }

        public void HearNoise(Vector3 position, float radius)
        {
            if (Vector3.Distance(transform.position, position) > radius)
                return;

            _suspiciousPosition = position;

            if (State is EnemyAIState.Patrol or EnemyAIState.Return)
                EnterState(EnemyAIState.Suspicious);
        }

        protected abstract void CombatTick(float deltaTime);

        protected virtual void OnStateEntered(EnemyAIState state)
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

        protected void SetMoveSpeed(float speed)
        {
            _mover.SetSpeed(speed);
        }

        protected void MoveTo(Vector3 position, float deltaTime)
        {
            Vector3 direction = position - transform.position;
            direction.y = 0f;

            RotateTowardsDirection(direction, deltaTime);
            _mover.SetDirectional(direction, deltaTime);
        }

        protected bool IsTargetInsideAttackDistance()
        {
            float distance = DistanceToTarget();
            return distance >= Preset.MinAttackDistance && distance <= Preset.MaxAttackDistance;
        }

        protected bool TryDamageTarget(int damage)
        {
            if (HasTarget() == false)
                return false;

            if (Target.TryGetComponent(out IDamageble damageble) == false)
                return false;

            damageble.TakeDamage(damage);
            return true;
        }

        protected Vector3 GetAttackOrigin()
        {
            return transform.position + Vector3.up * Preset.AttackOriginHeight;
        }

        protected Vector3 GetDirectionToTarget()
        {
            if (HasTarget() == false)
                return transform.forward;

            return (Target.position + Vector3.up * Preset.AttackOriginHeight - GetAttackOrigin()).normalized;
        }

        protected bool HasLineOfSightToTarget()
        {
            if (HasTarget() == false)
                return false;

            Vector3 origin = GetAttackOrigin();
            Vector3 direction = GetDirectionToTarget();
            float distance = Vector3.Distance(origin, Target.position + Vector3.up * Preset.AttackOriginHeight);

            if (Physics.Raycast(origin, direction, out RaycastHit hit, distance, ~0, QueryTriggerInteraction.Ignore) == false)
                return true;

            return hit.collider.transform.root == Target.root;
        }

        protected void RotateTowardsTarget(float deltaTime)
        {
            if (HasTarget() == false)
                return;

            Vector3 direction = Target.position - transform.position;
            direction.y = 0f;
            RotateTowardsDirection(direction, deltaTime);
        }

        private void RotateTowardsDirection(Vector3 direction, float deltaTime)
        {
            if (direction == Vector3.zero)
                return;

            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Preset.RotationSpeed * deltaTime);
        }

        private bool CanSeeTarget()
        {
            if (HasTarget() == false)
                return false;

            Vector3 direction = Target.position - transform.position;
            direction.y = 0f;

            if (direction.magnitude > Preset.ViewRadius)
                return false;

            float angle = Vector3.Angle(transform.forward, direction);
            return angle <= Preset.ViewAngle * 0.5f && HasLineOfSightToTarget();
        }

        private void TickState(float deltaTime)
        {
            bool canSeeTarget = CanSeeTarget();

            if (canSeeTarget)
            {
                _lastKnownTargetPosition = Target.position;
                _lostTargetTimer = 0f;
            }

            switch (State)
            {
                case EnemyAIState.Patrol:
                    TickPatrol(deltaTime, canSeeTarget);
                    break;
                case EnemyAIState.Suspicious:
                    TickSuspicious(deltaTime, canSeeTarget);
                    break;
                case EnemyAIState.Combat:
                    TickCombat(deltaTime, canSeeTarget);
                    break;
                case EnemyAIState.Search:
                    TickSearch(deltaTime, canSeeTarget);
                    break;
                case EnemyAIState.Return:
                    TickReturn(deltaTime, canSeeTarget);
                    break;
            }
        }

        private void TickPatrol(float deltaTime, bool canSeeTarget)
        {
            SetMoveSpeed(Preset.PatrolSpeed);

            if (canSeeTarget == false)
            {
                _detectionTimer = 0f;
                return;
            }

            _detectionTimer += deltaTime;

            if (_detectionTimer >= Config.DetectionFillTime)
                EnterState(EnemyAIState.Combat);
            else
                EnterState(EnemyAIState.Suspicious);
        }

        private void TickSuspicious(float deltaTime, bool canSeeTarget)
        {
            SetMoveSpeed(Preset.PatrolSpeed);

            if (canSeeTarget)
            {
                _detectionTimer += deltaTime;

                if (_detectionTimer >= Config.DetectionFillTime)
                    EnterState(EnemyAIState.Combat);

                return;
            }

            MoveTo(_suspiciousPosition, deltaTime);
            _searchTimer += deltaTime;

            if (_searchTimer >= Config.SearchDuration * 0.5f)
                EnterState(EnemyAIState.Return);
        }

        private void TickCombat(float deltaTime, bool canSeeTarget)
        {
            SetMoveSpeed(Preset.CombatSpeed);

            if (canSeeTarget == false)
            {
                _lostTargetTimer += deltaTime;

                if (_lostTargetTimer >= Config.ForgetTime)
                {
                    EnterState(EnemyAIState.Search);
                    return;
                }
            }

            _helpCallTimer += deltaTime;
            if (_helpCallTimer >= Config.HelpCallDelay)
                _helpCallTimer = 0f;

            RotateTowardsTarget(deltaTime);
            CombatTick(deltaTime);
        }

        private void TickSearch(float deltaTime, bool canSeeTarget)
        {
            SetMoveSpeed(Preset.PatrolSpeed);

            if (canSeeTarget)
            {
                EnterState(EnemyAIState.Combat);
                return;
            }

            MoveTo(_lastKnownTargetPosition, deltaTime);
            _searchTimer += deltaTime;

            if (_searchTimer >= Config.SearchDuration)
                EnterState(EnemyAIState.Return);
        }

        private void TickReturn(float deltaTime, bool canSeeTarget)
        {
            SetMoveSpeed(Preset.PatrolSpeed);

            if (canSeeTarget)
            {
                EnterState(EnemyAIState.Suspicious);
                return;
            }

            MoveTo(_startPosition, deltaTime);

            if (Vector3.Distance(transform.position, _startPosition) <= 0.5f)
                EnterState(EnemyAIState.Patrol);
        }

        private void EnterState(EnemyAIState state)
        {
            State = state;
            _searchTimer = 0f;

            if (state == EnemyAIState.Combat)
            {
                _detectionTimer = Config.DetectionFillTime;
                _helpCallTimer = 0f;
            }

            OnStateEntered(state);
        }
    }
}
