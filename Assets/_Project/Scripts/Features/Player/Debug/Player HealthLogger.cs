using UnityEngine;

namespace Game.Features.Player
{
    public class PlayerHealthLogger : MonoBehaviour
    {
        [SerializeField] private PlayerHealth _health;
        [SerializeField] private float _interval = 5f;

        private float _timer;

        private void Awake()
        {
            if (_health == null) _health = GetComponent<PlayerHealth>();
        }

        private void Update()
        {
            if (_health == null) return;

            _timer += Time.deltaTime;
            if (_timer < _interval) return;

            _timer -= _interval;
            Debug.Log($"[HP] {_health.CurrentHealth:F1} / {_health.MaxHealth:F1}");
        }
    }
}