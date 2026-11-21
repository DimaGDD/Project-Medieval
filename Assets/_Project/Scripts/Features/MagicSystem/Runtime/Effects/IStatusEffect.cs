using System;
using UnityEngine;
using Game.Features.Core.Tags;
using Game.Features.MagicSystem.Statuses;

namespace Game.Features.MagicSystem
{
    public interface IStatusEffect
    {
        void Apply(StatusInstance instance);
    }

    public abstract class StatusEffect : ScriptableObject, IStatusEffect
    {
        public virtual void Apply(StatusInstance instance) { }
    }
}
