using _Project.Develop.Runtime.Configs.Meta.Characters;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters
{
    public abstract class Character : MonoBehaviour, IDamageble
    {
        private int _health;
        private bool _isDead;
        
        protected int Health => _health;
        protected bool IsDead => _isDead;

        protected void InitializeHealth(ICharacterConfig config)
        {
            _health = config.Health;
            _isDead = false;
        }

        public void TakeDamage(int damage)
        {
            if (_isDead || damage <= 0)
                return;

            _health = Mathf.Max(0, _health - damage);
            Debug.Log($"Character {name} damaged with {damage} remain hp: {_health}");
            if (_health == 0)
                Die();
        }

        protected virtual void Die()
        {
            _isDead = true;
            Destroy(gameObject);
        }
    }
}
