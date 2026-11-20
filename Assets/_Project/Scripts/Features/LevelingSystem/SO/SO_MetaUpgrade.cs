using System.Collections.Generic;
using UnityEngine;

namespace Game.Features.LevelingSystem
{
    [CreateAssetMenu(fileName = "NewMetaUpgrade", menuName = "Game/Meta Upgrade")]
    public class SO_MetaUpgrade : ScriptableObject
    {
        [Tooltip("Уникальный ID для сохранения в PlayerPrefs. Не меняйте после релиза!")]
        public string _upgradeId;

        public string _displayName;
        public int _cost;

        public List<StatModifier> _modifiers = new();
    }
}