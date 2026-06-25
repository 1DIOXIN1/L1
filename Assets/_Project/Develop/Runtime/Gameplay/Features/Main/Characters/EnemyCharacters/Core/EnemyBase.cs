using System.Collections.Generic;
using _Project.Develop.Runtime.Configs.Meta.Enemy;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Core;
using UnityEngine;
using UnityEngine.AI;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class EnemyBase : Character
    {
        private EnemyContext _context;
        private bool _isInitialized;

        public EnemyStateMachine StateMachine { get; private set; }
        public bool IsAlive => IsDead == false;
        protected EnemyContext Context => _context;
        protected abstract EnemyType Type { get; }

        public void Initialize(
            EnemyAIService aiService,
            EnemyConfig config,
            Transform player,
            GameObject projectilePrefab,
            IReadOnlyList<Transform> patrolPoints)
        {
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            EnemyPreset preset = config.GetPreset(Type);

            ConfigureAgent(agent, preset);

            _context = new EnemyContext(
                this,
                player,
                config,
                preset,
                agent,
                aiService,
                projectilePrefab,
                patrolPoints);

            StateMachine = new EnemyStateMachine();
            RegisterStates(StateMachine);
            StateMachine.Initialize(_context, EnemyStateId.Patrol);

            InitializeHealth(preset);
            aiService.Register(this);
            _isInitialized = true;
        }

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

        public abstract void TickCombat(float deltaTime);

        public abstract void ResetCombat();

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

        protected void ChasePlayer(float stoppingDistance)
        {
            if (_context?.Player == null)
                return;

            NavMeshAgent agent = _context.Agent;
            agent.isStopped = false;
            agent.speed = _context.Preset.ChaseSpeed;
            agent.stoppingDistance = stoppingDistance;
            agent.SetDestination(_context.Player.position);
        }

        protected void MoveAwayFromPlayer(float desiredDistance)
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

        protected void StopAgent()
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

        protected Vector3 GetAttackOrigin()
        {
            return transform.position + Vector3.up * _context.Preset.AttackOriginHeight;
        }

        protected Vector3 GetDirectionToPlayer()
        {
            if (_context?.Player == null)
                return transform.forward;

            Vector3 target = _context.Player.position + Vector3.up * _context.Preset.AttackOriginHeight;
            return (target - GetAttackOrigin()).normalized;
        }

        protected float DistanceToPlayer()
        {
            if (_context?.Player == null)
                return float.MaxValue;

            return Vector3.Distance(transform.position, _context.Player.position);
        }

        protected override void Die()
        {
            if (_context != null)
                _context.AIService.Unregister(this);

            base.Die();
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

        private static void ConfigureAgent(NavMeshAgent agent, EnemyPreset preset)
        {
            agent.speed = preset.PatrolSpeed;
            agent.angularSpeed = preset.AgentAngularSpeed;
            agent.acceleration = preset.AgentAcceleration;
            agent.stoppingDistance = 0.5f;
            agent.autoBraking = true;
            agent.updateRotation = false;
        }

        protected bool HasLineOfSightToPlayer()
        {
            Vector3 origin = GetAttackOrigin();
            Vector3 direction = GetDirectionToPlayer();
            float distance = Vector3.Distance(origin, _context.Player.position + Vector3.up * _context.Preset.AttackOriginHeight);

            if (Physics.Raycast(origin, direction, out RaycastHit hit, distance, ~0, QueryTriggerInteraction.Ignore) == false)
                return true;

            return hit.collider.transform.root == _context.Player.root;
        }
    }
}
