using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Laba_1
{
    public partial class GameWindow : Window
    {
        private CEnemyTemplateList templates;
        private List<EnemyIcon> icons;
        private Player player;
        private IEnemy currentEnemy;
        private Random rng = new Random();
        private int stageLevel = 1;

        private DispatcherTimer gameTimer;
        private List<CCollectable> activeCollectables = new List<CCollectable>();
        private double spawnTimer = 0;
        private double spawnInterval = 3.0;

        public GameWindow(CEnemyTemplateList templates, List<EnemyIcon> icons)
        {
            InitializeComponent();
            this.templates = templates;
            this.icons = icons;
            player = new Player();

            templates.NormalizeChances();
            SpawnNextEnemy();
            UpdateUI();

            EnemyImage.MouseLeftButtonDown += EnemyImage_MouseLeftButtonDown;

            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromMilliseconds(16);
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            double delta = 0.016;
            player.Update(delta);

            for (int i = activeCollectables.Count - 1; i >= 0; i--)
            {
                if (activeCollectables[i].Update(delta))
                {
                    GameCanvas.Children.Remove(activeCollectables[i].Sprite);
                    activeCollectables.RemoveAt(i);
                }
            }

            spawnTimer += delta;
            if (spawnTimer >= spawnInterval && currentEnemy != null && !currentEnemy.IsDead)
            {
                spawnTimer = 0;
                SpawnRandomCollectable();
            }

            UpdateUI();
        }

        private void SpawnRandomCollectable()
        {
            double x = rng.Next(50, 250);
            double y = rng.Next(50, 250);
            Point pos = new Point(x, y);
            double size = 45;

            CCollectable sphere;
            int type = rng.Next(3);
            if (type == 0) sphere = new CGoldGiver(pos, size);
            else if (type == 1) sphere = new CDamageBooster(pos, size);
            else sphere = new CCooldownReducer(pos, size);

            sphere.Sprite.MouseDown += (s, ev) =>
            {
                sphere.OnClick(player);
                GameCanvas.Children.Remove(sphere.Sprite);
                activeCollectables.Remove(sphere);
                UpdateUI();
            };

            activeCollectables.Add(sphere);
            GameCanvas.Children.Add(sphere.Sprite);
        }

        private void EnemyImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (currentEnemy == null || currentEnemy.IsDead) return;
            if (!player.CanClick) return;

            BigNumber dmg = player.Damage;
            BigNumber reward;
            bool killed = currentEnemy.TakeDamage(dmg, out reward);

            if (currentEnemy is CHealingEnemy healer && healer.JustHealed)
            {
                MessageBox.Show($"{currentEnemy.Name} полечился! Урон не прошёл.");
            }

            player.Click();

            if (killed)
            {
                player.AddGold(reward);
                stageLevel++;
                GameCanvas.Children.Clear();
                activeCollectables.Clear();
                SpawnNextEnemy();
            }
            UpdateUI();
        }

        private void SpawnNextEnemy()
        {
            if (templates.Count == 0) return;
            CEnemyTemplate chosen = templates.FindByChance(rng.NextDouble());
            if (chosen == null) return;

            EnemyIcon icon = null;
            foreach (var i in icons) if (i.Name == chosen.IconName) { icon = i; break; }

            BigNumber hp = new BigNumber(chosen.BaseLife.ToString()) * Math.Pow(chosen.LifeModifier, stageLevel - 1);
            BigNumber gold = new BigNumber(chosen.BaseGold.ToString()) * Math.Pow(chosen.GoldModifier, stageLevel - 1);

            if (chosen is CArmoredEnemyTemplate armored)
            {
                currentEnemy = EnemyFactory.CreateEnemy("CArmoredEnemy", chosen.Name, hp, gold, armored.Armor);
            }
            else if (chosen is CHealingEnemyTemplate healing)
            {
                currentEnemy = EnemyFactory.CreateEnemy("CHealingEnemy", chosen.Name, hp, gold, healing.HealChance);
            }
            else
            {
                currentEnemy = EnemyFactory.CreateEnemy("CNormalEnemy", chosen.Name, hp, gold);
            }

            if (icon != null) EnemyImage.Source = new BitmapImage(new Uri(icon.ImagePath, UriKind.Absolute));
            else EnemyImage.Source = null;
        }

        private void UpgradeButton_Click(object sender, RoutedEventArgs e) { player.TryUpgrade(); UpdateUI(); }
        private void UpgradeCooldown_Click(object sender, RoutedEventArgs e) { player.TryUpgradeCooldown(); UpdateUI(); }

        private void UpdateUI()
        {
            if (currentEnemy != null)
            {
                EnemyNameText.Text = currentEnemy.Name;
                EnemyGoldText.Text = currentEnemy.GoldReward.ToString();

                double hpPercent = 0;
                if (!currentEnemy.MaxHitPoints.IsZero())
                {
                    double maxHp = double.Parse(currentEnemy.MaxHitPoints.ToString());
                    double curHp = double.Parse(currentEnemy.CurrentHitPoints.ToString());
                    hpPercent = (curHp / maxHp) * 100;
                }

                HpBarFill.Width = (300 * hpPercent) / 100;
                EnemyHpText.Text = $"{currentEnemy.CurrentHitPoints} / {currentEnemy.MaxHitPoints}";
            }

            LevelText.Text = player.Lvl.ToString();
            GoldText.Text = player.Gold.ToString();
            DamageText.Text = player.Damage.ToString();
            UpgradeCostText.Text = $"Цена: {player.UpgradeCost}";

            CooldownTimeText.Text = $"{player.CurrentCooldownTime:F2} сек.";
            CooldownCostText.Text = $"Цена: {player.CooldownUpgradeCost}";

            if (player.DamageBonusTimeLeft > 0)
                BonusStatusText.Text = $"Урон увеличен! ({player.DamageBonusTimeLeft:F1} сек.)";
            else
                BonusStatusText.Text = "";

            if (player.CanClick)
            {
                CooldownText.Text = "ГОТОВ К УДАРУ!";
                CooldownText.Foreground = Brushes.Green;
            }
            else
            {
                CooldownText.Text = "ПЕРЕЗАРЯДКА...";
                CooldownText.Foreground = Brushes.Red;
            }
        }
    }
}