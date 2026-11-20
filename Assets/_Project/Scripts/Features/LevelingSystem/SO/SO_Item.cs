using System.Collections.Generic;
using UnityEngine;

namespace Game.Features.LevelingSystem
{
    [CreateAssetMenu(fileName = "NewItem", menuName = "Game/Item")]
    public class SO_Item : ScriptableObject
    {
        public string _itemName;
        public List<StatModifier> _modifiers = new();
    }
}