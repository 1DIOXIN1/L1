using System;
using UnityEngine;

namespace _Project.Develop.Runtime.Meta.Features.Missions
{
    [Serializable]
    public class MissionObjectiveDefinition
    {
        [SerializeField] private string _id;
        [SerializeField] private string _title;
        [SerializeField] private string _description;
        [SerializeField] private MissionObjectiveRole _role = MissionObjectiveRole.Primary;
        [SerializeField] private MissionObjectiveType _type = MissionObjectiveType.ClearAllEnemies;
        [SerializeField] private string _targetId;

        public string Id => _id;
        public string Title => _title;
        public MissionObjectiveRole Role => _role;
        public MissionObjectiveType Type => _type;

        public string TargetId => string.IsNullOrWhiteSpace(_targetId) ? _id : _targetId;

        public string Description =>
            string.IsNullOrWhiteSpace(_description) == false
                ? _description
                : (string.IsNullOrWhiteSpace(_title) == false ? _title : _id);
    }
}
