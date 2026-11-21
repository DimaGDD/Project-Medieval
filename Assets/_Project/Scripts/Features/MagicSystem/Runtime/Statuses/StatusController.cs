using System;
using System.Collections.Generic;
using UnityEngine;
using Game.Features.Core.Tags;

namespace Game.Features.MagicSystem.Statuses
{
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-100)]
    [ExecuteAlways]
    public class StatusController : MonoBehaviour
    {
        public event Action<StatusInstance> OnStatusApplied;
        public event Action<StatusInstance> OnStatusRemoved;
        public event Action<StatusInstance> OnStatusTicked;

        private readonly List<StatusInstance> _active = new List<StatusInstance>();
        private TagMask _aggregateTags;
        private bool _aggregateDirty = true;

        public IReadOnlyList<StatusInstance> ActiveStatuses => _active;

        public StatusInstance ApplyStatus(StatusDefinition def, IStatusSource source, int stacks = 1, float duration = -1f)
        {
            if (def == null) return null;

            float resolvedDuration = duration >= 0f ? duration : def.DefaultDuration;
            int requestedStacks = stacks < 1 ? 1 : stacks;

            if (def.Stacking == StatusStacking.Independent)
            {
                var inst = new StatusInstance
                {
                    Definition = def,
                    Stacks = Mathf.Clamp(requestedStacks, 1, def.MaxStacks),
                    RemainingTime = resolvedDuration,
                    TickTimer = def.TickInterval,
                    Source = source,
                    Controller = this
                };
                _active.Add(inst);
                _aggregateDirty = true;
                RunEffects(def.OnApplyEffects, inst);
                OnStatusApplied?.Invoke(inst);
                return inst;
            }

            StatusInstance existing = null;
            for (int i = 0; i < _active.Count; i++)
            {
                if (_active[i].Definition == def)
                {
                    existing = _active[i];
                    break;
                }
            }

            if (existing == null)
            {
                var inst = new StatusInstance
                {
                    Definition = def,
                    Stacks = Mathf.Clamp(requestedStacks, 1, def.MaxStacks),
                    RemainingTime = resolvedDuration,
                    TickTimer = def.TickInterval,
                    Source = source,
                    Controller = this
                };
                _active.Add(inst);
                _aggregateDirty = true;
                RunEffects(def.OnApplyEffects, inst);
                OnStatusApplied?.Invoke(inst);
                return inst;
            }

            if (def.Stacking == StatusStacking.Refresh)
            {
                existing.RemainingTime = resolvedDuration;
            }
            else if (def.Stacking == StatusStacking.Stack)
            {
                existing.Stacks = Mathf.Min(def.MaxStacks, existing.Stacks + requestedStacks);
                existing.RemainingTime = resolvedDuration;
            }

            RunEffects(def.OnApplyEffects, existing);
            OnStatusApplied?.Invoke(existing);
            return existing;
        }

        public void RemoveStatus(StatusDefinition def)
        {
            if (def == null) return;

            for (int i = _active.Count - 1; i >= 0; i--)
            {
                if (_active[i].Definition != def) continue;
                var inst = _active[i];
                _active.RemoveAt(i);
                _aggregateDirty = true;
                OnStatusRemoved?.Invoke(inst);
            }
        }

        public bool HasStatus(StatusDefinition def)
        {
            for (int i = 0; i < _active.Count; i++)
            {
                if (_active[i].Definition == def) return true;
            }
            return false;
        }

        public int GetStacks(StatusDefinition def)
        {
            int total = 0;
            for (int i = 0; i < _active.Count; i++)
            {
                if (_active[i].Definition == def)
                {
                    total += _active[i].Stacks;
                }
            }
            return total;
        }

        public TagMask GetAggregateTags()
        {
            if (_aggregateDirty)
            {
                TagMask result = TagMask.None;
                for (int i = 0; i < _active.Count; i++)
                {
                    result = result | new TagMask(_active[i].Definition.Tags);
                }
                _aggregateTags = result;
                _aggregateDirty = false;
            }
            return _aggregateTags;
        }

        public void Tick(float deltaTime)
        {
            int i = 0;
            while (i < _active.Count)
            {
                var inst = _active[i];
                var def = inst.Definition;

                inst.RemainingTime -= deltaTime;
                inst.TickTimer -= deltaTime;

                while (inst.TickTimer <= 0f)
                {
                    RunEffects(def.OnTickEffects, inst);
                    OnStatusTicked?.Invoke(inst);
                    inst.TickTimer += def.TickInterval;
                }

                if (inst.RemainingTime <= 0f)
                {
                    RunEffects(def.OnExpireEffects, inst);
                    _active.RemoveAt(i);
                    _aggregateDirty = true;
                    OnStatusRemoved?.Invoke(inst);
                }
                else
                {
                    i++;
                }
            }
        }

        private void OnEnable()
        {
            StatusTickSystem.Register(this);
        }

        private void OnDisable()
        {
            StatusTickSystem.Unregister(this);
        }

        private static void RunEffects(StatusEffect[] effects, StatusInstance instance)
        {
            if (effects == null) return;
            for (int i = 0; i < effects.Length; i++)
            {
                if (effects[i] != null)
                {
                    effects[i].Apply(instance);
                }
            }
        }
    }
}
