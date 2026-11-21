using System;
using System.Collections.Generic;
using _Project.Develop.Runtime.Configs.Meta.Weapon;
using _Project.Develop.Runtime.Meta.Features.Player;
using _Project.Develop.Runtime.Utilities.CoroutinesManagement;
using _Project.Develop.Runtime.Utilities.DataManagement.DataProviders;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Gadget
{
    public sealed class MissionSupplyInventory
    {
        private readonly PlayerStateService _state;
        private readonly PlayerDataProvider _data;
        private readonly CoroutinesPerformer _coroutines;
        private readonly Dictionary<SupplyType, int> _remaining = new();
        private bool _changed;

        public MissionSupplyInventory(int capacity, PlayerStateService state, PlayerDataProvider data,
            CoroutinesPerformer coroutines)
        {
            _state = state;
            _data = data;
            _coroutines = coroutines;

            foreach (SupplyType type in Enum.GetValues(typeof(SupplyType)))
                _remaining[type] = Mathf.Min(capacity, state.SupplyCount(type));
        }

        public int Count(SupplyType type) => Mathf.Min(_remaining[type], _state.SupplyCount(type));

        public void Consume(SupplyType type)
        {
            _state.ConsumeSupply(type);
            _remaining[type]--;
            _changed = true;
        }

        public bool SaveIfChanged()
        {
            if (!_changed)
                return false;

            _changed = false;
            _coroutines.StartPerform(_data.Save());
            return true;
        }
    }
}
