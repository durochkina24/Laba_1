using System;

namespace Laba_1
{
    public interface IEnemy
    {
        string Name { get; }
        BigNumber MaxHitPoints { get; }
        BigNumber CurrentHitPoints { get; }
        BigNumber GoldReward { get; }
        bool IsDead { get; }
        bool TakeDamage(BigNumber damage, out BigNumber reward);
    }

    public abstract class CEnemy : IEnemy
    {
        public string Name { get; protected set; }
        public BigNumber MaxHitPoints { get; protected set; }
        public BigNumber CurrentHitPoints { get; protected set; }
        public BigNumber GoldReward { get; protected set; }
        public bool IsDead { get; protected set; }

        protected CEnemy(string name, BigNumber baseLife, BigNumber gold)
        {
            Name = name;
            MaxHitPoints = baseLife;
            CurrentHitPoints = baseLife;
            GoldReward = gold;
            IsDead = false;
        }

        public virtual bool TakeDamage(BigNumber damage, out BigNumber reward)
        {
            reward = BigNumber.Zero;
            if (IsDead) return false;

            if (damage >= CurrentHitPoints)
            {
                CurrentHitPoints = BigNumber.Zero;
                IsDead = true;
                reward = GoldReward;
                return true;
            }
            CurrentHitPoints = CurrentHitPoints - damage;
            return false;
        }
    }

    public class CNormalEnemy : CEnemy
    {
        public CNormalEnemy(string name, BigNumber baseLife, BigNumber gold) : base(name, baseLife, gold) { }
    }

    public class CArmoredEnemy : CEnemy
    {
        private double armorPercent;
        public CArmoredEnemy(string name, BigNumber baseLife, BigNumber gold, double armorPercent) : base(name, baseLife, gold)
        {
            this.armorPercent = armorPercent;
        }

        public override bool TakeDamage(BigNumber damage, out BigNumber reward)
        {
            BigNumber reducedDamage = damage * (1.0 - armorPercent);
            return base.TakeDamage(reducedDamage, out reward);
        }
    }

    public class CHealingEnemy : CEnemy
    {
        private double healChance;
        private Random rng = new Random();
        public bool JustHealed { get; private set; } = false;

        public CHealingEnemy(string name, BigNumber baseLife, BigNumber gold, double healChance) : base(name, baseLife, gold)
        {
            this.healChance = healChance;
        }

        public override bool TakeDamage(BigNumber damage, out BigNumber reward)
        {
            JustHealed = false;

            if (!IsDead && rng.NextDouble() < healChance)
            {
                BigNumber healAmount = MaxHitPoints * 0.2;
                CurrentHitPoints = CurrentHitPoints + healAmount;

                if (CurrentHitPoints > MaxHitPoints)
                    CurrentHitPoints = MaxHitPoints;

                JustHealed = true;
                reward = BigNumber.Zero;
                return false;
            }

            return base.TakeDamage(damage, out reward);
        }
    }
}