using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Game
{
    class Item
    {
        public System.Drawing.PointF Position { set; get; }
        public int size = 20;

        public Item(float x, float y)
        {
            Position = new System.Drawing.PointF(x, y);
        }

        public Shape Draw(float width)
        {
            Rectangle rect = new Rectangle
            {
                Width = size,
                Height = size,
                Fill = Brushes.Yellow,
                Stroke = Brushes.Red,
                StrokeThickness = width
            };

            Canvas.SetLeft(rect, Position.X - size / 2);
            Canvas.SetTop(rect, Position.Y - size / 2);
            return rect;

        }
        public Shape Draw()
        {
            Rectangle rect = new Rectangle
            {
                Width = size,
                Height = size,
                Fill = Brushes.Yellow
            };

            Canvas.SetLeft(rect, Position.X - size / 2);
            Canvas.SetTop(rect, Position.Y - size / 2);
            return rect;
        }
    }
}
