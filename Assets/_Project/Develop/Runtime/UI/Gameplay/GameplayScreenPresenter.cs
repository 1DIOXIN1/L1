using System.Collections.Generic;
using _Project.Develop.Runtime.UI.Core;

namespace _Project.Develop.Runtime.UI.Gameplay
{
    public class GameplayScreenPresenter : IPresenter
    {
        private readonly GameplayScreenView _view;
        private readonly GameplayPresentersFactory _presentersFactory;
        
        private readonly List<IPresenter> _childPresenters = new();

        public GameplayScreenPresenter(GameplayScreenView view, GameplayPresentersFactory presentersFactory)
        {
            _view = view;
            _presentersFactory = presentersFactory;
        }

        public void Initialize()
        {
            
            foreach (var childPresenter in _childPresenters)
                childPresenter.Initialize();
        }

        public void Dispose()
        {
            foreach (var childPresenter in _childPresenters)
                childPresenter.Dispose();
            
            _childPresenters.Clear();
        }
    }
}