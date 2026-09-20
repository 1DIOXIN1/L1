using System;
using System.Collections.Generic;
using _Project.Develop.Runtime.Meta.Features.Missions;

namespace _Project.Develop.Runtime.Gameplay.Infrastructure.Mission
{
    public readonly struct QuestObjectiveUiEntry
    {
        public QuestObjectiveUiEntry(string id, string description, bool isCompleted)
        {
            Id = id;
            Description = description;
            IsCompleted = isCompleted;
        }

        public string Id { get; }
        public string Description { get; }
        public bool IsCompleted { get; }
    }

    public class MissionQuestTracker
    {
        private readonly List<QuestObjectiveUiEntry> _optionals = new();
        private readonly List<QuestObjectiveUiEntry> _completedPrimaries = new();
        private IReadOnlyList<MissionObjectiveDefinition> _primaryDefinitions = Array.Empty<MissionObjectiveDefinition>();

        public event Action Changed;

        public string MissionId { get; private set; }
        public string MissionDisplayName { get; private set; }
        public bool HasActiveMission => string.IsNullOrEmpty(MissionId) == false;
        public QuestObjectiveUiEntry? ActivePrimary { get; private set; }
        public IReadOnlyList<QuestObjectiveUiEntry> Optionals => _optionals;
        public IReadOnlyList<QuestObjectiveUiEntry> CompletedPrimaries => _completedPrimaries;

        public void BeginMission(MissionDefinition mission)
        {
            Clear();

            if (mission == null)
                return;

            MissionId = mission.Id;
            MissionDisplayName = string.IsNullOrEmpty(mission.DisplayName) ? mission.Id : mission.DisplayName;
            _primaryDefinitions = mission.PrimarySteps ?? Array.Empty<MissionObjectiveDefinition>();

            for (int i = 0; i < mission.OptionalObjectives.Count; i++)
            {
                MissionObjectiveDefinition definition = mission.OptionalObjectives[i];
                if (definition == null)
                    continue;

                _optionals.Add(new QuestObjectiveUiEntry(definition.Id, definition.Description, false));
            }

            NotifyChanged();
        }

        public void SetActivePrimary(int stepIndex)
        {
            if (stepIndex < 0 || stepIndex >= _primaryDefinitions.Count)
            {
                ActivePrimary = null;
                NotifyChanged();
                return;
            }

            MissionObjectiveDefinition definition = _primaryDefinitions[stepIndex];
            ActivePrimary = new QuestObjectiveUiEntry(definition.Id, definition.Description, false);
            NotifyChanged();
        }

        public void CompletePrimaryStep(int stepIndex)
        {
            if (stepIndex < 0 || stepIndex >= _primaryDefinitions.Count)
                return;

            MissionObjectiveDefinition definition = _primaryDefinitions[stepIndex];
            _completedPrimaries.Insert(0, new QuestObjectiveUiEntry(definition.Id, definition.Description, true));

            if (ActivePrimary.HasValue && ActivePrimary.Value.Id == definition.Id)
                ActivePrimary = null;

            NotifyChanged();
        }

        public void CompleteOptional(string objectiveId)
        {
            for (int i = 0; i < _optionals.Count; i++)
            {
                QuestObjectiveUiEntry entry = _optionals[i];
                if (entry.Id != objectiveId)
                    continue;

                _optionals[i] = new QuestObjectiveUiEntry(entry.Id, entry.Description, true);
                NotifyChanged();
                return;
            }
        }

        public void ClearActivePrimary()
        {
            ActivePrimary = null;
            NotifyChanged();
        }

        public void Clear()
        {
            MissionId = null;
            MissionDisplayName = null;
            ActivePrimary = null;
            _optionals.Clear();
            _completedPrimaries.Clear();
            _primaryDefinitions = Array.Empty<MissionObjectiveDefinition>();
            NotifyChanged();
        }

        private void NotifyChanged() => Changed?.Invoke();
    }
}
