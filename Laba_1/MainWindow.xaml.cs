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
            var dialog = new OpenFolderDialog();
            if (dialog.ShowDialog() == true)
            {
                LoadIconsFromFolder(dialog.FolderName);
            }
        }

        public void LoadIconsFromFolder(string path)
        {
            string filter = "*.png";
            string[] files = Directory.GetFiles(path, filter);

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

                // Отображение в ListBox
                Image image = new Image
                {
                    Source = new BitmapImage(new Uri(icon.ImagePath)),
                    Height = 64
                };
                IconsListBox.Items.Add(image);
            }
        }

        // 2. Обработка выбора иконки
        private void IconsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox iconHolder = sender as ListBox;
            if (iconHolder.SelectedItem is Image selectedImage && iconHolder.SelectedItem != null)
            {
                // Получение имени файла из пути
                string iconName = Path.GetFileName(selectedImage.Source.ToString());
                IconNameTextBox.Text = iconName;

                // Отображение главной иконки
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
    }
}