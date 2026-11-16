using UnityEngine;

namespace Game.Features.Player
{
    public interface IDamageable
    {
        void TakeDamage(DamageInfo damageInfo);
    }
}
