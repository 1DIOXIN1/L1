using _Project.Develop.Runtime.Gameplay.Features.Main.Characters;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Gadget.GadgetEffect
{
    public class GrenadeProjectile : MonoBehaviour
    {
        private GameObject _owner;
        private int _damage;
        private float _explosionRadius;
        private float _explosionForce;
        private bool _isExploded;

        public void Initialize(
            GameObject owner,
            int damage,
            float explosionRadius,
            float explosionForce,
            float fuseTime)
        {
            _owner = owner;
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
                if (hit.transform.root.gameObject == _owner)
                    continue;

                if (!hit.TryGetComponent<IDamageble>(out var damageble))
                    damageble = hit.GetComponentInParent<IDamageble>();

                damageble?.TakeDamage(_damage);

                if (hit.attachedRigidbody != null)
                    hit.attachedRigidbody.AddExplosionForce(_explosionForce, transform.position, _explosionRadius);
            }

            Destroy(gameObject);
        }
    }
}
