using _Project.Develop.Runtime.Gameplay.Infrastructure.Mission;
using _Project.Develop.Runtime.UI.Core;

namespace _Project.Develop.Runtime.UI.Gameplay.Phone
{
    public class QuestsPresenter : IPresenter
    {
        private readonly QuestsPanelView _view;
        private readonly MissionQuestTracker _questTracker;

        public QuestsPresenter(QuestsPanelView view, MissionQuestTracker questTracker)
        {
            _view = view;
            _questTracker = questTracker;
        }

        public void Initialize()
        {
            _questTracker.Changed += OnChanged;
            Refresh();
        }

        public void Dispose()
        {
            _questTracker.Changed -= OnChanged;
        }

        public void Show()
        {
            Refresh();
        }

        private void OnChanged()
        {
            Refresh();
        }

        private void Refresh()
        {
            _view.Render(_questTracker);
        }
    }
}
