using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters
{
    public static class DamageUtility
    {
        private static DamageableRegistry _registry;

        public static void Configure(DamageableRegistry registry)
        {
            _registry = registry;
        }

        public static bool TryApplyDamage(Collider collider, int damage, IDamageble attacker)
        {
            if (collider == null || damage <= 0)
                return false;

            if (_registry.TryGet(collider, out IDamageble target) == false)
                return false;

            if (ReferenceEquals(attacker, target))
                return false;

            if (attacker != null
                && attacker.Affiliation != DamageAffiliation.Neutral
                && attacker.Affiliation == target.Affiliation)
                return false;

            target.TakeDamage(damage);
            return true;
        }
    }
}
