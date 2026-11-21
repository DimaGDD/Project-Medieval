using System;
using System.Linq;
using System.Text.RegularExpressions;
using Game.Features.Core.Tags;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Game.Features.Core.Tests
{
    public class TagSystemTests
    {
        [Test]
        public void TagMask_Has_SingleBit_ReturnsTrue()
        {
            var mask = new TagMask(MagicTag.Fire);
            Assert.IsTrue(mask.Has(MagicTag.Fire));
        }

        [Test]
        public void TagMask_Has_MissingBit_ReturnsFalse()
        {
            var mask = new TagMask(MagicTag.Fire);
            Assert.IsFalse(mask.Has(MagicTag.Ice));
        }

        [Test]
        public void TagMask_Has_None_ReturnsFalse()
        {
            var mask = new TagMask(MagicTag.Fire);
            Assert.IsFalse(mask.Has(MagicTag.None));
        }

        [Test]
        public void TagMask_Has_TagMaskNone_ReturnsFalse()
        {
            var mask = new TagMask(MagicTag.Fire);
            Assert.IsFalse(mask.Has(TagMask.None));
        }

        [Test]
        public void TagMask_Has_PartialIntersection_ReturnsFalse()
        {
            var mask = new TagMask(MagicTag.Fire);
            Assert.IsFalse(mask.Has(new TagMask(MagicTag.Fire) | new TagMask(MagicTag.Ice)));
        }

        [Test]
        public void TagMask_HasAny_SharedBit_ReturnsTrue()
        {
            var mask = new TagMask(MagicTag.Fire);
            Assert.IsTrue(mask.HasAny(MagicTag.Fire | MagicTag.Ice));
            Assert.IsTrue(mask.HasAny(new TagMask(MagicTag.Fire) | new TagMask(MagicTag.Ice)));
        }

        [Test]
        public void TagMask_HasAny_None_ReturnsFalse()
        {
            var mask = new TagMask(MagicTag.Fire);
            Assert.IsFalse(mask.HasAny(MagicTag.None));
            Assert.IsFalse(mask.HasAny(TagMask.None));
        }

        [Test]
        public void TagMask_Add_DoesNotMutateOriginal()
        {
            var original = new TagMask(MagicTag.None);
            var added = original.Add(MagicTag.Fire);
            Assert.IsTrue(added.Has(MagicTag.Fire));
            Assert.IsTrue(original.IsEmpty);
        }

        [Test]
        public void TagMask_Remove_ClearsBit()
        {
            var mask = new TagMask(MagicTag.Fire) | new TagMask(MagicTag.Ice);
            var removed = mask.Remove(MagicTag.Fire);
            Assert.IsFalse(removed.Has(MagicTag.Fire));
            Assert.IsTrue(removed.Has(MagicTag.Ice));
        }

        [Test]
        public void TagMask_AddRemove_TagMaskOverloads()
        {
            var mask = TagMask.None;
            mask = mask.Add(new TagMask(MagicTag.Fire) | new TagMask(MagicTag.Ice));
            Assert.IsTrue(mask.Has(MagicTag.Fire));
            Assert.IsTrue(mask.Has(MagicTag.Ice));
            mask = mask.Remove(new TagMask(MagicTag.Fire));
            Assert.IsFalse(mask.Has(MagicTag.Fire));
            Assert.IsTrue(mask.Has(MagicTag.Ice));
        }

        [Test]
        public void TagMask_Operators_CombineAndIntersect()
        {
            var combined = new TagMask(MagicTag.Fire) | new TagMask(MagicTag.Ice);
            var intersected = combined & new TagMask(MagicTag.Fire);
            Assert.IsTrue(intersected.Has(MagicTag.Fire));
            Assert.IsFalse(intersected.Has(MagicTag.Ice));
        }

        [Test]
        public void TagMask_EqualityAndHashCode()
        {
            var a = new TagMask(MagicTag.Fire);
            var b = new TagMask(MagicTag.Fire);
            var c = new TagMask(MagicTag.Ice);
            Assert.AreEqual(a, b);
            Assert.IsTrue(a == b);
            Assert.IsFalse(a != b);
            Assert.AreNotEqual(a, c);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void TagMask_Equals_Object_ReturnsTrue()
        {
            object a = new TagMask(MagicTag.Fire);
            object b = new TagMask(MagicTag.Fire);
            Assert.IsTrue(a.Equals(b));
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void TagMask_Equals_Object_DifferentType_ReturnsFalse()
        {
            var mask = new TagMask(MagicTag.Fire);
            Assert.IsFalse(mask.Equals("not a tagmask"));
            Assert.IsFalse(mask.Equals(null));
        }

        [Test]
        public void TagMask_Not_InvertsOnlyKnownBits()
        {
            var mask = new TagMask(MagicTag.Fire);
            var inverted = ~mask;
            Assert.IsFalse(inverted.Has(MagicTag.Fire));
            Assert.IsTrue(inverted.Has(MagicTag.Ice));
            Assert.AreEqual(TagMask.All.Value & ~(ulong)MagicTag.Fire, inverted.Value);
        }

        [Test]
        public void TagMask_Not_StripsBitsOutsideKnownRange()
        {
            var mask = (TagMask)((ulong)MagicTag.Fire | (1UL << 63));
            var inverted = ~mask;
            Assert.IsFalse(((ulong)inverted & (1UL << 63)) != 0);
            Assert.IsFalse(inverted.Has(MagicTag.Fire));
        }

        [Test]
        public void TagMask_ToString_FormatsNamesAndNone()
        {
            Assert.AreEqual("None", TagMask.None.ToString());
            Assert.AreEqual("Shrink", new TagMask(MagicTag.Shrink).ToString());
            Assert.AreEqual("Fire | Ice", (new TagMask(MagicTag.Fire) | new TagMask(MagicTag.Ice)).ToString());
        }

        [Test]
        public void TagCatalog_Get_KnownName_ReturnsTag()
        {
            Assert.AreEqual(MagicTag.Shrink, TagCatalog.Get("Shrink"));
        }

        [Test]
        public void TagCatalog_Get_UnknownName_LogsErrorAndReturnsNone()
        {
            LogAssert.Expect(LogType.Error, new Regex(".*"));
            Assert.AreEqual(MagicTag.None, TagCatalog.Get("DefinitelyUnknownTag"));
        }

        [Test]
        public void TagCatalog_Get_Null_LogsErrorAndReturnsNone()
        {
            LogAssert.Expect(LogType.Error, new Regex(".*"));
            Assert.AreEqual(MagicTag.None, TagCatalog.Get(null));
        }

        [Test]
        public void TagCatalog_Get_LiteralNoneString_LogsErrorAndReturnsNone()
        {
            LogAssert.Expect(LogType.Error, new Regex(".*"));
            Assert.AreEqual(MagicTag.None, TagCatalog.Get("None"));
        }

        [Test]
        public void TagCatalog_TryGet_Unknown_ReturnsFalseWithoutLog()
        {
            Assert.IsFalse(TagCatalog.TryGet("DefinitelyUnknownTag", out _));
        }

        [Test]
        public void TagCatalog_TryGet_Lowercase_ReturnsFalse()
        {
            Assert.IsFalse(TagCatalog.TryGet("fire", out _));
        }

        [Test]
        public void TagCatalog_Names_ContainsAllEnumNamesExceptNone()
        {
            var enumNames = Enum.GetNames(typeof(MagicTag)).Where(n => n != "None").ToArray();
            Assert.AreEqual(enumNames.Length, TagCatalog.Names.Count);
            Assert.IsFalse(TagCatalog.Names.Contains("None"));
            Assert.IsTrue(TagCatalog.Names.Contains("Shrink"));
            foreach (var n in enumNames)
            {
                Assert.IsTrue(TagCatalog.Names.Contains(n));
            }
        }

        [Test]
        public void TagCatalog_Names_AreSorted()
        {
            for (int i = 1; i < TagCatalog.Names.Count; i++)
            {
                Assert.LessOrEqual(string.Compare(TagCatalog.Names[i - 1], TagCatalog.Names[i], StringComparison.Ordinal), 0);
            }
        }

        [Test]
        public void MagicTag_AllValues_AreDistinctPowersOfTwo()
        {
            var values = Enum.GetValues(typeof(MagicTag)).Cast<MagicTag>().Where(t => t != MagicTag.None).Select(t => (ulong)t).ToArray();
            Assert.AreEqual(values.Length, values.Distinct().Count());
            foreach (var v in values)
            {
                Assert.IsTrue(v != 0 && (v & (v - 1)) == 0);
            }
        }

        [Test]
        public void MagicTag_SerializesThroughUlongBackingField()
        {
            var probe = new TagSerializationProbe { Tag = MagicTag.Shrink };
            string json = JsonUtility.ToJson(probe);
            var restored = JsonUtility.FromJson<TagSerializationProbe>(json);
            Assert.AreEqual(MagicTag.Shrink, restored.Tag);
            Assert.AreNotEqual(0UL, restored.Tag);
        }

        [Serializable]
        private class TagSerializationProbe
        {
            [SerializeField] private ulong _tag;
            public MagicTag Tag { get => (MagicTag)_tag; set => _tag = (ulong)value; }
        }
    }
}
