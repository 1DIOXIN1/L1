using _Project.Develop.Runtime.Gameplay.Main;
using _Project.Develop.Runtime.Infrastructure.DI;
using _Project.Develop.Runtime.UI.Gameplay.Sequence;

namespace _Project.Develop.Runtime.UI.Gameplay
{
    public class GameplayPresentersFactory
    {
        private DIContainer _container;

        public GameplayPresentersFactory(DIContainer container)
        {
            _container = container;
        }

        public GameplayScreenPresenter CreateGameplayScreenPresenter(GameplayScreenView gameplayScreenView)
        {
            return new GameplayScreenPresenter(gameplayScreenView, this);
        }

        public SequencePresenter CreateSequencePresenter(SequenceView sequenceView)
        {
            SequenceChecker sequenceChecker = _container.Resolve<SequenceChecker>();
            
            return new SequencePresenter(sequenceView, sequenceChecker);
        }
    }
}