using _Project.Develop.Runtime.Gameplay.Main;
using _Project.Develop.Runtime.UI.Core;

namespace _Project.Develop.Runtime.UI.Gameplay.Sequence
{
    public class SequencePresenter : IPresenter
    {
        private SequenceView _view;
        private SequenceChecker _sequenceChecker;

        public SequencePresenter(SequenceView view, SequenceChecker sequenceChecker)
        {
            _view = view;
            _sequenceChecker = sequenceChecker;
        }

        public void Initialize()
        {
            _sequenceChecker.OnRequiredSequenceChanged += OnRequiredSequenceChanged;
            _sequenceChecker.OnCurrentSequenceChanged += OnCurrentSequenceChanged;
        }

        public void Dispose()
        {
            _sequenceChecker.OnRequiredSequenceChanged -= OnRequiredSequenceChanged;
            _sequenceChecker.OnCurrentSequenceChanged -= OnCurrentSequenceChanged;
        }
        
        private void OnRequiredSequenceChanged(string requiredSequence) => _view.SetRequiredText(requiredSequence);
        
        private void OnCurrentSequenceChanged(string currentSequence) => _view.SetCurrentText(currentSequence);
    }
}