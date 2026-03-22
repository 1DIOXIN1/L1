using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Develop.Runtime.Utilities.DataManagement;
using _Project.Develop.Runtime.Utilities.DataManagement.DataProviders;
using _Project.Develop.Runtime.Utilities.Reactive;

namespace _Project.Develop.Runtime.Meta.Features.Wallet
{
    public class WalletService : IDataWriter<PlayerData>, IDataReader<PlayerData>
    {
        private readonly Dictionary<CurrencyTypes, ReactiveVariable<int>> _currencies;

        public WalletService(Dictionary<CurrencyTypes, ReactiveVariable<int>> currencies, PlayerDataProvider playerDataProvider)
        {
            _currencies = new Dictionary<CurrencyTypes, ReactiveVariable<int>>(currencies);
            
            playerDataProvider.RegisterReader(this);
            playerDataProvider.RegisterWriter(this);
        }

        public List<CurrencyTypes> AvaliableCurrencies => _currencies.Keys.ToList();
        
        public IReadOnlyVariable<int> GetCurrency(CurrencyTypes type) => _currencies[type];

        public bool Enough(CurrencyTypes type, int amount)
        {
            if(amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));
            
            return _currencies[type].Value >= amount;
        }

        public void Add(CurrencyTypes type, int amount)
        {
            if(amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));
            
            _currencies[type].Value += amount;
        }
        
        public void Spend(CurrencyTypes type, int amount)
        {
            if (Enough(type, amount) == false)
                throw new InvalidOperationException("Not enough currency" + type.ToString());
            
            if(amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));
            
            _currencies[type].Value -= amount;
        }

        public void WriteTo(PlayerData data)
        {
            foreach (KeyValuePair<CurrencyTypes, ReactiveVariable<int>> currency in _currencies)
            {
                if(data.WalletData.ContainsKey(currency.Key))
                    data.WalletData[currency.Key] = currency.Value.Value;
                else
                    data.WalletData.Add(currency.Key, currency.Value.Value);
            }
        }

        public void ReadFrom(PlayerData data)
        {
            foreach (KeyValuePair<CurrencyTypes, int> currency in data.WalletData)
            {
                if(_currencies.ContainsKey(currency.Key))
                    _currencies[currency.Key].Value = currency.Value;
                else
                    _currencies.Add(currency.Key, new ReactiveVariable<int>(currency.Value));
            }
        }
    }
}