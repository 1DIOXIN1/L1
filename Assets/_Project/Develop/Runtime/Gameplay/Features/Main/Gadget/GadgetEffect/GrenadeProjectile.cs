using _Project.Develop.Runtime.Gameplay.Features.Main.Characters;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Gadget.GadgetEffect
{
    public class GrenadeProjectile : MonoBehaviour
    {
        private IDamageble _attacker;
        private int _damage;
        private float _explosionRadius;
        private float _explosionForce;
        private bool _isExploded;

        public void Initialize(
            IDamageble attacker,
            int damage,
            float explosionRadius,
            float explosionForce,
            float fuseTime)
        {
            _attacker = attacker;
            _damage = damage;
            _explosionRadius = explosionRadius;
            _explosionForce = explosionForce;

            Invoke(nameof(Explode), fuseTime);
        }

        private void Explode()
        {
            if (_isExploded)
                return;

            _isExploded = true;
            Collider[] hits = Physics.OverlapSphere(transform.position, _explosionRadius);

            foreach (Collider hit in hits)
            {
                if (DamageUtility.TryApplyDamage(hit, _damage, _attacker) == false)
                    continue;

                if (hit.attachedRigidbody != null)
                    hit.attachedRigidbody.AddExplosionForce(_explosionForce, transform.position, _explosionRadius);
            }

            Destroy(gameObject);
        }
    }
}
