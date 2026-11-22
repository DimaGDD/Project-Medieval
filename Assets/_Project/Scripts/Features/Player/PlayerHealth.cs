using System;
using UnityEngine;
using Game.Features.Core.Combat;
using Game.Features.Core.Tags;
using Game.Features.LevelingSystem;
using Game.Features.LevelingSystem.Combat;

namespace Game.Features.Player
{
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        private float _currentHealth;
        private bool _healthInitialized;
        private CharacterStats _stats;

        public float CurrentHealth
        {
            get { EnsureInitialized(); return _currentHealth; }
            private set { _currentHealth = value; }
        }

        /// <summary>requires CharacterStats; without it, MaxHealth is 0.</summary>
        public float MaxHealth
        {
            get
            {
                if (_stats == null) _stats = GetComponent<CharacterStats>();
                return _stats != null ? _stats.GetStat(StatType.MaxHealth) : 0f;
            }
        }

        public bool IsDead { get; private set; }

        public event Action<float, float> OnHealthChanged;
        public event Action OnDeath;
        public event Action<DamageContext, DamageResult> OnHit;

        private void Awake() => EnsureInitialized();

        private void EnsureInitialized()
        {
            if (_healthInitialized) return;
            if (_stats == null) _stats = GetComponent<CharacterStats>();
            _currentHealth = MaxHealth;
            _healthInitialized = true;
        }

        private void Start()
        {
            EnsureInitialized();
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

        public DamageResult TakeDamage(DamageContext context)
        {
            EnsureInitialized();
            if (IsDead) return default;

            StatType reductionStat = context.Type.ToReductionStat();
            float reduction = _stats != null ? _stats.GetDamageReduction(reductionStat) : 0f;
            float effectiveReduction = reduction - context.ArmorPenetration;
            effectiveReduction = Mathf.Clamp(effectiveReduction, 0f, 0.9f);

            float multiplier = 1f;
            if (_stats != null)
            {
                multiplier = context.Type == DamageType.Physical
                    ? _stats.GetPhysicalDamageMultiplier()
                    : _stats.GetMagicalDamageMultiplier();
            }

            float finalDamage = context.BaseAmount * (1f - effectiveReduction) * multiplier;

            bool wasBlocked = context.IsBlocked;
            bool wasDodged = context.IsDodged;

            if (wasDodged) finalDamage = 0f;
            if (wasBlocked) finalDamage *= (1f - 0.5f); // TODO Step 4: move BlockReduction to CharacterStats

            CurrentHealth -= finalDamage;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);

            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

            bool killed = false;
            if (CurrentHealth <= 0f)
            {
                Die();
                killed = true;
            }

            var result = new DamageResult
            {
                FinalDamage = finalDamage,
                WasBlocked = wasBlocked,
                WasDodged = wasDodged,
                WasCritical = context.IsCritical,
                Killed = killed,
                AppliedTags = MagicTag.None
            };

            OnHit?.Invoke(context, result);
            return result;
        }

        public void Heal(float amount)
        {
            EnsureInitialized();
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
