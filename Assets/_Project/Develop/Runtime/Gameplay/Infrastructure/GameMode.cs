using System;
using _Project.Develop.Runtime.Gameplay.Main;

namespace _Project.Develop.Runtime.Gameplay.Infrastructure
{
    public class GameMode
    {
        public event Action Win;
        public event Action Defeat;

        private readonly CorrectSequenceChecker _correctSequenceChecker;

        public GameMode(CorrectSequenceChecker correctSequenceChecker)
        {
            _correctSequenceChecker = correctSequenceChecker;
        }

        public void Start()
        {
            _correctSequenceChecker.OnCorrectSequenceCheck += OnRightSequence;
            _correctSequenceChecker.OnWrongSequenceCheck += OnWrongSequence;
        }

        private void OnRightSequence()
        {
            Win?.Invoke();
        }

        private void OnWrongSequence()
        {
            Defeat?.Invoke();
        }
    }
}