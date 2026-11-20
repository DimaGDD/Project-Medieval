using Game.Features.LevelingSystem;
using UnityEditor.Graphs;

namespace Game.Features.Player
{
    public static class DamageTypeExtensions
    {
        public static StatType ToReductionStat(this DamageType type)
        {
            return type switch
            {
                DamageType.Physical => StatType.DamageReductionPhysical,
                DamageType.Fire => StatType.DamageReductionFire,
                DamageType.Ice => StatType.DamageReductionIce,
                DamageType.Lightning => StatType.DamageReductionLightning,
                DamageType.Poison => StatType.DamageReductionPoison,
                DamageType.Bleed => StatType.DamageReductionBleed,
                DamageType.Dark => StatType.DamageReductionDark,
                DamageType.Light => StatType.DamageReductionLight,
                DamageType.Nature => StatType.DamageReductionNature,
                _ => StatType.DamageReductionPhysical
            };
        }
    }
}