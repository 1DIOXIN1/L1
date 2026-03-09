using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Develop.Runtime.Configs;
using _Project.Develop.Runtime.Utilities.AssetsManagement;
using UnityEngine;

namespace _Project.Develop.Runtime.Utilities.ConfigsManagement
{
    public class ResourcesConfigsLoader : IConfigsLoader
    {
        private readonly ResuorcesAssetsLoader _resources;

        private readonly Dictionary<Type, string> _configsResourcesPath = new()
        {
            {typeof(GameplayPreset), "GameplayPresets/GameplayPreset"}
        };

        public ResourcesConfigsLoader(ResuorcesAssetsLoader resources)
        {
            _resources = resources;
        }

        public IEnumerator LoadAsync(Action<Dictionary<Type, object>> onConfigsLoaded)
        {
            Dictionary<Type, object> loadedConfigs = new();

            foreach (KeyValuePair<Type, string> configsResourcesPath in _configsResourcesPath)
            {
                ScriptableObject config = _resources.Load<ScriptableObject>(configsResourcesPath.Value);
                loadedConfigs.Add(configsResourcesPath.Key, config);
                yield return  null;
            }
            onConfigsLoaded?.Invoke(loadedConfigs);
        }
    }
}