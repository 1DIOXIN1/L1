using System.Collections.Generic;
using _Project.Develop.Runtime.Utilities.ConfigsManagement;
using _Project.Develop.Runtime.Utilities.DataManagement;

namespace _Project.Develop.Runtime.Utilities.DataManagement.DataProviders
{
    public class GameplayDataProvider : DataProvider<GameplayData>
    {
        private readonly ConfigsProviderService _configsProviderService;
        
        public GameplayDataProvider(ISaveLoadService saveLoadService, ConfigsProviderService configsProviderService) : base(saveLoadService)
        {
            _configsProviderService =  configsProviderService;
        }

        protected override GameplayData GetOriginData()
        {
            return new GameplayData()
            {
                CountWins = 0,
                CountLoss = 0,
                CompletedMissionIds = new List<string>(),
                CompletedOptionalObjectives = new List<MissionOptionalSaveData>(),
            };
        }
    }
}
