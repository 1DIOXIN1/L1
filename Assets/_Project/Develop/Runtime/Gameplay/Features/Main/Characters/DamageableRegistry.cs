using System.Collections.Generic;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters
{
    public sealed class DamageableRegistry
    {
        private readonly Dictionary<int, IDamageble> _byColliderId = new Dictionary<int, IDamageble>();

        public void Register(IDamageble damageble, Collider collider)
        {
            _byColliderId[collider.GetInstanceID()] = damageble;
        }

        public void Register(IDamageble damageble, Collider[] colliders)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                Collider collider = colliders[i];
                if (collider != null)
                    Register(damageble, collider);
            }
        }

        public void Unregister(Collider collider)
        {
            _byColliderId.Remove(collider.GetInstanceID());
        }

        public void Unregister(IDamageble damageble)
        {
            List<int> removeKeys = null;
            foreach (KeyValuePair<int, IDamageble> pair in _byColliderId)
            {
                if (ReferenceEquals(pair.Value, damageble) == false)
                    continue;

                removeKeys ??= new List<int>();
                removeKeys.Add(pair.Key);
            }

            if (removeKeys == null)
                return;

            for (int i = 0; i < removeKeys.Count; i++)
                _byColliderId.Remove(removeKeys[i]);
        }

        public bool TryGet(Collider collider, out IDamageble damageble)
        {
            return _byColliderId.TryGetValue(collider.GetInstanceID(), out damageble);
        }
    }
}
