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
        public int size = 20;

        public Item(float x, float y)
        {
            Position = new PointF(x, y);
        }

        public void Draw(Graphics gr, float width)
        {
            gr.FillRectangle(new SolidBrush(Color.Yellow), Position.X-size/2, Position.Y - size / 2, size, size);
            gr.DrawRectangle(new Pen(Color.Green, width), Position.X - size / 2, Position.Y - size / 2, size, size);

        }
        public void Draw(Graphics gr)
        {
            gr.FillRectangle(new SolidBrush(Color.Yellow), Position.X - size / 2, Position.Y - size / 2, size, size);
        }
    }
}
