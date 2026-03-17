using System;
using System.Collections.Generic;

namespace _Project.Develop.Runtime.Utilities.DataManagement.KeyStorage
{
    public class MapDataKeysStorage : IDataKeyStorage
    {
        private readonly Dictionary<Type, string> _keys = new Dictionary<Type, string>()
        {
            { typeof(PlayerData), "PlayerData" },
            { typeof(GameplayData), "GameplayData" },
        };

        public string GetKeyFor<TData>() where TData : ISaveData
            => _keys[typeof(TData)];
    }
}