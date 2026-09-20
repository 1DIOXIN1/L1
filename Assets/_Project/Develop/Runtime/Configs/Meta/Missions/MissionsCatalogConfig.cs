using System;
using System.Collections.Generic;
using _Project.Develop.Runtime.Meta.Features.Missions;
using UnityEngine;

namespace _Project.Develop.Runtime.Configs.Meta.Missions
{
    [CreateAssetMenu(menuName = "Configs/Meta/Missions/MissionsCatalogConfig", fileName = "MissionsCatalogConfig")]
    public class MissionsCatalogConfig : ScriptableObject
    {
        [SerializeField] private List<MissionDefinition> _missions = new();

        public IReadOnlyList<MissionDefinition> Missions => _missions;

        public MissionDefinition GetMission(string missionId)
        {
            if (TryGetMission(missionId, out MissionDefinition mission))
                return mission;

            throw new InvalidOperationException($"Mission '{missionId}' is not configured.");
        }

        public bool TryGetMission(string missionId, out MissionDefinition mission)
        {
            for (int i = 0; i < _missions.Count; i++)
            {
                mission = _missions[i];
                if (mission != null && mission.Id == missionId)
                    return true;
            }

            mission = null;
            return false;
        }
    }
}
