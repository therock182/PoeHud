using System;
using System.Collections.Generic;
using PoeHUD.Game.Enums;
using PoeHUD.Poe;
using PoeHUD.Poe.EntityComponents;

namespace PoeHUD.Controllers
{
    public class ItemStats
    {
        private static StatTranslator translate;
        protected Entity item;
        protected float[] stats;

        public ItemStats(Entity item)
        {
            this.item = item;
            if (translate == null)
            {
                translate = new StatTranslator();
            }
            stats = new float[Enum.GetValues(typeof (ItemStat)).Length];
            ParseSockets();
            ParseExplicitMods();
            if (item.HasComponent<Weapon>())
            {
                ParseWeaponStats();
            }
        }

        private void ParseWeaponStats()
        {
            var component = item.GetComponent<Weapon>();
            float num = (component.DamageMin + component.DamageMax)/2f + GetStat(ItemStat.LocalPhysicalDamage);
            num *= 1f + (GetStat(ItemStat.LocalPhysicalDamagePercent) + item.GetComponent<Quality>().ItemQuality)/100f;
            AddToMod(ItemStat.AveragePhysicalDamage, num);
            float num2 = 1f/(component.AttackTime/1000f);
            num2 *= 1f + GetStat(ItemStat.LocalAttackSpeed)/100f;
            AddToMod(ItemStat.AttackPerSecond, num2);
            float num3 = component.CritChance/100f;
            num3 *= 1f + GetStat(ItemStat.LocalCritChance)/100f;
            AddToMod(ItemStat.WeaponCritChance, num3);
            float num4 = GetStat(ItemStat.LocalAddedColdDamage) + GetStat(ItemStat.LocalAddedFireDamage) +
                         GetStat(ItemStat.LocalAddedLightningDamage);
            AddToMod(ItemStat.AverageElementalDamage, num4);
            AddToMod(ItemStat.DPS, (num + num4)*num2);
            AddToMod(ItemStat.PhysicalDPS, num*num2);
        }

        private void ParseArmorStats()
        {
        }

        private void ParseExplicitMods()
        {
            foreach (ItemMod current in item.GetComponent<Mods>().ItemMods)
            {
                translate.Translate(this, current);
            }
            AddToMod(ItemStat.ElementalResistance,
                GetStat(ItemStat.LightningResistance) + GetStat(ItemStat.FireResistance) +
                GetStat(ItemStat.ColdResistance));
            AddToMod(ItemStat.TotalResistance, GetStat(ItemStat.ElementalResistance) + GetStat(ItemStat.TotalResistance));
        }

        private void ParseSockets()
        {
        }

        private void AddToMod(ItemStat stat, float value)
        {
            stats[(int) stat] += value;
        }

        public float GetStat(ItemStat stat)
        {
            return stats[(int) stat];
        }

        [Obsolete]
        public ItemType GetSlot()
        {
            return ItemType.All;
        }

        private class StatTranslator
        {
            private readonly Dictionary<string, AddStat> dict;

