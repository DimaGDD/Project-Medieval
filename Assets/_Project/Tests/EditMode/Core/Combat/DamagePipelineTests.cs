using System;
using System.Reflection;
using Game.Features.Core.Combat;
using Game.Features.Core.Tags;
using Game.Features.LevelingSystem;
using Game.Features.Player;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.Features.Core.Tests
{
    public class DamagePipelineTests
    {
        private GameObject _go;
        private PlayerHealth _health;
        private CharacterStats _stats;
        private SO_CharacterClass _class;

        [SetUp]
        public void SetUp()
        {
            _class = AssetDatabase.LoadAssetAtPath<SO_CharacterClass>("Assets/_Project/Data/Classes/SO_CharacterClass_Warrior.asset");
            _go = new GameObject("Player");
            _stats = _go.AddComponent<CharacterStats>();

            var ccField = typeof(CharacterStats).GetField("_characterClass", BindingFlags.NonPublic | BindingFlags.Instance);
            ccField.SetValue(_stats, _class);
            var initField = typeof(CharacterStats).GetField("_initialized", BindingFlags.NonPublic | BindingFlags.Instance);
            initField.SetValue(_stats, false);
            _stats.GetStat(StatType.MaxHealth);

            _health = _go.AddComponent<PlayerHealth>();
        }

        [TearDown]
        public void TearDown()
        {
            if (_go != null) UnityEngine.Object.DestroyImmediate(_go);
        }

        private DamageContext MakeContext(float amount, DamageType type = DamageType.Physical, bool dodged = false, bool blocked = false, float armorPen = 0f)
        {
            return new DamageContext
            {
                BaseAmount = amount,
                Type = type,
                Tags = MagicTag.None,
                Target = _go,
                IsDodged = dodged,
                IsBlocked = blocked,
                ArmorPenetration = armorPen,
                IsCritical = false
            };
        }

        [Test]
        public void PlayerHealth_TakeDamage_AppliesReductionAndMultiplier()
        {
            _stats.AddModifier(new StatModifier(StatType.DamageReductionPhysical, ModifierType.Flat, 50f));
            _stats.AddModifier(new StatModifier(StatType.PhysicalDamagePercent, ModifierType.Flat, 50f));

            var ctx = MakeContext(100f);
            var result = _health.TakeDamage(ctx);

            Assert.AreEqual(75f, result.FinalDamage, 0.001f);
            Assert.AreEqual(45f, _health.CurrentHealth, 0.001f);
        }

        [Test]
        public void PlayerHealth_TakeDamage_ClampsHealthToZeroAndSetsKilled()
        {
            var ctx = MakeContext(1000f);
            var result = _health.TakeDamage(ctx);

            Assert.IsTrue(result.Killed);
            Assert.AreEqual(0f, _health.CurrentHealth, 0.001f);
            Assert.IsTrue(_health.IsDead);
        }

        [Test]
        public void PlayerHealth_TakeDamage_WhenDodged_DealsZero()
        {
            var ctx = MakeContext(100f, dodged: true);
            var result = _health.TakeDamage(ctx);

            Assert.AreEqual(0f, result.FinalDamage, 0.001f);
            Assert.IsTrue(result.WasDodged);
            Assert.AreEqual(120f, _health.CurrentHealth, 0.001f);
        }

        [Test]
        public void PlayerHealth_TakeDamage_WhenBlocked_ReducesDamage()
        {
            var ctx = MakeContext(100f, blocked: true);
            var result = _health.TakeDamage(ctx);

            Assert.AreEqual(50f, result.FinalDamage, 0.001f);
            Assert.IsTrue(result.WasBlocked);
        }

        [Test]
        public void PlayerHealth_TakeDamage_WhenDead_ReturnsZero()
        {
            _health.TakeDamage(MakeContext(1000f));
            var result = _health.TakeDamage(MakeContext(50f));

            Assert.AreEqual(0f, result.FinalDamage, 0.001f);
            Assert.IsFalse(result.Killed);
        }

        [Test]
        public void PlayerHealth_TakeDamage_RaisesOnHit()
        {
            bool raised = false;
            DamageContext receivedCtx = default;
            DamageResult receivedRes = default;
            _health.OnHit += (c, r) => { raised = true; receivedCtx = c; receivedRes = r; };

            var ctx = MakeContext(10f);
            var result = _health.TakeDamage(ctx);

            Assert.IsTrue(raised);
            Assert.AreEqual(ctx.BaseAmount, receivedCtx.BaseAmount, 0.001f);
            Assert.AreEqual(result.FinalDamage, receivedRes.FinalDamage, 0.001f);
        }
    }
}
