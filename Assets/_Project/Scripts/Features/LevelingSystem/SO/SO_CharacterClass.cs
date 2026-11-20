using System.Collections.Generic;
using UnityEngine;

namespace Game.Features.LevelingSystem
{
    [CreateAssetMenu(fileName = "NewCharacterClass", menuName = "Game/Character Class")]
    public class SO_CharacterClass : ScriptableObject
    {
        [System.Serializable]
        public struct StatEntry
        {
            public StatType _type;
            public float _value;
        }

        [SerializeField] private List<StatEntry> _baseStats = new();

        public float GetBaseStat(StatType type)
        {
            foreach (var entry in _baseStats)
                if (entry._type == type) return entry._value;
            return 0f;
        }
    }
}