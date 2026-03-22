using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Develop.Runtime.Configs;
using _Project.Develop.Runtime.Configs.Core.Gameplay;
using _Project.Develop.Runtime.Configs.Meta.Progress;
using _Project.Develop.Runtime.Configs.Meta.Wallet;
using _Project.Develop.Runtime.Utilities.AssetsManagement;
using UnityEngine;

namespace _Project.Develop.Runtime.Utilities.ConfigsManagement
{
    public class ResourcesConfigsLoader : IConfigsLoader
    {
        private readonly ResourcesAssetsLoader _resources;

        private readonly Dictionary<Type, string> _configsResourcesPath = new()
        {
            {typeof(StartWalletConfig), "Configs/StartWalletConfig"},
            {typeof(StartGameplayConfig), "Configs/StartGameplayConfig"},
            {typeof(ProgressConfig), "Configs/ProgressConfig"},
            {typeof(CurrencyIconsConfig), "Configs/CurrencyIconsConfig"}
        };

        public ResourcesConfigsLoader(ResourcesAssetsLoader resources)
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