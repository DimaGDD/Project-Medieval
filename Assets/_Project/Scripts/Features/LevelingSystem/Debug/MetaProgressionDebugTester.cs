using UnityEngine;

namespace Game.Features.LevelingSystem.DebugTools
{
    public class MetaProgressionDebugTester : MonoBehaviour
    {
        [SerializeField] private SO_MetaUpgrade _testUpgrade;

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 200));

            GUILayout.Label("<b>Meta Progression Debug</b>", new GUIStyle(GUI.skin.label) { richText = true });

            if (_testUpgrade == null)
            {
                GUILayout.Label("_testUpgrade not assigned");
            }
            else
            {
                bool purchased = MetaProgressionManager.Instance.IsPurchased(_testUpgrade);
                GUILayout.Label($"Upgrade: {_testUpgrade._displayName} ({(purchased ? "purchased" : "not purchased")})");

                GUI.enabled = !purchased;
                if (GUILayout.Button("Purchase (restart Play to apply)"))
                {
                    bool ok = MetaProgressionManager.Instance.PurchaseUpgrade(_testUpgrade);
                    Debug.Log($"[MetaTest] Purchase '{_testUpgrade._displayName}': {ok}");
                }
                GUI.enabled = true;
            }

            if (GUILayout.Button("Reset Progression"))
                MetaProgressionManager.Instance.ResetProgression();

            GUILayout.EndArea();
        }
    }
}