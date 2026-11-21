using System;
using System.Collections.Generic;

namespace Game.Features.Core.Tags
{
    public readonly struct TagMask : IEquatable<TagMask>
    {
        private readonly ulong _value;

        public ulong Value => _value;
        public bool IsEmpty => _value == 0;

        public static readonly TagMask None = new TagMask(0UL);
        public static readonly TagMask All = new TagMask((1UL << 52) - 1);

        public TagMask(ulong value)
        {
            _value = value;
        }

        public TagMask(MagicTag tag)
        {
            _value = (ulong)tag;
        }

        public bool Has(MagicTag tag)
        {
            if (tag == MagicTag.None) return false;
            ulong t = (ulong)tag;
            return (_value & t) == t;
        }

        public bool Has(TagMask other)
        {
            if (other._value == 0) return false;
            return (_value & other._value) == other._value;
        }

        public bool HasAny(MagicTag tag)
        {
            if (tag == MagicTag.None) return false;
            return (_value & (ulong)tag) != 0;
        }

        public bool HasAny(TagMask other)
        {
            if (other._value == 0) return false;
            return (_value & other._value) != 0;
        }

        public TagMask Add(MagicTag tag)
        {
            return new TagMask(_value | (ulong)tag);
        }

        public TagMask Add(TagMask other)
        {
            return new TagMask(_value | other._value);
        }

        public TagMask Remove(MagicTag tag)
        {
            return new TagMask(_value & ~(ulong)tag);
        }

        public TagMask Remove(TagMask other)
        {
            return new TagMask(_value & ~other._value);
        }

        public static TagMask operator |(TagMask a, TagMask b)
        {
            return new TagMask(a._value | b._value);
        }

        public static TagMask operator &(TagMask a, TagMask b)
        {
            return new TagMask(a._value & b._value);
        }

        public static TagMask operator ~(TagMask mask)
        {
            return new TagMask(~mask._value & All._value);
        }

        public static bool operator ==(TagMask a, TagMask b)
        {
            return a._value == b._value;
        }

        public static bool operator !=(TagMask a, TagMask b)
        {
            return a._value != b._value;
        }

        public bool Equals(TagMask other)
        {
            return _value == other._value;
        }

        public override bool Equals(object obj)
        {
            return obj is TagMask other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override string ToString()
        {
            if (_value == 0) return "None";
            var names = new List<string>();
            foreach (MagicTag tag in Enum.GetValues(typeof(MagicTag)))
            {
                if (tag == MagicTag.None) continue;
                ulong bit = (ulong)tag;
                if ((_value & bit) == bit)
                {
                    names.Add(TagCatalog.GetName(tag));
                }
            }
            return string.Join(" | ", names);
        }

        public static implicit operator TagMask(MagicTag tag)
        {
            return new TagMask(tag);
        }

        public static explicit operator TagMask(ulong value)
        {
            return new TagMask(value);
        }

        public static explicit operator ulong(TagMask mask)
        {
            return mask._value;
        }
    }
}
