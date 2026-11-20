using System.Collections.Generic;
using UnityEngine;

namespace Game.Features.LevelingSystem
{
    public class SessionProgressionManager : MonoBehaviour
    {
        private CharacterStats _stats;
        private readonly List<SO_Item> _appliedItems = new();

        public bool IsApplied(SO_Item item) => _appliedItems.Contains(item);

        private void Awake()
        {
            _stats = GetComponent<CharacterStats>();
        }

        public void ApplyItem(SO_Item item)
        {
            if (item == null || _appliedItems.Contains(item)) return;

            foreach (var raw in item._modifiers)
            {
                var modifier = new StatModifier(raw._statType, raw._type, raw._value, item);
                _stats.AddModifier(modifier);
            }

            _appliedItems.Add(item);
            Debug.Log($"[Session] Item applied: {item._itemName}");
        }

        public void RemoveItem(SO_Item item)
        {
            if (!_appliedItems.Contains(item)) return;

            _stats.RemoveModifiersFromSource(item);
            _appliedItems.Remove(item);
        }

        public void ClearSession()
        {
            foreach (var item in _appliedItems)
                _stats.RemoveModifiersFromSource(item);

            _appliedItems.Clear();
            Debug.Log("[Session] Session cleared");
        }
    }
}