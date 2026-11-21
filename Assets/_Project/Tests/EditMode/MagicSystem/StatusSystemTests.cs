using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Game.Features.Core.Tags;
using Game.Features.MagicSystem;
using Game.Features.MagicSystem.Statuses;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Game.Features.MagicSystem.Tests
{
    public class CountingEffect : StatusEffect
    {
        public int ApplyCount;
        public override void Apply(StatusInstance instance) { ApplyCount++; }
    }

    public class StatusSystemTests
    {
        private List<UnityEngine.Object> _created = new List<UnityEngine.Object>();

        [SetUp]
        public void SetUp()
        {
            StatusTickSystem.ResetStatics();
            _created.Clear();
        }

        [TearDown]
        public void TearDown()
        {
            for (int i = 0; i < _created.Count; i++)
            {
                if (_created[i] != null) UnityEngine.Object.DestroyImmediate(_created[i]);
            }
            _created.Clear();
            StatusTickSystem.ResetStatics();
        }

        private StatusDefinition MakeDef(string id = "burn", MagicTag tags = MagicTag.Burn, float duration = 3f, int maxStacks = 5, StatusStacking stacking = StatusStacking.Refresh, float tickInterval = 0.5f)
        {
            var def = ScriptableObject.CreateInstance<StatusDefinition>();
            def.Id = id;
            def.Tags = tags;
            def.DefaultDuration = duration;
            def.MaxStacks = maxStacks;
            def.Stacking = stacking;
            def.TickInterval = tickInterval;
            _created.Add(def);
            return def;
        }

        private GameObject MakeGo(string name = "Entity")
        {
            var go = new GameObject(name);
            _created.Add(go);
            return go;
        }

        [Test]
        public void StatusDefinition_SerializesTagsViaUlongBacking()
        {
            var def = MakeDef(tags: MagicTag.Fire);
            string json = JsonUtility.ToJson(def);
            var restored = ScriptableObject.CreateInstance<StatusDefinition>();
            _created.Add(restored);
            JsonUtility.FromJsonOverwrite(json, restored);
            Assert.AreEqual(MagicTag.Fire, restored.Tags);
        }

        [Test]
        public void StatusDefinition_OnValidate_RejectsEmptyId()
        {
            var def = ScriptableObject.CreateInstance<StatusDefinition>();
            _created.Add(def);
            def.Id = string.Empty;
            LogAssert.Expect(LogType.Warning, new Regex(".*"));
            def.OnValidate();
        }

        [Test]
        public void StatusController_ApplyAndRemove_UpdatesActiveList()
        {
            var def = MakeDef();
            var go = MakeGo();
            var ctrl = go.AddComponent<StatusController>();
            var src = new TestSource(go);
            ctrl.ApplyStatus(def, src);
            Assert.AreEqual(1, ctrl.ActiveStatuses.Count);
            Assert.IsTrue(ctrl.HasStatus(def));
            ctrl.RemoveStatus(def);
            Assert.AreEqual(0, ctrl.ActiveStatuses.Count);
            Assert.IsFalse(ctrl.HasStatus(def));
        }

        [Test]
        public void StatusController_HasStatusAndGetStacks()
        {
            var def = MakeDef();
            var go = MakeGo();
            var ctrl = go.AddComponent<StatusController>();
            var src = new TestSource(go);
            ctrl.ApplyStatus(def, src, 2);
            Assert.IsTrue(ctrl.HasStatus(def));
            Assert.AreEqual(2, ctrl.GetStacks(def));
            Assert.IsFalse(ctrl.HasStatus(MakeDef("other")));
        }

        [Test]
        public void StatusController_GetAggregateTags_Combines()
        {
            var def1 = MakeDef("a", MagicTag.Fire);
            var def2 = MakeDef("b", MagicTag.Ice);
            var go = MakeGo();
            var ctrl = go.AddComponent<StatusController>();
            var src = new TestSource(go);
            ctrl.ApplyStatus(def1, src);
            ctrl.ApplyStatus(def2, src);
            var agg = ctrl.GetAggregateTags();
            Assert.IsTrue(agg.Has(MagicTag.Fire));
            Assert.IsTrue(agg.Has(MagicTag.Ice));
        }

        [Test]
        public void StatusController_Stacking_RefreshUpdatesDuration()
        {
            var def = MakeDef(stacking: StatusStacking.Refresh, duration: 3f);
            var go = MakeGo();
            var ctrl = go.AddComponent<StatusController>();
            var src = new TestSource(go);
            var first = ctrl.ApplyStatus(def, src, 1, 3f);
            first.RemainingTime = 1f;
            var second = ctrl.ApplyStatus(def, src, 1, 5f);
            Assert.AreSame(first, second);
            Assert.AreEqual(5f, second.RemainingTime, 0.001f);
        }

        [Test]
        public void StatusController_Stacking_StackIncreasesStacks()
        {
            var def = MakeDef(stacking: StatusStacking.Stack, duration: 3f, maxStacks: 5);
            var go = MakeGo();
            var ctrl = go.AddComponent<StatusController>();
            var src = new TestSource(go);
            ctrl.ApplyStatus(def, src, 2, 3f);
            ctrl.ApplyStatus(def, src, 2, 3f);
            Assert.AreEqual(4, ctrl.GetStacks(def));
        }

        [Test]
        public void StatusController_Stacking_IndependentCreatesSeparateInstances()
        {
            var def = MakeDef(stacking: StatusStacking.Independent, duration: 3f);
            var go = MakeGo();
            var ctrl = go.AddComponent<StatusController>();
            var src = new TestSource(go);
            var a = ctrl.ApplyStatus(def, src, 1, 3f);
            var b = ctrl.ApplyStatus(def, src, 1, 3f);
            Assert.AreNotSame(a, b);
            Assert.AreEqual(2, ctrl.ActiveStatuses.Count);
        }

        [Test]
        public void StatusController_Tick_RemovesOnExpiry()
        {
            var def = MakeDef(duration: 1f, tickInterval: 0.5f);
            var go = MakeGo();
            var ctrl = go.AddComponent<StatusController>();
            var src = new TestSource(go);
            ctrl.ApplyStatus(def, src, 1, 1f);
            ctrl.Tick(1.1f);
            Assert.AreEqual(0, ctrl.ActiveStatuses.Count);
            Assert.IsFalse(ctrl.HasStatus(def));
        }

        [Test]
        public void StatusController_Tick_InvokesOnStatusTicked()
        {
            var def = MakeDef(duration: 3f, tickInterval: 0.5f);
            var go = MakeGo();
            var ctrl = go.AddComponent<StatusController>();
            var src = new TestSource(go);
            ctrl.ApplyStatus(def, src, 1, 3f);
            int ticks = 0;
            ctrl.OnStatusTicked += _ => ticks++;
            ctrl.Tick(0.5f);
            Assert.GreaterOrEqual(ticks, 1);
        }

        [Test]
        public void StatusController_ApplyStatus_InvokesOnStatusApplied()
        {
            var def = MakeDef();
            var go = MakeGo();
            var ctrl = go.AddComponent<StatusController>();
            var src = new TestSource(go);
            StatusInstance received = null;
            ctrl.OnStatusApplied += i => received = i;
            var inst = ctrl.ApplyStatus(def, src);
            Assert.AreSame(inst, received);
        }

        [Test]
        public void StatusController_RemoveStatus_InvokesOnStatusRemoved()
        {
            var def = MakeDef();
            var go = MakeGo();
            var ctrl = go.AddComponent<StatusController>();
            var src = new TestSource(go);
            ctrl.ApplyStatus(def, src);
            StatusInstance removed = null;
            ctrl.OnStatusRemoved += i => removed = i;
            ctrl.RemoveStatus(def);
            Assert.NotNull(removed);
        }

        [Test]
        public void StatusController_ApplyStatus_InvokesOnApplyEffects()
        {
            var def = MakeDef();
            var effect = ScriptableObject.CreateInstance<CountingEffect>();
            _created.Add(effect);
            def.OnApplyEffects = new StatusEffect[] { effect };
            var go = MakeGo();
            var ctrl = go.AddComponent<StatusController>();
            var src = new TestSource(go);
            ctrl.ApplyStatus(def, src);
            Assert.AreEqual(1, effect.ApplyCount);
        }

        [Test]
        public void StatusController_Tick_InvokesOnTickEffects()
        {
            var def = MakeDef(duration: 3f, tickInterval: 0.5f);
            var effect = ScriptableObject.CreateInstance<CountingEffect>();
            _created.Add(effect);
            def.OnTickEffects = new StatusEffect[] { effect };
            var go = MakeGo();
            var ctrl = go.AddComponent<StatusController>();
            var src = new TestSource(go);
            ctrl.ApplyStatus(def, src, 1, 3f);
            ctrl.Tick(0.5f);
            Assert.GreaterOrEqual(effect.ApplyCount, 1);
        }

        [Test]
        public void StatusController_Tick_InvokesOnExpireEffects_BeforeOnStatusRemoved()
        {
            var def = MakeDef(duration: 0.1f, tickInterval: 10f);
            var effect = ScriptableObject.CreateInstance<CountingEffect>();
            _created.Add(effect);
            def.OnExpireEffects = new StatusEffect[] { effect };
            var go = MakeGo();
            var ctrl = go.AddComponent<StatusController>();
            var src = new TestSource(go);
            ctrl.ApplyStatus(def, src, 1, 0.1f);
            bool effectAppliedBeforeRemoved = false;
            ctrl.OnStatusRemoved += _ => { effectAppliedBeforeRemoved = effect.ApplyCount > 0; };
            ctrl.Tick(0.2f);
            Assert.IsTrue(effectAppliedBeforeRemoved);
            Assert.GreaterOrEqual(effect.ApplyCount, 1);
        }

        [Test]
        public void StatusController_GetAggregateTags_InvalidatesCacheOnChange()
        {
            var def1 = MakeDef("a", MagicTag.Fire);
            var def2 = MakeDef("b", MagicTag.Ice);
            var go = MakeGo();
            var ctrl = go.AddComponent<StatusController>();
            var src = new TestSource(go);
            ctrl.ApplyStatus(def1, src);
            var first = ctrl.GetAggregateTags();
            Assert.IsTrue(first.Has(MagicTag.Fire));
            ctrl.ApplyStatus(def2, src);
            var second = ctrl.GetAggregateTags();
            Assert.IsTrue(second.Has(MagicTag.Ice));
            Assert.IsTrue(second.Has(MagicTag.Fire));
        }

        [Test]
        public void StatusController_RegisterUnregister_OnEnableDisable()
        {
            var go = MakeGo();
            var ctrl = go.AddComponent<StatusController>();
            Assert.IsTrue(ContainsController(ctrl));
            go.SetActive(false);
            Assert.IsFalse(ContainsController(ctrl));
            go.SetActive(true);
            Assert.IsTrue(ContainsController(ctrl));
        }

        [Test]
        public void StatusTickSystem_Register_AddsToList()
        {
            var go = MakeGo();
            var ctrl = go.AddComponent<StatusController>();
            // OnEnable registers
            Assert.IsTrue(ContainsController(ctrl));
        }

        [Test]
        public void StatusTickSystem_ResetStatics_ClearsList()
        {
            var go = MakeGo();
            var ctrl = go.AddComponent<StatusController>();
            StatusTickSystem.ResetStatics();
            Assert.IsFalse(ContainsController(ctrl));
        }

        [Test]
        public void StatusController_Refresh_StacksUnchanged()
        {
            var def = MakeDef(stacking: StatusStacking.Refresh, duration: 3f);
            var go = MakeGo();
            var ctrl = go.AddComponent<StatusController>();
            var src = new TestSource(go);
            ctrl.ApplyStatus(def, src, 3, 3f);
            ctrl.ApplyStatus(def, src, 1, 5f);
            Assert.AreEqual(3, ctrl.GetStacks(def));
        }

        [Test]
        public void StatusController_Stack_ClampsToMaxStacks()
        {
            var def = MakeDef(stacking: StatusStacking.Stack, duration: 3f, maxStacks: 3);
            var go = MakeGo();
            var ctrl = go.AddComponent<StatusController>();
            var src = new TestSource(go);
            ctrl.ApplyStatus(def, src, 2, 3f);
            ctrl.ApplyStatus(def, src, 2, 3f);
            Assert.AreEqual(3, ctrl.GetStacks(def));
        }

        [Test]
        public void StatusController_Tick_CatchUpMultipleIntervals()
        {
            var def = MakeDef(duration: 3f, tickInterval: 0.1f);
            var go = MakeGo();
            var ctrl = go.AddComponent<StatusController>();
            var src = new TestSource(go);
            ctrl.ApplyStatus(def, src, 1, 3f);
            int ticks = 0;
            ctrl.OnStatusTicked += _ => ticks++;
            ctrl.Tick(0.35f);
            Assert.AreEqual(3, ticks);
        }

        [Test]
        public void StatusController_ApplyStatus_NegativeDuration_UsesDefault()
        {
            var def = MakeDef(duration: 2.5f);
            var go = MakeGo();
            var ctrl = go.AddComponent<StatusController>();
            var src = new TestSource(go);
            var inst = ctrl.ApplyStatus(def, src, 1, -1f);
            Assert.AreEqual(2.5f, inst.RemainingTime, 0.001f);
        }

        private bool ContainsController(StatusController ctrl)
        {
            // access via reflection? Instead check StatusTickSystem static list through a public helper.
            // We'll use StatusTickSystem.Instance? Not enough. Add a public static accessor.
            return StatusTickSystem.Contains(ctrl);
        }

        private class TestSource : IStatusSource
        {
            public GameObject SourceObject { get; }
            public MagicTag SourceTags { get; } = MagicTag.None;
            public TestSource(GameObject go) { SourceObject = go; }
        }
    }
}
