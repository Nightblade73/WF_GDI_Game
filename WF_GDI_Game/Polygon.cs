using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WF_GDI_Game
{
    class Polygon
    {
        public List<Segment> segments = new List<Segment>();

        public Polygon(string points)
        {
            string[] point = points.Split(';');
            for (int i = 0; i < point.Length; i++)
            {
                if (i == point.Length-1)
                {
                    Segment lastSeg = new Segment(new PointF(Convert.ToInt32(point[i].Split(',')[0]), Convert.ToInt32(point[i].Split(',')[1])),
                    new Point(Convert.ToInt32(point[0].Split(',')[0]), Convert.ToInt32(point[0].Split(',')[1])));
                    segments.Add(lastSeg);
                    break;
                }
                Segment segment = new Segment(new PointF(Convert.ToInt32(point[i].Split(',')[0]), Convert.ToInt32(point[i].Split(',')[1])),
                    new Point(Convert.ToInt32(point[i + 1].Split(',')[0]), Convert.ToInt32(point[i + 1].Split(',')[1])));
                
                segments.Add(segment);
            }
        }

        public void Draw(Graphics gr)
        {
            foreach (Segment segment in segments)
            {
                gr.DrawLine(new Pen(Color.FromArgb(255, 255, 255)), segment.Begin, segment.End);
            }
        }
    }
}
