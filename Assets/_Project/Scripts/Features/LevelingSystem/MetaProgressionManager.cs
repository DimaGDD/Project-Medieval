using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Features.LevelingSystem
{
    [DefaultExecutionOrder(-1000)]
    public class MetaProgressionManager : MonoBehaviour
    {
        private const string SaveKey = "MetaProgression";

        private static MetaProgressionManager _instance;
        public static MetaProgressionManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindFirstObjectByType<MetaProgressionManager>();
                return _instance;
            }
        }

        [SerializeField] private List<SO_MetaUpgrade> _availableUpgrades = new();

        private readonly List<SO_MetaUpgrade> _purchasedUpgrades = new();

        [Serializable]
        private class SaveData
        {
            public List<string> _purchasedIds = new();
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
            LoadProgression();
        }

        public void ApplyMetaProgression(CharacterStats stats)
        {
            foreach (var upgrade in _purchasedUpgrades)
            {
                foreach (var modifier in upgrade._modifiers)
                    stats.AddModifier(modifier);
            }
        }

        public bool IsPurchased(SO_MetaUpgrade upgrade)
            => _purchasedUpgrades.Exists(u => u._upgradeId == upgrade._upgradeId);

        public bool PurchaseUpgrade(SO_MetaUpgrade upgrade)
        {
            if (upgrade == null || IsPurchased(upgrade)) return false;

            // TODO: списать золото (добавить GoldWallet в будущем)
            _purchasedUpgrades.Add(upgrade);
            SaveProgression();

            Debug.Log($"[Meta] Purchased: {upgrade._displayName}");
            return true;
        }

        public void ResetProgression()
        {
            _purchasedUpgrades.Clear();
            PlayerPrefs.DeleteKey(SaveKey);
            PlayerPrefs.Save();
            Debug.Log("[Meta] Progression reset");
        }

        private void SaveProgression()
        {
            var data = new SaveData();
            foreach (var u in _purchasedUpgrades) data._purchasedIds.Add(u._upgradeId);

            PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(data));
            PlayerPrefs.Save();
        }

        private void LoadProgression()
        {
            string json = PlayerPrefs.GetString(SaveKey, string.Empty);
            if (string.IsNullOrEmpty(json)) return;

            var data = JsonUtility.FromJson<SaveData>(json);
            if (data == null) return;

            foreach (var id in data._purchasedIds)
            {
                var upgrade = _availableUpgrades.Find(u => u._upgradeId == id);
                if (upgrade != null) _purchasedUpgrades.Add(upgrade);
                else Debug.LogWarning($"[Meta] Upgrade '{id}' not found in _availableUpgrades.");
            }

            Debug.Log($"[Meta] Loaded {_purchasedUpgrades.Count} upgrades.");
        }
    }
}