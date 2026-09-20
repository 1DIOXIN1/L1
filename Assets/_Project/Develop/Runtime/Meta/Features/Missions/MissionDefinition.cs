using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Develop.Runtime.Meta.Features.Missions
{
    [Serializable]
    public class MissionDefinition
    {
        [SerializeField] private string _id;
        [SerializeField] private string _displayName;
        [SerializeField] private string _sceneName;
        [SerializeField] private string _prerequisiteMissionId;
        [SerializeField] private List<MissionObjectiveDefinition> _primarySteps = new();
        [SerializeField] private List<MissionObjectiveDefinition> _optionalObjectives = new();

        public string Id => _id;
        public string DisplayName => _displayName;
        public string SceneName => _sceneName;
        public string PrerequisiteMissionId => string.IsNullOrWhiteSpace(_prerequisiteMissionId) ? null : _prerequisiteMissionId;
        public IReadOnlyList<MissionObjectiveDefinition> PrimarySteps => _primarySteps;
        public IReadOnlyList<MissionObjectiveDefinition> OptionalObjectives => _optionalObjectives;
    }
}
