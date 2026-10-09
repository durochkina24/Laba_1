namespace Laba_1
{
    public class Enemy
    {
        private string name;
        private BigNumber maxHitPoints;
        private BigNumber currentHitPoints;
        private BigNumber goldReward;
        private bool isDead;
        private EnemyIcon icon;

        public string Name => name;
        public BigNumber MaxHitPoints => maxHitPoints;
        public BigNumber CurrentHitPoints => currentHitPoints;
        public BigNumber GoldReward => goldReward;
        public bool IsDead => isDead;
        public EnemyIcon Icon => icon;

        public Enemy(CEnemyTemplate template, EnemyIcon icon, int stageLevel)
        {
            name = template.Name;
            this.icon = icon;

            double scaleLife = System.Math.Pow(template.LifeModifier, stageLevel - 1);
            double scaleGold = System.Math.Pow(template.GoldModifier, stageLevel - 1);

            maxHitPoints = new BigNumber(template.BaseLife.ToString()) * scaleLife;
            currentHitPoints = maxHitPoints;
            goldReward = new BigNumber(template.BaseGold.ToString()) * scaleGold;

            isDead = false;
        }

        public bool TakeDamage(BigNumber dmg, out BigNumber reward)
        {
            reward = BigNumber.Zero;
            if (isDead) return false;

            if (dmg >= currentHitPoints)
            {
                currentHitPoints = BigNumber.Zero;
                isDead = true;
                reward = goldReward;
                return true;
            }
            currentHitPoints = currentHitPoints - dmg;
            return false;
        }
    }
}