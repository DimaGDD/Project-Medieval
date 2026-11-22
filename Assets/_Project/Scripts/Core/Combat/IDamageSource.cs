using UnityEngine;
using Game.Features.Core.Tags;

namespace Game.Features.Core.Combat
{
    public interface IDamageSource
    {
        /// <summary>tags of the source itself (caster class, faction traits); not the tags of the current hit</summary>
        GameObject SourceObject { get; }
        Faction Faction { get; }
        MagicTag Tags { get; }
    }
}
