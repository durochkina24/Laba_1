using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Laba_1
{
    public partial class GameWindow : Window
    {
        private CEnemyTemplateList templates;
        private List<EnemyIcon> icons;
        private Player player;
        private Enemy currentEnemy;
        private System.Random rng = new System.Random();
        private int stageLevel = 1;

        public GameWindow(CEnemyTemplateList templates, List<EnemyIcon> icons)
        {
            InitializeComponent();
            this.templates = templates;
            this.icons = icons;
            player = new Player();

            templates.NormalizeChances();
            SpawnNextEnemy();
            UpdateUI();
        }

        private EnemyIcon FindIconByName(string name)
        {
            foreach (var i in icons) if (i.Name == name) return i;
            return null;
        }

        private void SpawnNextEnemy()
        {
            if (templates.Count == 0)
            {
                MessageBox.Show("Список противников пуст. Добавьте хотя бы одного в редакторе.");
                Close();
                return;
            }

            double roll = rng.NextDouble();
            CEnemyTemplate chosen = templates.FindByChance(roll);
            if (chosen == null) return;

            EnemyIcon icon = FindIconByName(chosen.IconName);
            currentEnemy = new Enemy(chosen, icon, stageLevel);

            if (icon != null)
            {
                EnemyImage.Source = new BitmapImage(
                    new System.Uri(icon.ImagePath, System.UriKind.Absolute));
            }
            else
            {
                EnemyImage.Source = null;
            }
        }

        private void EnemyImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (currentEnemy == null || currentEnemy.IsDead) return;

            BigNumber dmg = player.DealDamage();
            BigNumber reward;
            bool killed = currentEnemy.TakeDamage(dmg, out reward);

            if (killed)
            {
                player.AddGold(reward);
                stageLevel++;
                SpawnNextEnemy();
            }
            UpdateUI();
        }

        private void UpgradeButton_Click(object sender, RoutedEventArgs e)
        {
            player.TryUpgrade();
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (currentEnemy != null)
            {
                EnemyNameText.Text = currentEnemy.Name;
                EnemyHpText.Text = currentEnemy.CurrentHitPoints + " / " + currentEnemy.MaxHitPoints;
                EnemyGoldText.Text = currentEnemy.GoldReward.ToString();
            }

            LevelText.Text = player.Lvl.ToString();
            GoldText.Text = player.Gold.ToString();
            DamageText.Text = player.Damage.ToString();
            UpgradeCostText.Text = player.UpgradeCost.ToString();
        }
    }
}