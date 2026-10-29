using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace Laba_1
{
    public partial class MainWindow : Window
    {
        private CEnemyTemplateList enemyList = new CEnemyTemplateList();
        private List<EnemyIcon> enemyIcons = new List<EnemyIcon>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadIconsButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog();
            if (dialog.ShowDialog() == true)
            {
                string[] files = System.IO.Directory.GetFiles(dialog.FolderName, "*.png");
                enemyIcons.Clear();
                IconsListBox.Items.Clear();

                foreach (string file in files)
                {
                    EnemyIcon icon = new EnemyIcon { Name = System.IO.Path.GetFileName(file), ImagePath = file };
                    enemyIcons.Add(icon);
                    Image image = new Image { Source = new BitmapImage(new Uri(icon.ImagePath)), Height = 64 };
                    IconsListBox.Items.Add(image);
                }
            }
        }

        private void IconsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IconsListBox.SelectedItem is Image selectedImage)
            {
                string iconName = System.IO.Path.GetFileName(selectedImage.Source.ToString());
                IconNameTextBox.Text = iconName;
                MainEnemyIcon.Source = selectedImage.Source;
            }
        }

        private void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ArmorLabel == null || ArmorTextBox == null) return;

            string selected = (TypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (selected == "Бронированный")
            {
                ArmorLabel.Visibility = Visibility.Visible;
                ArmorTextBox.Visibility = Visibility.Visible;
            }
            else
            {
                ArmorLabel.Visibility = Visibility.Collapsed;
                ArmorTextBox.Visibility = Visibility.Collapsed;
            }
        }

        // Вспомогательный метод для безопасного парсинга чисел (и точка, и запятая)
        private double ParseDouble(string text)
        {
            return double.Parse(text.Replace('.', ','));
        }

        private CEnemyTemplate CreateEnemyFromInputs()
        {
            string name = NameTextBox.Text;
            string icon = IconNameTextBox.Text;
            int life = int.Parse(BaseLifeTextBox.Text);
            double lifeMod = ParseDouble(LifeModifierTextBox.Text);
            int gold = int.Parse(BaseGoldTextBox.Text);
            double goldMod = ParseDouble(GoldModifierTextBox.Text);
            double chance = ParseDouble(SpawnChanceTextBox.Text);

            string type = (TypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (type == "Бронированный")
            {
                double armor = ParseDouble(ArmorTextBox.Text);
                return new CArmoredEnemyTemplate(name, icon, life, lifeMod, gold, goldMod, chance, armor);
            }
            else if (type == "Лечащийся")
            {
                // Для простоты зададим шанс лечения 30% (0.3)
                return new CHealingEnemyTemplate(name, icon, life, lifeMod, gold, goldMod, chance, 0.3);
            }
            else
            {
                return new CNormalEnemyTemplate(name, icon, life, lifeMod, gold, goldMod, chance);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                enemyList.AddEnemy(CreateEnemyFromInputs());
                UpdateEnemiesList();
                MessageBox.Show("Противник успешно добавлен!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Проверьте правильность ввода: " + ex.Message);
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (EnemiesListBox.SelectedItem == null)
            {
                MessageBox.Show("Сначала выберите противника из списка, которого хотите обновить!");
                return;
            }

            string oldName = EnemiesListBox.SelectedItem.ToString();

            try
            {
                enemyList.DeleteEnemyByName(oldName);
                enemyList.AddEnemy(CreateEnemyFromInputs());
                UpdateEnemiesList();
                MessageBox.Show("Противник успешно обновлен!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Проверьте правильность ввода: " + ex.Message);
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (EnemiesListBox.SelectedItem != null)
            {
                enemyList.DeleteEnemyByName(EnemiesListBox.SelectedItem.ToString());
                UpdateEnemiesList();
            }
            else
            {
                MessageBox.Show("Выберите противника для удаления.");
            }
        }

        private void UpdateEnemiesList()
        {
            EnemiesListBox.Items.Clear();
            foreach (string name in enemyList.GetListOfEnemyNames())
            {
                EnemiesListBox.Items.Add(name);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog { Filter = "JSON files (*.json)|*.json" };
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    enemyList.SaveToJson(dlg.FileName);
                    MessageBox.Show("Список успешно сохранен!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при сохранении: " + ex.Message);
                }
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog { Filter = "JSON files (*.json)|*.json" };
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    enemyList.LoadFromJson(dlg.FileName);
                    UpdateEnemiesList();
                    MessageBox.Show("Список успешно загружен!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке: " + ex.Message);
                }
            }
        }

        private void EnemiesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EnemiesListBox.SelectedItem != null)
            {
                CEnemyTemplate enemy = enemyList.GetEnemyByName(EnemiesListBox.SelectedItem.ToString());
                if (enemy != null)
                {
                    NameTextBox.Text = enemy.Name;
                    IconNameTextBox.Text = enemy.IconName;
                    BaseLifeTextBox.Text = enemy.BaseLife.ToString();
                    LifeModifierTextBox.Text = enemy.LifeModifier.ToString();
                    BaseGoldTextBox.Text = enemy.BaseGold.ToString();
                    GoldModifierTextBox.Text = enemy.GoldModifier.ToString();
                    SpawnChanceTextBox.Text = enemy.SpawnChance.ToString();

                    if (enemy is CArmoredEnemyTemplate armored)
                    {
                        TypeComboBox.SelectedIndex = 1;
                        ArmorTextBox.Text = armored.Armor.ToString();
                    }
                    else if (enemy is CHealingEnemyTemplate)
                    {
                        TypeComboBox.SelectedIndex = 2;
                    }
                    else
                    {
                        TypeComboBox.SelectedIndex = 0;
                    }
                }
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (enemyList.Count == 0)
            {
                MessageBox.Show("Сначала добавьте противника.");
                return;
            }
            new GameWindow(enemyList, enemyIcons).Show();
        }
    }
}