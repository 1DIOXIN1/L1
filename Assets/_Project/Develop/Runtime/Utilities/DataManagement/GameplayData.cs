using System;
using System.Collections.Generic;

namespace _Project.Develop.Runtime.Utilities.DataManagement
{
    public class GameplayData : ISaveData
    {
        public int CountWins;
        public int CountLoss;
        public List<string> CompletedMissionIds = new();
        public List<MissionOptionalSaveData> CompletedOptionalObjectives = new();
    }

    [Serializable]
    public class MissionOptionalSaveData
    {
        public string MissionId;
        public List<string> ObjectiveIds = new();
    }
}
