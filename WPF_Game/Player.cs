using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using WPF_Game.Properties;

namespace WPF_Game
{
    class Player
    {
        public float X { set; get; }
        public float Y { set; get; }
        public int Size { set; get; }
        public int ВirectionOfSight { set; get; }
        public int speed = 1;

        public Player(float x, float y, int size, int birectionOfSight)
        {
            X = x;
            Y = y;
            Size = size;
            ВirectionOfSight = birectionOfSight;
        }

        public Shape Draw()
        {

            Ellipse ellipse = new Ellipse
            {
                Width = Size,
                Height = Size,
                Fill = Brushes.Red,
                Stroke = Brushes.Green,
                StrokeThickness = 2
            };

            Canvas.SetLeft(ellipse, X - Size / 2);
            Canvas.SetTop(ellipse, Y - Size / 2);
            return ellipse;

        }

        public void MoveUp(float cos, float sin)
        {
            Y -= speed;
        }

        public void MoveDown()
        {
            Y += speed;
        }

        public void MoveRight()
        {
            X += speed;
        }

        public void MoveLeft()
        {
            X -= speed;
        }
    }
}
