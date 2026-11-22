using System;
using UnityEngine;

namespace Game.Features.Core.Combat
{
    public interface IDamageable
    {
        event Action<DamageContext, DamageResult> OnHit;
        DamageResult TakeDamage(DamageContext context);
    }
}
