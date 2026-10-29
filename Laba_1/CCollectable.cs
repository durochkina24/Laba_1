using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Laba_1
{     // общий родитель
    public abstract class CCollectable
    {
        protected Point position;
        protected Grid spriteContainer;
        protected double lifetime;

        public UIElement Sprite => spriteContainer;

        public CCollectable(Point pos, double size, string label)
        {
            position = pos;
            lifetime = 6.0; // Живет 6 секунд

            // Создаем контейнер
            spriteContainer = new Grid
            {
                Width = size,
                Height = size,
                RenderTransform = new TranslateTransform(pos.X, pos.Y),
                Cursor = System.Windows.Input.Cursors.Hand
            };

            // Сам кружок
            Ellipse circle = new Ellipse
            {
                Width = size,
                Height = size,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            spriteContainer.Children.Add(circle);

            // Текст внутри кружка
            TextBlock text = new TextBlock
            {
                Text = label,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                Foreground = Brushes.Black,
                IsHitTestVisible = false // Чтобы клик проходил сквозь текст на кружок
            };
            spriteContainer.Children.Add(text);

            // Задаем цвет наследникам
            if (this is CGoldGiver) circle.Fill = Brushes.Gold;
            else if (this is CDamageBooster) circle.Fill = Brushes.Red;
            else if (this is CCooldownReducer) circle.Fill = Brushes.Cyan;
        }

        public bool Update(double delta)
        {
            lifetime -= delta;
            if (lifetime <= 0) return true;
            if (lifetime < 1.5) spriteContainer.Opacity = lifetime % 0.4 < 0.2 ? 0.2 : 1.0;
            return false;
        }

        public abstract void OnClick(Player player);
    }

    public class CGoldGiver : CCollectable
    {
        public CGoldGiver(Point pos, double size) : base(pos, size, "Зол.") { }
        public override void OnClick(Player player) => player.AddGold(new BigNumber("500"));
    }

    public class CDamageBooster : CCollectable
    {
        public CDamageBooster(Point pos, double size) : base(pos, size, "Урон") { }
        public override void OnClick(Player player) => player.ApplyDamageBonus(3.0, 10.0); // x3 урона на 10 сек
    }

    public class CCooldownReducer : CCollectable
    {
        public CCooldownReducer(Point pos, double size) : base(pos, size, "КД") { }
        public override void OnClick(Player player) => player.ApplyCooldownBonus(0.3); // Сбрасывает 0.3 сек
    }
}