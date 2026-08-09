using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters
{
    public static class DamageUtility
    {
        public static bool TryApplyDamage(Collider collider, int damage, GameObject owner)
        {
            if (collider == null || damage <= 0)
                return false;

            if (owner != null && collider.transform.root == owner.transform.root)
                return false;

            if (collider.TryGetComponent(out IDamageble damageble) == false)
                damageble = collider.GetComponentInParent<IDamageble>();

            if (damageble == null)
                return false;

            damageble.TakeDamage(damage);
            return true;
        }
    }
}
