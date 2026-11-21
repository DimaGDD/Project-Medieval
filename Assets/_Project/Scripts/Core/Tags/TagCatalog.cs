using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Features.Core.Tags
{
    public static class TagCatalog
    {
        private static readonly Dictionary<string, MagicTag> _byName;
        private static readonly Dictionary<MagicTag, string> _nameByTag;
        private static readonly string[] _sortedNames;

        static TagCatalog()
        {
            _byName = new Dictionary<string, MagicTag>(StringComparer.Ordinal);
            _nameByTag = new Dictionary<MagicTag, string>();
            var names = new List<string>();

            foreach (MagicTag tag in Enum.GetValues(typeof(MagicTag)))
            {
                if (tag == MagicTag.None) continue;

                string name = tag.ToString();

                if (_byName.ContainsKey(name) || _nameByTag.ContainsKey(tag))
                {
                    Debug.LogError($"TagCatalog: duplicate tag name or value '{name}' ({tag}); skipped.");
                    continue;
                }

                _byName[name] = tag;
                _nameByTag[tag] = name;
                names.Add(name);
            }

            names.Sort(StringComparer.Ordinal);
            _sortedNames = names.ToArray();
        }

        public static bool TryGet(string name, out MagicTag tag)
        {
            tag = MagicTag.None;
            if (string.IsNullOrWhiteSpace(name)) return false;
            return _byName.TryGetValue(name, out tag);
        }

        public static MagicTag Get(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                Debug.LogError("TagCatalog.Get: name is null or whitespace.");
                return MagicTag.None;
            }

            if (_byName.TryGetValue(name, out var tag))
            {
                return tag;
            }

            Debug.LogError($"TagCatalog.Get: unknown tag name '{name}'.");
            return MagicTag.None;
        }

        public static IReadOnlyList<string> Names => _sortedNames;

        public static string GetName(MagicTag tag)
        {
            if (_nameByTag.TryGetValue(tag, out var name))
            {
                return name;
            }
            return tag.ToString();
        }
    }
}
