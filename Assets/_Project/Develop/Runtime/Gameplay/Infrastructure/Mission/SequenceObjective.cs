using System;
using System.Collections.Generic;

namespace _Project.Develop.Runtime.Gameplay.Infrastructure.Mission
{
    public class SequenceObjective : IMissionObjective
    {
        private readonly IReadOnlyList<IMissionObjective> _steps;
        private int _index = -1;
        private bool _isStopped;

        public SequenceObjective(IReadOnlyList<IMissionObjective> steps)
        {
            if (steps == null || steps.Count == 0)
                throw new ArgumentException("Sequence requires at least one step.", nameof(steps));

            _steps = steps;
        }

        public bool IsComplete { get; private set; }
        public int CurrentIndex => _index;
        public int StepCount => _steps.Count;

        public event Action Completed;
        public event Action<int> StepCompleted;
        public event Action<int> StepStarted;

        public void Start()
        {
            _isStopped = false;
            IsComplete = false;
            _index = -1;
            Advance();
        }

        public void Stop()
        {
            _isStopped = true;
            DetachCurrent();
        }

        private void Advance()
        {
            if (_isStopped || IsComplete)
                return;

            int completedIndex = _index;
            DetachCurrent();
            _index++;

            if (completedIndex >= 0)
                StepCompleted?.Invoke(completedIndex);

            if (_index >= _steps.Count)
            {
                IsComplete = true;
                _index = -1;
                Completed?.Invoke();
                return;
            }

            IMissionObjective step = _steps[_index];
            step.Completed += OnStepCompleted;
            step.Start();
            StepStarted?.Invoke(_index);
        }

        private void OnStepCompleted()
        {
            if (_isStopped || IsComplete)
                return;

            Advance();
        }

        private void DetachCurrent()
        {
            if (_index < 0 || _index >= _steps.Count)
                return;

            IMissionObjective step = _steps[_index];
            step.Completed -= OnStepCompleted;
            step.Stop();
        }
    }
}
