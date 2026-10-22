using System;

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

        // --- НОВОЕ ДЛЯ ЛАБЫ 3 ---
        private CCountdownTimer cooldownTimer;
        private BigNumber cooldownUpgradeCost;
        private double damageBonusMultiplier = 1.0;
        private double damageBonusTimeLeft = 0;

        public int Lvl => lvl;
        public BigNumber Gold => gold;
        public BigNumber Damage => damage * damageBonusMultiplier;
        public BigNumber UpgradeCost => upgradeCost;
        public BigNumber CooldownUpgradeCost => cooldownUpgradeCost;
        public double CurrentCooldownTime => cooldownTimer.GetTargetTime();
        public bool CanClick => cooldownTimer.IsReady();
        public double DamageBonusTimeLeft => damageBonusTimeLeft;
        public double DamageBonusMultiplier => damageBonusMultiplier;

        public Player()
        {
            lvl = 1;
            gold = new BigNumber("0");
            damage = new BigNumber("5");
            damageModifier = 1.5;
            upgradeCost = new BigNumber("50");
            upgradeModifier = 1.25;

            cooldownTimer = new CCountdownTimer(2.0); // Стартовая перезарядка 2 сек
            cooldownUpgradeCost = new BigNumber("100");
        }

        public void Update(double delta)
        {
            cooldownTimer.Update(delta);
            if (damageBonusTimeLeft > 0)
            {
                damageBonusTimeLeft -= delta;
                if (damageBonusTimeLeft <= 0) damageBonusMultiplier = 1.0;
            }
        }

        public void Click() => cooldownTimer.Start();
        public void AddGold(BigNumber amount) => gold = gold + amount;

        public bool TrySpendGold(BigNumber amount)
        {
            if (gold < amount) return false;
            gold = gold - amount;
            return true;
        }

        public bool TryUpgrade()
        {
            BigNumber cost = upgradeCost;
            if (!TrySpendGold(cost)) return false;
            lvl++;
            damage = damage * damageModifier;
            upgradeCost = cost * upgradeModifier;
            return true;
        }

        public bool TryUpgradeCooldown()
        {
            if (!TrySpendGold(cooldownUpgradeCost)) return false;
            double newTime = Math.Max(0.5, CurrentCooldownTime * 0.9);
            cooldownTimer.ChangeTargetTime(newTime);
            cooldownUpgradeCost = cooldownUpgradeCost * 1.5;
            return true;
        }

        public void ApplyDamageBonus(double multiplier, double duration)
        {
            damageBonusMultiplier = multiplier;
            damageBonusTimeLeft = duration;
        }

        public void ApplyCooldownBonus(double seconds)
        {
            cooldownTimer.ChangeTargetTime(Math.Max(0.2, CurrentCooldownTime - seconds));
        }
    }
}