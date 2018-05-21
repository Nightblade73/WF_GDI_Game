using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    class Exit
    {
        public PointF Position { set; get; }
        public int size = 40;

        public Exit(float x, float y)
        {
            Position = new PointF(x, y);
        }

        public void Draw(RenderTarget renderTarget)
        {
            BrushProperties bp = new BrushProperties();
            bp.Opacity = 0.5f;
            using (var brush = new SolidColorBrush(renderTarget, SharpDX.Color.Red, bp))
            {
                renderTarget.FillRectangle(new RawRectangleF(Position.X - size / 2, Position.Y - size / 2, Position.X + size / 2, Position.Y + size / 2), brush);
            }
        }
    }
}
