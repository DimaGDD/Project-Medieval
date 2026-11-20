using System;
using UnityEngine;

namespace Game.Features.LevelingSystem
{
    public class ResourcePool : MonoBehaviour
    {
        [SerializeField] private StatType _maxStatType = StatType.MaxMana;
        [SerializeField] private StatType _regenStatType = StatType.ManaRegen;
        [SerializeField] private float _regenInterval = 1f;

        public float Current { get; private set; }
        public float Max => _stats != null ? _stats.GetStat(_maxStatType) : 0f;

        public event Action<float, float> OnChanged;

        private CharacterStats _stats;
        private float _timer;

        private void Awake()
        {
            _stats = GetComponent<CharacterStats>();
        }

        private void Start()
        {
            Current = Max;
            OnChanged?.Invoke(Current, Max);

            if (_stats != null) _stats.OnStatChanged += HandleStatChanged;
        }

        private void OnDestroy()
        {
            if (_stats != null) _stats.OnStatChanged -= HandleStatChanged;
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer < _regenInterval) return;

            _timer -= _regenInterval;

            float regen = _stats != null ? _stats.GetStat(_regenStatType) : 0f;
            if (regen > 0f) Restore(regen);
        }

        private void HandleStatChanged(StatType type)
        {
            if (type != _maxStatType) return;

            Current = Mathf.Min(Current, Max);
            OnChanged?.Invoke(Current, Max);
        }

        public void Restore(float amount)
        {
            if (amount <= 0f) return;

            Current = Mathf.Min(Current + amount, Max);
            OnChanged?.Invoke(Current, Max);
        }

        public bool TrySpend(float amount)
        {
            if (Current < amount) return false;

            Current -= amount;
            OnChanged?.Invoke(Current, Max);
            return true;
        }
    }
}