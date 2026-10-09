namespace Laba_1
{
    public class Player
    {
        private int lvl;
        private BigNumber gold;
        private BigNumber damage;
        private double damageModifier;
        private BigNumber upgradeCost;
        private double upgradeModifier;

        public int Lvl => lvl;
        public BigNumber Gold => gold;
        public BigNumber Damage => damage;
        public double DamageModifier => damageModifier;

        public BigNumber UpgradeCost => upgradeCost;

        public double UpgradeModifier => upgradeModifier;

        public Player()
        {
            lvl = 1;
            gold = new BigNumber("0");
            damage = new BigNumber("5");
            damageModifier = 1.5;                       
            upgradeCost = new BigNumber("50");         
            upgradeModifier = 1.25;                     
        }

        public void AddGold(BigNumber amount) => gold = gold + amount;

        private bool TrySpendGold(BigNumber amount)
        {
            if (gold < amount) return false;
            gold = gold - amount;
            return true;
        }

        private BigNumber CalculateTotalDamage()
            => damage * damageModifier;

        public bool TryUpgrade()
        {
            BigNumber cost = upgradeCost;
            if (!TrySpendGold(cost)) return false;

            lvl++;
            damage = CalculateTotalDamage();

            
            upgradeCost = cost * upgradeModifier;

            return true;
        }

        public BigNumber DealDamage() => damage;
    }
}