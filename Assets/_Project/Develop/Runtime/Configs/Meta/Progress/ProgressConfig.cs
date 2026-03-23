using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Develop.Runtime.Meta.Features.Progress;
using UnityEngine;

namespace _Project.Develop.Runtime.Configs.Meta.Progress
{
    [CreateAssetMenu(menuName = "Configs/Meta/Progress/ProgressConfig", fileName = "ProgressConfig")]
    public class ProgressConfig : ScriptableObject
    {
        [field: SerializeField] public int ValueToResetProgress { get; private set; } = 50;
        
        [SerializeField] private List<ProgressUIData> _progressCategories;
        
        public string GetNameFor(ProgressTypes type)
            => _progressCategories.First(config => config.Type == type).Name;

        [Serializable]
        private class ProgressUIData
        {
            [field: SerializeField] public ProgressTypes Type { get; private set; }
            [field: SerializeField] public string Name { get; private set; }
        }
    }
}