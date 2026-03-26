using System.Collections.Generic;
using _Project.Develop.Runtime.Utilities;
using _Project.Develop.Runtime.Utilities.DataManagement;
using _Project.Develop.Runtime.Utilities.DataManagement.DataProviders;
using _Project.Develop.Runtime.Utilities.Reactive;

namespace _Project.Develop.Runtime.Meta.Features.Progress
{
    public class ProgressService : IDataWriter<GameplayData>, IDataReader<GameplayData>
    {
        private readonly GameplayDataProvider _gameplayDataProvider;
        private readonly CoroutinesPerformer _coroutinesPerformer;
        private readonly Dictionary<ProgressTypes, ReactiveVariable<int>> _progressItems;

        public ProgressService(
            Dictionary<ProgressTypes, ReactiveVariable<int>> progressItems,
            GameplayDataProvider gameplayDataProvider,
            CoroutinesPerformer coroutinesPerformer)
        {
            _gameplayDataProvider = gameplayDataProvider;
            _coroutinesPerformer = coroutinesPerformer;

            _progressItems = new Dictionary<ProgressTypes, ReactiveVariable<int>>(progressItems);
            
            _gameplayDataProvider.RegisterReader(this);
            _gameplayDataProvider.RegisterWriter(this);
        }
        
        public IReadOnlyVariable<int> GetProgress(ProgressTypes progressType) => _progressItems[progressType];
        
        public void Win() => _progressItems[ProgressTypes.CountWins].Value++;
        public void Lose() => _progressItems[ProgressTypes.CountLosses].Value++;
        
        public void ResetProgress()
        {
            _gameplayDataProvider.Reset();
            _coroutinesPerformer.StartPerform(_gameplayDataProvider.Save());
        }

        public void WriteTo(GameplayData data)
        {
            data.CountWins = _progressItems[ProgressTypes.CountWins].Value;
            data.CountLoss = _progressItems[ProgressTypes.CountLosses].Value;
        }

        public void ReadFrom(GameplayData data)
        {
            _progressItems[ProgressTypes.CountWins].Value = data.CountWins;
            _progressItems[ProgressTypes.CountLosses].Value = data.CountLoss;
        }
    }
}