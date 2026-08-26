using System;
using _Project.Develop.Runtime.Configs.Meta.Characters;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters
{
    public abstract class Character : MonoBehaviour, IDamageble
    {
        private int _health;
        private int _maxHealth;
        private bool _isDead;
        private Action _onDied;

        public event Action<int, int> HealthChanged;

        public int CurrentHealth => _health;
        public int MaxHealth => _maxHealth;
        protected int Health => _health;
        protected bool IsDead => _isDead;

        public void SetDeathHandler(Action onDied)
        {
            _onDied = onDied;
        }

        protected void InitializeHealth(ICharacterConfig config)
        {
            InitializeHealth(config.Health, config.Health);
        }

        protected void InitializeHealth(int currentHealth, int maxHealth)
        {
            _maxHealth = Mathf.Max(1, maxHealth);
            _health = Mathf.Clamp(currentHealth, 0, _maxHealth);
            _isDead = false;
            HealthChanged?.Invoke(_health, _maxHealth);
        }

        public void TakeDamage(int damage)
        {
            if (_isDead || damage <= 0)
                return;

            _health = Mathf.Max(0, _health - damage);
            HealthChanged?.Invoke(_health, _maxHealth);
            OnDamaged(damage);

            if (_health == 0)
                Die();
        }

        protected virtual void OnDamaged(int damage)
        {
        }

        protected virtual void Die()
        {
            _isDead = true;
            _onDied?.Invoke();
            Destroy(gameObject);
        }
    }
}
