using UnityEngine;
using Game.Features.Core.Tags;

namespace Game.Features.MagicSystem.Statuses
{
    public interface IStatusSource
    {
        GameObject SourceObject { get; }
        MagicTag SourceTags { get; }
    }
}
