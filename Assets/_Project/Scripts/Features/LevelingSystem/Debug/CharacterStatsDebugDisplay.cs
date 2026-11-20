using UnityEngine;

namespace Game.Features.LevelingSystem.DebugTools
{
    public class CharacterStatsDebugDisplay : MonoBehaviour
    {
        [SerializeField] private CharacterStats _stats;

        private void OnGUI()
        {
            if (_stats == null) return;

            GUILayout.BeginArea(new Rect(10, 10, 340, 820));
            GUILayout.Label("<b>Character Stats</b>", new GUIStyle(GUI.skin.label) { richText = true });

            foreach (StatType type in System.Enum.GetValues(typeof(StatType)))
                GUILayout.Label($"{type}: {_stats.GetStat(type):F2}");

            GUILayout.EndArea();
        }
    }
}