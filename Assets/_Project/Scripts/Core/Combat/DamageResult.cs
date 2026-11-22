using Game.Features.Core.Tags;

namespace Game.Features.Core.Combat
{
    public struct DamageResult
    {
        public float FinalDamage;
        public bool WasBlocked;
        public bool WasDodged;
        public bool WasCritical;
        public bool Killed;
        public MagicTag AppliedTags;
    }
}
