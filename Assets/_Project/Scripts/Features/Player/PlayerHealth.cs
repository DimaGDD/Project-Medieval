using System;
using UnityEngine;
using Game.Features.LevelingSystem;

namespace Game.Features.Player
{
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        public float CurrentHealth { get; private set; }
        public float MaxHealth => _stats != null ? _stats.GetStat(StatType.MaxHealth) : 0f;
        public bool IsDead { get; private set; }

        public event Action<float, float> OnHealthChanged;
        public event Action OnDeath;

        private CharacterStats _stats;

        private void Awake()
        {
            _stats = GetComponent<CharacterStats>();
            CurrentHealth = MaxHealth;
        }

        private void Start()
        {
            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
            if (_stats != null) _stats.OnStatChanged += HandleStatChanged;
        }

        private void OnDestroy()
        {
            if (_stats != null) _stats.OnStatChanged -= HandleStatChanged;
        }

        private void HandleStatChanged(StatType type)
        {
            if (type != StatType.MaxHealth) return;

            CurrentHealth = Mathf.Min(CurrentHealth, MaxHealth);
            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        }

        public void TakeDamage(DamageInfo damageInfo)
        {
            if (IsDead) return;

            StatType reductionStat = damageInfo.type.ToReductionStat();
            float reduction01 = _stats.GetDamageReduction(reductionStat);

            float finalDamage = damageInfo.amount * (1f - reduction01);

            CurrentHealth -= finalDamage;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);

            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

            if (CurrentHealth <= 0f) Die();
        }

        public void Heal(float amount)
        {
            if (IsDead || amount <= 0f) return;

            CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0f, MaxHealth);
            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        }

        private void Die()
        {
            if (IsDead) return;

            IsDead = true;
            OnDeath?.Invoke();
        }
    }
}