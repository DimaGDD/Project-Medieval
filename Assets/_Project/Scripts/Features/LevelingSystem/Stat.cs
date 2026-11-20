using System.Collections.Generic;

namespace Game.Features.LevelingSystem
{
    public class Stat
    {
        public float BaseValue { get; private set; }

        private readonly List<StatModifier> _modifiers = new();
        private float _cachedValue;
        private bool _isDirty = true;

        public Stat(float baseValue)
        {
            BaseValue = baseValue;
        }

        public float Value
        {
            get
            {
                if (_isDirty) Recalculate();
                return _cachedValue;
            }
        }

        public void SetBaseValue(float value)
        {
            BaseValue = value;
            _isDirty = true;
        }

        public void AddModifier(StatModifier modifier)
        {
            _modifiers.Add(modifier);
            _isDirty = true;
        }

        public void RemoveModifier(StatModifier modifier)
        {
            _modifiers.Remove(modifier);
            _isDirty = true;
        }

        public void RemoveModifiersFromSource(object source)
        {
            _modifiers.RemoveAll(m => m._source == source);
            _isDirty = true;
        }

        private void Recalculate()
        {
            float flatSum = 0f;
            float percentSum = 0f;

            foreach (var mod in _modifiers)
            {
                if (mod._type == ModifierType.Flat) flatSum += mod._value;
                else percentSum += mod._value;
            }

            _cachedValue = (BaseValue + flatSum) * (1f + percentSum);
            _isDirty = false;
        }
    }
}