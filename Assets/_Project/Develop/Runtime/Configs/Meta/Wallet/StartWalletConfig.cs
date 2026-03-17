using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Develop.Runtime.Meta.Features.Wallet;
using UnityEngine;

namespace _Project.Develop.Runtime.Configs.Meta.Wallet
{
    [CreateAssetMenu(menuName = "Configs/Meta/Wallet/StartWalletConfig", fileName = "StartWalletConfig")]
    public class StartWalletConfig :ScriptableObject
    {
        [SerializeField] private List<CurrencyConfig> _values;
        [field: SerializeField] public int ValueToAdd { get; private set; } = 10;
        [field: SerializeField] public int ValueToSpend { get; private set; } = 10;

        public int GetValueFor(CurrencyTypes currencyType)
            => _values.First(config => config.Type == currencyType).Value;
                
        [Serializable]
        private class CurrencyConfig
        {
            [field: SerializeField] public CurrencyTypes Type { get; private set; }
            [field: SerializeField] public int Value { get; private set; }
        }
    }
}
