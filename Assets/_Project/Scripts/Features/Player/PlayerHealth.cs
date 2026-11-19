using System;
using UnityEngine;

namespace Game.Features.Player
{
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        [Header("Settings")]
        [SerializeField] private float _maxHealth = 100f;

        public float CurrentHealth { get; private set; }
        public float MaxHealth { get; private set; }
        public bool IsDead { get; private set; }

        public event Action<float, float> OnHealthChanged;
        public event Action OnDeath;


        private void Awake()
        {
            CurrentHealth = _maxHealth;
        }

        private void Start()
        {
            OnHealthChanged?.Invoke(CurrentHealth, _maxHealth);
        }

        public void TakeDamage(DamageInfo damageInfo)
        {
            if (IsDead) return;

            float damage = damageInfo.amount;

            CurrentHealth -= damage;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, _maxHealth);

            OnHealthChanged?.Invoke(CurrentHealth, _maxHealth);

            if (CurrentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            if (IsDead) return;

            IsDead = true;

            OnDeath?.Invoke();
        }
    }
}
