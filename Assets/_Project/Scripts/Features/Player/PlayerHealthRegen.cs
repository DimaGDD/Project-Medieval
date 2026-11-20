using UnityEngine;
using Game.Features.LevelingSystem;

namespace Game.Features.Player
{
    public class PlayerHealthRegen : MonoBehaviour
    {
        [SerializeField] private float _tickInterval = 1f;

        private CharacterStats _stats;
        private PlayerHealth _health;
        private float _timer;

        private void Awake()
        {
            _stats = GetComponent<CharacterStats>();
            _health = GetComponent<PlayerHealth>();
        }

        private void Update()
        {
            if (_stats == null || _health == null || _health.IsDead) return;

            _timer += Time.deltaTime;
            if (_timer < _tickInterval) return;

            _timer -= _tickInterval;

            float regen = _stats.GetStat(StatType.HealthRegen);
            if (regen > 0f) _health.Heal(regen);
        }
    }
}