using System;
using _Project.Develop.Runtime.Utilities.InputManagement;

namespace _Project.Develop.Runtime.Gameplay.Main
{
    public class SequenceChecker
    {
        public event Action<string> OnCurrentSequenceChanged;
        public event Action<string> OnRequiredSequenceChanged;
        public event Action OnCorrectSequenceCheck;
        public event Action OnWrongSequenceCheck;

        private string _requiredSequence = "";
        private string _currentSequence = "";
        private IInputService _input;
        private bool _isFinished;

        public void StartCheck(string rightSequence, IInputService input)
        {
            if (_input != null)
            {
                _input.CharEntered -= OnCharEntered;
            }

            _requiredSequence = rightSequence ?? "";
            
            OnRequiredSequenceChanged?.Invoke(_requiredSequence);
            
            _currentSequence = "";
            _input = input;
            _isFinished = false;

            _input.CharEntered += OnCharEntered;
        }

        private void OnCharEntered(char inputChar)
        {
            if (_isFinished)
            {
                return;
            }

            _currentSequence += inputChar;
            OnCurrentSequenceChanged?.Invoke(_currentSequence);
            
            if (_currentSequence.Length < _requiredSequence.Length)
                return;

            _isFinished = true;

            if (_currentSequence == _requiredSequence)
            {
                OnCorrectSequenceCheck?.Invoke();
            }
            else
            {
                OnWrongSequenceCheck?.Invoke();
            }

            Disable();
        }

        private void Disable()
        {
            if (_input != null)
            {
                _input.CharEntered -= OnCharEntered;
                _input = null;
            }
        }
    }
}