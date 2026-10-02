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
        
    }
}