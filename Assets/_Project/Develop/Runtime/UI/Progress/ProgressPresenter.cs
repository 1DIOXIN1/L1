using System;
using System.Collections.Generic;
using _Project.Develop.Runtime.Meta.Features.Progress;
using _Project.Develop.Runtime.UI.CommonViews;
using _Project.Develop.Runtime.UI.Core;

namespace _Project.Develop.Runtime.UI.Progress
{
    public class ProgressPresenter : IPresenter
    {
        private readonly ProgressItemListView _view;
        private readonly ProgressService _progressService;
        private readonly ViewsFactory _viewsFactory;
        private readonly ProjectPresentersFactory _presentersFactory;
        
        private readonly List<ProgressItemPresenter> _itemPresenters = new();

        public ProgressPresenter(
            ProgressItemListView view,
            ProgressService progressService, 
            ViewsFactory viewFactory, 
            ProjectPresentersFactory presentersFactory)
        {
            _view = view;
            _progressService = progressService;
            _viewsFactory = viewFactory;
            _presentersFactory = presentersFactory;
        }
        
        public void Initialize()
        {
            foreach (ProgressTypes type in Enum.GetValues(typeof(ProgressTypes)))
            {
                NameValueTextView itemView = _viewsFactory.Create<NameValueTextView>(ViewIDs.ProgressItem);
                _view.Add(itemView);

                ProgressItemPresenter itemPresenter = _presentersFactory.CreateProgressItemPresenter(
                    _progressService.GetProgress(type), 
                    type,
                    itemView);
                
                itemPresenter.Initialize();
                _itemPresenters.Add(itemPresenter);
            }
        }

        public void Dispose()
        {
            foreach (ProgressItemPresenter presenter in _itemPresenters)
            {
                _view.Remove(presenter.View);
                _viewsFactory.Release(presenter.View);
                
                presenter.Dispose();
            }
            
            _itemPresenters.Clear();
        }
    }
}