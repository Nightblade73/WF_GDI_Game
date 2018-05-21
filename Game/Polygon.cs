using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    class Polygon
    {
        public List<Segment> segments = new List<Segment>();

        public Polygon(string[] points)
        {
            //  string[] point = points.Split(';');
            for (int i = 0; i < points.Length - 1; i++)
            {
                //if (i == points.Length-1)
                //{
                //    Segment lastSeg = new Segment(new PointF(Convert.ToInt32(points[i].Split(',')[0]), Convert.ToInt32(points[i].Split(',')[1])),
                //    new Point(Convert.ToInt32(points[0].Split(',')[0]), Convert.ToInt32(points[0].Split(',')[1])));
                //    segments.Add(lastSeg);
                //    break;
                //}
                Segment segment = new Segment(new PointF(Convert.ToInt32(points[i].Split(',')[0]), Convert.ToInt32(points[i].Split(',')[1])),
                    new PointF(Convert.ToInt32(points[i + 1].Split(',')[0]), Convert.ToInt32(points[i + 1].Split(',')[1])));
                segments.Add(segment);
            }
        }

        public void Draw(RenderTarget renderTarget)
        {
            using (var brush = new SolidColorBrush(renderTarget, SharpDX.Color.White))
            {
                foreach (Segment segment in segments)
                {
                    renderTarget.DrawLine(new Vector2(segment.Begin.X, segment.Begin.Y), new Vector2(segment.End.X, segment.End.Y), brush);
                //    renderTarget.FillRectangle(new RawRectangleF(Position.X - size / 2, Position.Y - size / 2, Position.X + size / 2, Position.Y + size / 2), brush);
                }
             //   renderTarget.DrawLine(new Pen(Color.FromArgb(255, 255, 255)), segment.Begin, segment.End);
            }
        }
    }
}
