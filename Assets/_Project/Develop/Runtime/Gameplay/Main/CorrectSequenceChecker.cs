using System;
using _Project.Develop.Runtime.Utilities.InputManagement;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Main
{
    public class CorrectSequenceChecker
    {
        public event Action OnCorrectSequenceCheck;
        public event Action OnWrongSequenceCheck;

        private string _rightSequence = "";
        private string _currentSequence = "";
        private IInputService _input;
        private bool _isFinished;

        public void StartCheck(string rightSequence, IInputService input)
        {
            if (_input != null)
            {
                _input.CharEntered -= OnCharEntered;
            }

            _rightSequence = rightSequence ?? "";
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

            if (_currentSequence.Length < _rightSequence.Length)
                return;

            _isFinished = true;

            if (_currentSequence == _rightSequence)
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