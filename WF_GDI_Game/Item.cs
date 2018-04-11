using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WF_GDI_Game
{
    class Item
    {
        public PointF Position { set; get; }

        public Item(float x, float y)
        {
            Position = new PointF(x, y);
        }

        public void Draw(Graphics gr)
        {
            gr.FillRectangle(new SolidBrush(Color.Yellow), Position.X, Position.Y, 20, 20);

        }
    }
}
