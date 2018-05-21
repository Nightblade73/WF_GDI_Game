using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace Game
{
    class Item
    {
        public PointF Position { set; get; }
        public int size = 20;

        public Item(float x, float y)
        {
            Position = new PointF(x, y);
        }

        public void Draw(RenderTarget renderTarget, float width)
        {
            using (var brush = new SolidColorBrush(renderTarget, SharpDX.Color.Red))
            {
                renderTarget.FillRectangle(new RawRectangleF(Position.X - size / 2 - width, Position.Y - size / 2 - width, Position.X + size / 2 + width, Position.Y + size / 2 + width), brush);
            }
            Draw(renderTarget);
        }
        public void Draw(RenderTarget renderTarget)
        {
            using (var brush = new SolidColorBrush(renderTarget, SharpDX.Color.Yellow))
            {
                renderTarget.FillRectangle(new RawRectangleF(Position.X - size / 2, Position.Y - size / 2, Position.X + size / 2, Position.Y + size / 2), brush);
            }
        }
    }
}
