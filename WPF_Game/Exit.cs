using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WPF_Game
{
    class Exit
    {
        public System.Drawing.PointF Position { set; get; }
        public int size = 40;

        public Exit(float x, float y)
        {
            Position = new System.Drawing.PointF(x, y);
        }

        public Shape Draw()
        {
            Rectangle rect = new Rectangle
            {
                Width = size,
                Height = size,
                Fill = Brushes.Red,
                Opacity = 0.5
            };

            Canvas.SetLeft(rect, Position.X - size / 2);
            Canvas.SetTop(rect, Position.Y - size / 2);
            return rect;
        }
    }
}
