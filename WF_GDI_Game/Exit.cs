using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WF_GDI_Game
{
    class Exit
    {
        public PointF Position { set; get; }
        public int size = 40;

        public Exit(float x, float y)
        {
            Position = new PointF(x, y);
        }

        public void Draw(Graphics gr)
        {
            gr.FillRectangle(new SolidBrush(Color.FromArgb(50,Color.Red)), Position.X - size / 2, Position.Y - size / 2, size, size);
        }
    }
}
