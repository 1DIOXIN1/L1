using System;
using System.Collections.Generic;
using _Project.Develop.Runtime.Configs.Meta.Enemy;
using _Project.Develop.Runtime.Configs.Meta.Enemy.AttackBehaviors;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.AttackBehaviors;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Core;
using UnityEngine;
using UnityEngine.AI;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class EnemyBase : Character
    {
        private EnemyContext _context;
        private EnemyAttackBehaviorFactory _attackBehaviorFactory;
        private IEnemyAttackBehavior[] _attackBehaviors = Array.Empty<IEnemyAttackBehavior>();
        private IEnemyAttackBehavior _activeAttackBehavior;
        private bool _isInitialized;

        public EnemyStateMachine StateMachine { get; private set; }
        public bool IsAlive => IsDead == false;
        public EnemyContext Context => _context;
        protected abstract EnemyType Type { get; }

        public void Initialize(
            EnemyAIService aiService,
            EnemyConfig config,
            Transform player,
            GameObject projectilePrefab,
            IReadOnlyList<Transform> patrolPoints,
            EnemyAttackBehaviorFactory attackBehaviorFactory)
        {
            if (attackBehaviorFactory == null)
                throw new ArgumentNullException(nameof(attackBehaviorFactory));

            _attackBehaviorFactory = attackBehaviorFactory;

            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            EnemyPreset preset = config.GetPreset(Type);

            ConfigureAgent(agent, preset);
            EnsureDamageCollider(agent);

            _context = new EnemyContext(
                this,
                player,
                config,
                preset,
                agent,
                aiService,
                projectilePrefab,
                patrolPoints);

            CreateAttackBehaviors(preset);

            StateMachine = new EnemyStateMachine();
            RegisterStates(StateMachine);
            StateMachine.Initialize(_context, EnemyStateId.Patrol);

            InitializeHealth(preset);
            aiService.Register(this);
            _isInitialized = true;
        }

        protected virtual IEnemyAttackPresentation CreatePresentation()
            => new DefaultEnemyAttackPresentation();

        protected virtual void RegisterStates(EnemyStateMachine stateMachine)
        {
            stateMachine.RegisterState(new PatrolEnemyState());
            stateMachine.RegisterState(new InfiltrationEnemyState());
        }

        public void EnterCombatAsSpotter()
        {
            if (_isInitialized == false || IsDead)
                return;

            if (StateMachine.CurrentStateId == EnemyStateId.Infiltration)
                return;

            _context.IsSpotter = true;
            _context.SpotterTimer = 0f;
            _context.AlarmSpreadTriggered = false;
            _context.InfiltrationTriggered = true;
            StateMachine.ChangeState(EnemyStateId.Infiltration);
        }

        public void EnterInfiltration()
        {
            if (_isInitialized == false || IsDead)
                return;

            if (StateMachine.CurrentStateId == EnemyStateId.Infiltration)
                return;

            _context.IsSpotter = false;
            _context.InfiltrationTriggered = true;
            StateMachine.ChangeState(EnemyStateId.Infiltration);
        }

        public void TickCombat(float deltaTime) => _activeAttackBehavior?.Tick(deltaTime);

        public void ResetCombat()
        {
            for (int i = 0; i < _attackBehaviors.Length; i++)
                _attackBehaviors[i]?.Reset();
        }

        public bool CanSeePlayer()
        {
            if (_context?.Player == null)
                return false;

            Vector3 direction = _context.Player.position - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > _context.Preset.ViewRadius * _context.Preset.ViewRadius)
                return false;

            float halfAngle = _context.Preset.ViewAngle * 0.5f;
            if (Vector3.Angle(transform.forward, direction) > halfAngle)
                return false;

            return HasLineOfSightToPlayer();
        }

        public void LookAtPlayer()
        {
            LookAtPlayer(Time.deltaTime);
        }

        public void LookAtPlayer(float deltaTime)
        {
            if (_context?.Player == null)
                return;

            Vector3 direction = _context.Player.position - transform.position;
            RotateTowards(direction, deltaTime);
        }

        public void ChasePlayer(float stoppingDistance)
        {
            if (_context?.Player == null)
                return;

            NavMeshAgent agent = _context.Agent;
            agent.isStopped = false;
            agent.speed = _context.Preset.ChaseSpeed;
            agent.stoppingDistance = stoppingDistance;
            agent.SetDestination(_context.Player.position);
        }

        public void MoveAwayFromPlayer(float desiredDistance)
        {
            if (_context?.Player == null)
                return;

            NavMeshAgent agent = _context.Agent;
            Vector3 away = transform.position - _context.Player.position;
            away.y = 0f;

            if (away.sqrMagnitude < 0.01f)
                away = -transform.forward;

            away.Normalize();
            Vector3 target = _context.Player.position + away * desiredDistance;

            agent.isStopped = false;
            agent.speed = _context.Preset.ChaseSpeed;
            agent.stoppingDistance = 0.25f;
            agent.SetDestination(target);
        }

        public void StopAgent()
        {
            if (_context?.Agent == null)
                return;

            _context.Agent.isStopped = true;

            if (_context.Agent.hasPath)
                _context.Agent.ResetPath();
        }

        public void RotateTowards(Vector3 direction, float deltaTime)
        {
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.01f)
                return;

            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                _context.Preset.RotationSpeed * deltaTime);
        }

        public Vector3 GetAttackOrigin()
        {
            return transform.position + Vector3.up * _context.Preset.AttackOriginHeight;
        }

        public Vector3 GetDirectionToPlayer()
        {
            if (_context?.Player == null)
                return transform.forward;

            Vector3 target = _context.Player.position + Vector3.up * _context.Preset.AttackOriginHeight;
            return (target - GetAttackOrigin()).normalized;
        }

        public float DistanceToPlayer()
        {
            if (_context?.Player == null)
                return float.MaxValue;

            return Vector3.Distance(transform.position, _context.Player.position);
        }

        public bool HasLineOfSightToPlayer()
        {
            Vector3 origin = GetAttackOrigin();
            Vector3 direction = GetDirectionToPlayer();
            float distance = Vector3.Distance(origin, _context.Player.position + Vector3.up * _context.Preset.AttackOriginHeight);

            if (Physics.Raycast(origin, direction, out RaycastHit hit, distance, ~0, QueryTriggerInteraction.Ignore) == false)
                return true;

            return hit.collider.transform.root == _context.Player.root;
        }

        protected override void Die()
        {
            if (_context != null)
            {
                _context.Agent.enabled = false;
                _context.AIService.Unregister(this);
            }

            base.Die();
        }

        private void CreateAttackBehaviors(EnemyPreset preset)
        {
            IReadOnlyList<EnemyAttackBehaviorConfig> configs = preset.AttackBehaviors;

            if (configs == null || configs.Count == 0)
                throw new InvalidOperationException(
                    $"Enemy preset '{preset.Id}' has no {nameof(EnemyPreset.AttackBehaviors)} configured.");

            IEnemyAttackPresentation presentation = CreatePresentation();
            if (presentation == null)
                throw new InvalidOperationException(
                    $"{GetType().Name} returned null from {nameof(CreatePresentation)}.");

            _attackBehaviors = new IEnemyAttackBehavior[configs.Count];

            for (int i = 0; i < configs.Count; i++)
            {
                EnemyAttackBehaviorConfig config = configs[i];
                if (config == null)
                    throw new InvalidOperationException(
                        $"Enemy preset '{preset.Id}' has a null attack behavior at index {i}.");

                _attackBehaviors[i] = _attackBehaviorFactory.Create(config, this, presentation);
            }

            _activeAttackBehavior = _attackBehaviors[0];
        }

        private void Update()
        {
            if (_isInitialized == false || IsDead)
                return;

            StateMachine.Tick(Time.deltaTime);
        }

        private void OnDestroy()
        {
            if (_context?.AIService != null)
                _context.AIService.Unregister(this);
        }

        private void EnsureDamageCollider(NavMeshAgent agent)
        {
            if (TryGetComponent<Collider>(out _))
                return;

            CapsuleCollider collider = gameObject.AddComponent<CapsuleCollider>();
            collider.isTrigger = true;
            collider.height = Mathf.Max(agent.height, 1f);
            collider.radius = Mathf.Max(agent.radius, 0.3f);
            collider.center = new Vector3(0f, agent.baseOffset, 0f);
        }

        private static void ConfigureAgent(NavMeshAgent agent, EnemyPreset preset)
        {
            agent.speed = preset.PatrolSpeed;
            agent.angularSpeed = preset.AgentAngularSpeed;
            agent.acceleration = preset.AgentAcceleration;
            agent.stoppingDistance = 0.5f;
            agent.autoBraking = true;
            agent.updateRotation = false;
        }
    }
}
