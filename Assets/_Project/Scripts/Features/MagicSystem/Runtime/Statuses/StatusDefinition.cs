using System;
using UnityEngine;
using Game.Features.Core.Tags;
using Game.Features.MagicSystem;

namespace Game.Features.MagicSystem.Statuses
{
    [CreateAssetMenu(fileName = "NewStatus", menuName = "Game/Magic/Status")]
    public class StatusDefinition : ScriptableObject
    {
        [SerializeField] private string _id = string.Empty;
        [SerializeField] private ulong _tags;
        [SerializeField] private float _defaultDuration = 3f;
        [SerializeField] private int _maxStacks = 5;
        [SerializeField] private StatusStacking _stacking = StatusStacking.Refresh;
        [SerializeField] private bool _isDebuff = true;
        [SerializeField] private float _tickInterval = 0.5f;
        [SerializeField] private StatusEffect[] _onApplyEffects;
        [SerializeField] private StatusEffect[] _onTickEffects;
        [SerializeField] private StatusEffect[] _onExpireEffects;

        public string Id { get => _id; set => _id = value; }
        public MagicTag Tags { get => (MagicTag)_tags; set => _tags = (ulong)value; }
        public float DefaultDuration { get => _defaultDuration; set => _defaultDuration = value; }
        public int MaxStacks { get => _maxStacks; set => _maxStacks = value; }
        public StatusStacking Stacking { get => _stacking; set => _stacking = value; }
        public bool IsDebuff { get => _isDebuff; set => _isDebuff = value; }
        public float TickInterval { get => _tickInterval; set => _tickInterval = value; }
        public StatusEffect[] OnApplyEffects { get => _onApplyEffects; set => _onApplyEffects = value; }
        public StatusEffect[] OnTickEffects { get => _onTickEffects; set => _onTickEffects = value; }
        public StatusEffect[] OnExpireEffects { get => _onExpireEffects; set => _onExpireEffects = value; }

        public void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(_id))
            {
                Debug.LogWarning($"[StatusDefinition] Id is empty on '{name}'.", this);
            }
            if (_tickInterval <= 0f)
            {
                _tickInterval = 0.5f;
            }
            if (_maxStacks <= 0)
            {
                _maxStacks = 1;
            }
            if (_defaultDuration < 0f)
            {
                _defaultDuration = 0f;
            }
        }
    }
}
