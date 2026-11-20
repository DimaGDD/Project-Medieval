using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Features.LevelingSystem
{
    [DefaultExecutionOrder(-100)]
    public class CharacterStats : MonoBehaviour
    {
        [SerializeField] private SO_CharacterClass _characterClass;

        private Dictionary<StatType, Stat> _stats;
        private bool _initialized;

        public event Action<StatType> OnStatChanged;

        private void Awake() => EnsureInitialized();

        private void EnsureInitialized()
        {
            if (_initialized) return;
            _initialized = true;

            if (_characterClass == null)
                Debug.LogWarning($"[CharacterStats] '{name}': _characterClass не назначен — все базовые статы будут 0.", this);

            InitializeStats();
            MetaProgressionManager.Instance?.ApplyMetaProgression(this);
        }

        private void InitializeStats()
        {
            _stats = new Dictionary<StatType, Stat>();
            foreach (StatType type in Enum.GetValues(typeof(StatType)))
            {
                float baseValue = _characterClass != null ? _characterClass.GetBaseStat(type) : 0f;
                _stats[type] = new Stat(baseValue);
            }
        }

        public float GetStat(StatType type)
        {
            EnsureInitialized();
            return _stats.TryGetValue(type, out var stat) ? stat.Value : 0f;
        }

        public void AddModifier(StatModifier modifier)
        {
            EnsureInitialized();
            if (!_stats.TryGetValue(modifier._statType, out var stat)) return;
            stat.AddModifier(modifier);
            OnStatChanged?.Invoke(modifier._statType);
        }

        public void RemoveModifier(StatModifier modifier)
        {
            EnsureInitialized();
            if (!_stats.TryGetValue(modifier._statType, out var stat)) return;
            stat.RemoveModifier(modifier);
            OnStatChanged?.Invoke(modifier._statType);
        }

        public void RemoveModifiersFromSource(object source)
        {
            EnsureInitialized();
            foreach (var kvp in _stats)
            {
                kvp.Value.RemoveModifiersFromSource(source);
                OnStatChanged?.Invoke(kvp.Key);
            }
        }

        public float GetDamageReduction(StatType reductionStat)
        {
            float raw = GetStat(reductionStat);
            float normalized = raw > 1f ? raw / 100f : raw;
            return Mathf.Clamp01(normalized);
        }

        public float GetPhysicalDamageMultiplier() => 1f + GetStat(StatType.PhysicalDamagePercent) / 100f;
        public float GetMagicalDamageMultiplier() => 1f + GetStat(StatType.MagicalDamagePercent) / 100f;
    }
}