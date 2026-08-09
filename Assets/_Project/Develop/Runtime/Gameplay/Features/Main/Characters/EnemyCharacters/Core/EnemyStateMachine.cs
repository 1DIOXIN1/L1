using System.Collections.Generic;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Core
{
    public sealed class EnemyStateMachine
    {
        private readonly Dictionary<EnemyStateId, IEnemyState> _states = new();
        private EnemyContext _context;

        public EnemyStateId CurrentStateId { get; private set; }
        public IEnemyState CurrentState { get; private set; }

        public void RegisterState(IEnemyState state)
        {
            _states[state.Id] = state;
        }

        public void Initialize(EnemyContext context, EnemyStateId initialStateId)
        {
            _context = context;
            ChangeState(initialStateId);
        }

        public void Tick(float deltaTime)
        {
            CurrentState?.Tick(_context, deltaTime);
        }

        public void ChangeState(EnemyStateId stateId)
        {
            if (CurrentState != null && CurrentStateId == stateId)
                return;

            CurrentState?.Exit(_context);

            if (_states.TryGetValue(stateId, out IEnemyState nextState) == false)
                return;

            CurrentStateId = stateId;
            CurrentState = nextState;
            CurrentState.Enter(_context);
        }

        public bool HasState(EnemyStateId stateId)
        {
            return _states.ContainsKey(stateId);
        }
    }
}