            public StatTranslator()
            {
                dict = new Dictionary<string, AddStat>();
                dict.Add("Dexterity", Single(ItemStat.Dexterity));
                dict.Add("Strength", Single(ItemStat.Strength));
                dict.Add("Intelligence", Single(ItemStat.Intelligence));
                dict.Add("IncreasedMana", Single(ItemStat.AddedMana));
                dict.Add("IncreasedLife", Single(ItemStat.AddedHP));
                dict.Add("IncreasedEnergyShield", Single(ItemStat.AddedES));
                dict.Add("IncreasedEnergyShieldPercent", Single(ItemStat.AddedESPercent));
                dict.Add("ColdResist", Single(ItemStat.ColdResistance));
                dict.Add("FireResist", Single(ItemStat.FireResistance));
                dict.Add("LightningResist", Single(ItemStat.LightningResistance));
                dict.Add("ChaosResist", Single(ItemStat.ChaosResistance));
                dict.Add("AllResistances", MultipleSame(new[]
                {
                    ItemStat.ColdResistance,
                    ItemStat.FireResistance,
                    ItemStat.LightningResistance
                }));
                dict.Add("CriticalStrikeChance", Single(ItemStat.CritChance));
                dict.Add("LocalCriticalMultiplier", Single(ItemStat.CritMultiplier));
                dict.Add("MovementVelocity", Single(ItemStat.MovementSpeed));
                dict.Add("ItemFoundRarityIncrease", Single(ItemStat.Rarity));
                dict.Add("ItemFoundQuantityIncrease", Single(ItemStat.Quantity));
                dict.Add("ManaLeech", Single(ItemStat.ManaLeech));
                dict.Add("LifeLeech", Single(ItemStat.LifeLeech));
                dict.Add("AddedLightningDamage", Average(ItemStat.AddedLightningDamage));
                dict.Add("AddedColdDamage", Average(ItemStat.AddedColdDamage));
                dict.Add("AddedFireDamage", Average(ItemStat.AddedFireDamage));
                dict.Add("AddedPhysicalDamage", Average(ItemStat.AddedPhysicalDamage));
                dict.Add("WeaponElementalDamage", Single(ItemStat.WeaponElementalDamagePercent));
                dict.Add("FireDamagePercent", Single(ItemStat.FireDamagePercent));
                dict.Add("ColdDamagePercent", Single(ItemStat.ColdDamagePercent));
                dict.Add("LightningDamagePercent", Single(ItemStat.LightningDamagePercent));
                dict.Add("SpellDamage", Single(ItemStat.SpellDamage));
                dict.Add("SpellDamageAndMana", Dual(ItemStat.SpellDamage, ItemStat.AddedMana));
                dict.Add("SpellCriticalStrikeChance", Single(ItemStat.SpellCriticalChance));
                dict.Add("IncreasedCastSpeed", Single(ItemStat.CastSpeed));
                dict.Add("ProjectileSpeed", Single(ItemStat.ProjectileSpeed));
                dict.Add("LocalIncreaseSocketedMinionGemLevel", Single(ItemStat.MinionSkillLevel));
                dict.Add("LocalIncreaseSocketedFireGemLevel", Single(ItemStat.FireSkillLevel));
                dict.Add("LocalIncreaseSocketedColdGemLevel", Single(ItemStat.ColdSkillLevel));
                dict.Add("LocalIncreaseSocketedLightningGemLevel", Single(ItemStat.LightningSkillLevel));
                dict.Add("LocalAddedPhysicalDamage", Average(ItemStat.LocalPhysicalDamage));
                dict.Add("LocalIncreasedPhysicalDamagePercent", Single(ItemStat.LocalPhysicalDamagePercent));
                dict.Add("LocalAddedColdDamage", Average(ItemStat.LocalAddedColdDamage));
                dict.Add("LocalAddedFireDamage", Average(ItemStat.LocalAddedFireDamage));
                dict.Add("LocalAddedLightningDamage", Average(ItemStat.LocalAddedLightningDamage));
                dict.Add("LocalCriticalStrikeChance", Single(ItemStat.LocalCritChance));
                dict.Add("LocalIncreasedAttackSpeed", Single(ItemStat.LocalAttackSpeed));
                dict.Add("LocalIncreasedEnergyShield", Single(ItemStat.LocalES));
                dict.Add("LocalIncreasedEvasionRating", Single(ItemStat.LocalEV));
                dict.Add("LocalIncreasedPhysicalDamageReductionRating", Single(ItemStat.LocalArmor));
                dict.Add("LocalIncreasedEvasionRatingPercent", Single(ItemStat.LocalEVPercent));
                dict.Add("LocalIncreasedEnergyShieldPercent", Single(ItemStat.LocalESPercent));
                dict.Add("LocalIncreasedPhysicalDamageReductionRatingPercent", Single(ItemStat.LocalArmorPercent));
                dict.Add("LocalIncreasedArmourAndEvasion", MultipleSame(new[]
                {
                    ItemStat.LocalArmorPercent,
                    ItemStat.LocalEVPercent
                }));
                dict.Add("LocalIncreasedArmourAndEnergyShield", MultipleSame(new[]
                {
                    ItemStat.LocalArmorPercent,
                    ItemStat.LocalESPercent
                }));
                dict.Add("LocalIncreasedEvasionAndEnergyShield", MultipleSame(new[]
                {
                    ItemStat.LocalEVPercent,
                    ItemStat.LocalESPercent
                }));
            }

            public void Translate(ItemStats stats, ItemMod m)
            {
                if (!dict.ContainsKey(m.Name))
                {
                    return;
                }
                dict[m.Name](stats, m);
            }

            private AddStat Single(ItemStat stat)
            {
                return delegate(ItemStats x, ItemMod m) { x.AddToMod(stat, m.Value1); };
            }

            private AddStat Average(ItemStat stat)
            {
                return delegate(ItemStats x, ItemMod m) { x.AddToMod(stat, (m.Value1 + m.Value2)/2f); };
            }

            private AddStat Dual(ItemStat s1, ItemStat s2)
            {
                return delegate(ItemStats x, ItemMod m)
                {
                    x.AddToMod(s1, m.Value1);
                    x.AddToMod(s2, m.Value2);
                };
            }

            private AddStat MultipleSame(params ItemStat[] stats)
            {
                return delegate(ItemStats x, ItemMod m)
                {
                    ItemStat[] stats2 = stats;
                    for (int i = 0; i < stats2.Length; i++)
                    {
                        ItemStat stat = stats2[i];
                        x.AddToMod(stat, m.Value1);
                    }
                };
            }

            private delegate void AddStat(ItemStats stats, ItemMod m);
        }
    }
}