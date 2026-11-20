using System;

namespace Game.Features.LevelingSystem
{
    public enum ModifierType
    {
        Flat,
        Percent
    }

    [Serializable]
    public struct StatModifier
    {
        public StatType _statType;
        public ModifierType _type;
        public float _value;

        [NonSerialized] public object _source;

        public StatModifier(StatType statType, ModifierType type, float value, object source = null)
        {
            _statType = statType;
            _type = type;
            _value = value;
            _source = source;
        }
    }
}