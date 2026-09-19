namespace _Project.Develop.Runtime.Utilities.DataManagement.DataProviders
{
    public class SettingsDataProvider : DataProvider<SettingsData>
    {
        public SettingsDataProvider(ISaveLoadService saveLoadService) : base(saveLoadService)
        {
        }

        protected override SettingsData GetOriginData()
        {
            return new SettingsData
            {
                MasterVolume = 1f,
                MouseSensitivity = 1f
            };
        }
    }
}
