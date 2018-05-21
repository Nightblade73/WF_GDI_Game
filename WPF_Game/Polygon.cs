using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WPF_Game
{
    class Polygon
    {
        public List<Segment> segments = new List<Segment>();

        public Polygon(string[] points)
        {
          //  string[] point = points.Split(';');
            for (int i = 0; i < points.Length-1; i++)
            {
                //if (i == points.Length-1)
                //{
                //    Segment lastSeg = new Segment(new PointF(Convert.ToInt32(points[i].Split(',')[0]), Convert.ToInt32(points[i].Split(',')[1])),
                //    new Point(Convert.ToInt32(points[0].Split(',')[0]), Convert.ToInt32(points[0].Split(',')[1])));
                //    segments.Add(lastSeg);
                //    break;
                //}
                Segment segment = new Segment(new Point(Convert.ToInt32(points[i].Split(',')[0]), Convert.ToInt32(points[i].Split(',')[1])),
                    new Point(Convert.ToInt32(points[i + 1].Split(',')[0]), Convert.ToInt32(points[i + 1].Split(',')[1])));
                segments.Add(segment);
            }
        }

        public Shape Draw()
        {
            List<Point> list = new List<Point>();
            foreach (Segment segment in segments)
            {
                list.Add(segment.Begin);
            }
            System.Windows.Shapes.Polygon pol = new System.Windows.Shapes.Polygon
            {
                Points = new PointCollection(list),
                Fill = System.Windows.Media.Brushes.Black,
                Stroke = Brushes.White,
                StrokeThickness = 1
            };
            return pol;
        }
    }
}
