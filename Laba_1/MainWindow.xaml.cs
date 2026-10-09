using Laba_1;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

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

        // 1. Загрузка иконок из папки
        private void LoadIconsButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFolderDialog();
            dialog.Title = "Выберите папку с иконками";

            if (dialog.ShowDialog() == true)
            {
                LoadIconsFromFolder(dialog.FolderName);
            }
        }

        public void LoadIconsFromFolder(string path)
        {
            string[] files = Directory.GetFiles(path, "*.png");

            enemyIcons.Clear();
            IconsListBox.Items.Clear();

            foreach (string file in files)
            {
                EnemyIcon icon = new EnemyIcon
                {
                    Name = Path.GetFileName(file),
                    ImagePath = file
                };
                enemyIcons.Add(icon);

                Image image = new Image
                {
                    Source = new BitmapImage(new Uri(icon.ImagePath, UriKind.Absolute)),
                    Height = 64
                };
                IconsListBox.Items.Add(image);
            }

            if (files.Length == 0)
            {
                MessageBox.Show("В выбранной папке нет .png файлов");
            }
        }

        // 2. Обработка выбора иконки
        private void IconsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IconsListBox.SelectedItem is Image selectedImage)
            {
                string iconName = Path.GetFileName(selectedImage.Source.ToString());
                IconNameTextBox.Text = iconName;
                MainEnemyIcon.Source = selectedImage.Source;
            }
        }

        // 3. Добавление противника
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                enemyList.AddEnemy(
                    NameTextBox.Text,
                    IconNameTextBox.Text,
                    int.Parse(BaseLifeTextBox.Text),
                    double.Parse(LifeModifierTextBox.Text),
                    int.Parse(BaseGoldTextBox.Text),
                    double.Parse(GoldModifierTextBox.Text),
                    double.Parse(SpawnChanceTextBox.Text)
                );
                UpdateEnemiesList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Проверьте правильность ввода данных: " + ex.Message);
            }
        }

        // 3.5. Обновление (редактирование) противника
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (EnemiesListBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите противника из списка");
                return;
            }

            string oldName = EnemiesListBox.SelectedItem.ToString();
            CEnemyTemplate enemy = enemyList.GetEnemyByName(oldName);

            if (enemy == null)
            {
                MessageBox.Show("Противник не найден");
                return;
            }

            try
            {
                // Если имя изменилось — удаляем старого и добавляем нового
                if (enemy.Name != NameTextBox.Text)
                {
                    enemyList.DeleteEnemyByName(oldName);
                    enemyList.AddEnemy(
                        NameTextBox.Text,
                        IconNameTextBox.Text,
                        int.Parse(BaseLifeTextBox.Text),
                        double.Parse(LifeModifierTextBox.Text),
                        int.Parse(BaseGoldTextBox.Text),
                        double.Parse(GoldModifierTextBox.Text),
                        double.Parse(SpawnChanceTextBox.Text)
                    );
                }
                else
                {
                    // Имя не менялось — обновляем через метод Update
                    enemy.Update(
                        NameTextBox.Text,
                        IconNameTextBox.Text,
                        int.Parse(BaseLifeTextBox.Text),
                        double.Parse(LifeModifierTextBox.Text),
                        int.Parse(BaseGoldTextBox.Text),
                        double.Parse(GoldModifierTextBox.Text),
                        double.Parse(SpawnChanceTextBox.Text)
                    );
                }

                UpdateEnemiesList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Проверьте правильность ввода данных: " + ex.Message);
            }
        }

        // 4. Удаление противника
        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (EnemiesListBox.SelectedItem != null)
            {
                string selectedName = EnemiesListBox.SelectedItem.ToString();
                enemyList.DeleteEnemyByName(selectedName);
                UpdateEnemiesList();
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

        // 5. Сохранение в JSON
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "JSON files (*.json)|*.json";
            if (dlg.ShowDialog() == true)
            {
                enemyList.SaveToJson(dlg.FileName);
            }
        }

        // 6. Загрузка из JSON
        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "JSON files (*.json)|*.json";
            if (dlg.ShowDialog() == true)
            {
                enemyList.LoadFromJson(dlg.FileName);
                UpdateEnemiesList();
            }
        }

        // Обработчик выбора противника в левом списке
        private void EnemiesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EnemiesListBox.SelectedItem != null)
            {
                string selectedName = EnemiesListBox.SelectedItem.ToString();
                CEnemyTemplate enemy = enemyList.GetEnemyByName(selectedName);

                if (enemy != null)
                {
                    NameTextBox.Text = enemy.Name;
                    IconNameTextBox.Text = enemy.IconName;
                    BaseLifeTextBox.Text = enemy.BaseLife.ToString();
                    LifeModifierTextBox.Text = enemy.LifeModifier.ToString();
                    BaseGoldTextBox.Text = enemy.BaseGold.ToString();
                    GoldModifierTextBox.Text = enemy.GoldModifier.ToString();
                    SpawnChanceTextBox.Text = enemy.SpawnChance.ToString();
                }
            }
        }

        // 7. Кнопка "Играть" — открывает окно кликера
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (enemyList.Count == 0)
            {
                MessageBox.Show("Сначала добавьте хотя бы одного противника.");
                return;
            }

            GameWindow game = new GameWindow(enemyList, enemyIcons);
            game.Show();
        }
    }
}