using System;
using System.Collections;
using System.Collections.Generic;

namespace _Project.Develop.Runtime.Utilities.ConfigsManagement
{
    public class ConfigsProviderService 
    {
        private readonly Dictionary<Type, object> _configs = new();
        
        private readonly IConfigsLoader[] _loaders;
        
        public ConfigsProviderService(params IConfigsLoader[] loaders)
        {
            _loaders = loaders;
        }

        public IEnumerator LoadAsync()
        {
            _configs.Clear();
            
            foreach (IConfigsLoader loader in _loaders)
            {
                yield return loader.LoadAsync(loadedConfigs =>
                {
                    foreach (KeyValuePair<Type, object> config in loadedConfigs)
                        _configs[config.Key] = config.Value;
                });
            }
        }

        public T GetConfig<T>() where T : class
        {
            if (_configs.ContainsKey(typeof(T)) == false)
                throw new InvalidOperationException($"Not found config by {typeof(T)}");
        
            return (T)_configs[typeof(T)];
        }
    }
}

