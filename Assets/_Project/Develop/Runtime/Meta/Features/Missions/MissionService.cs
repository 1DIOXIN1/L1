using System;
using System.Collections.Generic;
using _Project.Develop.Runtime.Configs.Meta.Missions;
using _Project.Develop.Runtime.Utilities.ConfigsManagement;
using _Project.Develop.Runtime.Utilities.CoroutinesManagement;
using _Project.Develop.Runtime.Utilities.DataManagement;
using _Project.Develop.Runtime.Utilities.DataManagement.DataProviders;

namespace _Project.Develop.Runtime.Meta.Features.Missions
{
    public class MissionService : IDataReader<GameplayData>, IDataWriter<GameplayData>
    {
        private readonly ConfigsProviderService _configsProviderService;
        private readonly GameplayDataProvider _gameplayDataProvider;
        private readonly CoroutinesPerformer _coroutinesPerformer;
        private readonly HashSet<string> _completedMissionIds = new();
        private readonly Dictionary<string, HashSet<string>> _completedOptionalByMission = new();

        public MissionService(
            ConfigsProviderService configsProviderService,
            GameplayDataProvider gameplayDataProvider,
            CoroutinesPerformer coroutinesPerformer)
        {
            _configsProviderService = configsProviderService;
            _gameplayDataProvider = gameplayDataProvider;
            _coroutinesPerformer = coroutinesPerformer;

            _gameplayDataProvider.RegisterReader(this);
            _gameplayDataProvider.RegisterWriter(this);
        }

        private MissionsCatalogConfig Catalog => _configsProviderService.GetConfig<MissionsCatalogConfig>();

        public IReadOnlyList<MissionDefinition> GetAllMissions() => Catalog.Missions;

        public MissionDefinition GetMission(string missionId) => Catalog.GetMission(missionId);

        public bool TryGetMission(string missionId, out MissionDefinition mission)
            => Catalog.TryGetMission(missionId, out mission);

        public bool IsCompleted(string missionId)
            => string.IsNullOrEmpty(missionId) == false && _completedMissionIds.Contains(missionId);

        public bool IsUnlocked(string missionId)
        {
            if (TryGetMission(missionId, out MissionDefinition mission) == false)
                return false;

            if (string.IsNullOrEmpty(mission.PrerequisiteMissionId))
                return true;

            return IsCompleted(mission.PrerequisiteMissionId);
        }

        public bool IsOptionalCompleted(string missionId, string objectiveId)
        {
            if (_completedOptionalByMission.TryGetValue(missionId, out HashSet<string> objectives) == false)
                return false;

            return objectives.Contains(objectiveId);
        }

        public void Complete(string missionId)
        {
            if (string.IsNullOrEmpty(missionId))
                return;

            if (_completedMissionIds.Add(missionId) == false)
                return;

            Save();
        }

        public void CompleteOptional(string missionId, string objectiveId)
        {
            if (string.IsNullOrEmpty(missionId) || string.IsNullOrEmpty(objectiveId))
                return;

            if (_completedOptionalByMission.TryGetValue(missionId, out HashSet<string> objectives) == false)
            {
                objectives = new HashSet<string>();
                _completedOptionalByMission[missionId] = objectives;
            }

            if (objectives.Add(objectiveId) == false)
                return;

            Save();
        }

        public void WriteTo(GameplayData data)
        {
            data.CompletedMissionIds = new List<string>(_completedMissionIds);
            data.CompletedOptionalObjectives = new List<MissionOptionalSaveData>();

            foreach (KeyValuePair<string, HashSet<string>> pair in _completedOptionalByMission)
            {
                data.CompletedOptionalObjectives.Add(new MissionOptionalSaveData
                {
                    MissionId = pair.Key,
                    ObjectiveIds = new List<string>(pair.Value)
                });
            }
        }

        public void ReadFrom(GameplayData data)
        {
            _completedMissionIds.Clear();
            _completedOptionalByMission.Clear();

            if (data.CompletedMissionIds != null)
            {
                for (int i = 0; i < data.CompletedMissionIds.Count; i++)
                {
                    string id = data.CompletedMissionIds[i];
                    if (string.IsNullOrEmpty(id) == false)
                        _completedMissionIds.Add(id);
                }
            }

            if (data.CompletedOptionalObjectives == null)
                return;

            for (int i = 0; i < data.CompletedOptionalObjectives.Count; i++)
            {
                MissionOptionalSaveData entry = data.CompletedOptionalObjectives[i];
                if (entry == null || string.IsNullOrEmpty(entry.MissionId))
                    continue;

                HashSet<string> objectives = new();
                if (entry.ObjectiveIds != null)
                {
                    for (int j = 0; j < entry.ObjectiveIds.Count; j++)
                    {
                        string objectiveId = entry.ObjectiveIds[j];
                        if (string.IsNullOrEmpty(objectiveId) == false)
                            objectives.Add(objectiveId);
                    }
                }

                _completedOptionalByMission[entry.MissionId] = objectives;
            }
        }

        private void Save()
        {
            _coroutinesPerformer.StartPerform(_gameplayDataProvider.Save());
        }
    }
}
