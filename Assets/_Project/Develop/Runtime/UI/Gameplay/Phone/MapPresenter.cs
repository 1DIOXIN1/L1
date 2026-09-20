using _Project.Develop.Runtime.Meta.Features.Missions;
using _Project.Develop.Runtime.UI.Core;

namespace _Project.Develop.Runtime.UI.Gameplay.Phone
{
    public class MapPresenter : IPresenter
    {
        private readonly MapPanelView _view;
        private readonly MissionService _missionService;
        private readonly LocationTravelService _locationTravel;

        public MapPresenter(
            MapPanelView view,
            MissionService missionService,
            LocationTravelService locationTravel)
        {
            _view = view;
            _missionService = missionService;
            _locationTravel = locationTravel;
        }

        public void Initialize()
        {
            _view.MissionTravelClicked += OnMissionTravelClicked;
            _view.HubTravelClicked += OnHubTravelClicked;
            Refresh();
        }

        public void Dispose()
        {
            _view.MissionTravelClicked -= OnMissionTravelClicked;
            _view.HubTravelClicked -= OnHubTravelClicked;
        }

        public void Show()
        {
            Refresh();
        }

        private void Refresh()
        {
            var buttons = _view.MissionButtons;
            for (int i = 0; i < buttons.Count; i++)
            {
                MissionTravelButtonBinding binding = buttons[i];
                string missionId = binding.MissionId;
                bool unlocked = _missionService.IsUnlocked(missionId);
                string displayName = missionId;

                if (_missionService.TryGetMission(missionId, out MissionDefinition mission))
                    displayName = string.IsNullOrEmpty(mission.DisplayName) ? mission.Id : mission.DisplayName;

                _view.SetMissionEntry(missionId, displayName, unlocked);
            }
        }

        private void OnMissionTravelClicked(string missionId)
        {
            _locationTravel.TryTravelToMission(missionId);
        }

        private void OnHubTravelClicked()
        {
            _locationTravel.GoToHub();
        }
    }
}
