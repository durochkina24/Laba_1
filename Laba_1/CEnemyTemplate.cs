using System.Text.Json.Serialization;

namespace Laba_1
{
    public abstract class CEnemyTemplate
    {
        public string Name { get; set; }
        public string IconName { get; set; }
        public int BaseLife { get; set; }
        public double LifeModifier { get; set; }
        public int BaseGold { get; set; }
        public double GoldModifier { get; set; }
        public double SpawnChance { get; set; }

        public CEnemyTemplate() { }

        protected CEnemyTemplate(string name, string iconName, int baseLife, double lifeModifier, int baseGold, double goldModifier, double spawnChance)
        {
            Name = name; IconName = iconName; BaseLife = baseLife; LifeModifier = lifeModifier;
            BaseGold = baseGold; GoldModifier = goldModifier; SpawnChance = spawnChance;
        }

       
        public void NormalizeChance(double sum) { if (sum > 0) SpawnChance /= sum; }
    }

    public class CNormalEnemyTemplate : CEnemyTemplate
    {
        public CNormalEnemyTemplate() { }
        public CNormalEnemyTemplate(string name, string iconName, int baseLife, double lifeModifier, int baseGold, double goldModifier, double spawnChance)
            : base(name, iconName, baseLife, lifeModifier, baseGold, goldModifier, spawnChance) { }
    }

    public class CArmoredEnemyTemplate : CEnemyTemplate
    {
        public double Armor { get; set; }
        public CArmoredEnemyTemplate() { }
        public CArmoredEnemyTemplate(string name, string iconName, int baseLife, double lifeModifier, int baseGold, double goldModifier, double spawnChance, double armor)
            : base(name, iconName, baseLife, lifeModifier, baseGold, goldModifier, spawnChance) { Armor = armor; }
    }

    public class CHealingEnemyTemplate : CEnemyTemplate
    {
        public double HealChance { get; set; }
        public CHealingEnemyTemplate() { }
        public CHealingEnemyTemplate(string name, string iconName, int baseLife, double lifeModifier, int baseGold, double goldModifier, double spawnChance, double healChance)
            : base(name, iconName, baseLife, lifeModifier, baseGold, goldModifier, spawnChance) { HealChance = healChance; }
    }
}