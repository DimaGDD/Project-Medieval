using UnityEngine;
using Game.Features.Core.Tags;

namespace Game.Features.Core.Combat
{
    public struct DamageContext
    {
        public float BaseAmount;
        public DamageType Type;
        public MagicTag Tags;
        public IDamageSource Source;
        public GameObject Target;
        public Vector3 HitPoint;
        public Vector3 HitNormal;
        public Vector3 Direction;
        public bool IsCritical;
        public bool IsBlocked;
        public bool IsDodged;
        public float ArmorPenetration;
    }
}
