using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OGL_Game
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
                Segment segment = new Segment(new PointF(Convert.ToInt32(points[i].Split(',')[0]), Convert.ToInt32(points[i].Split(',')[1])),
                    new Point(Convert.ToInt32(points[i + 1].Split(',')[0]), Convert.ToInt32(points[i + 1].Split(',')[1])));
                segments.Add(segment);
            }
        }

        public void Draw(OpenGL gl)
        {
            List<float> p = new List<float>();
            foreach (Segment segment in segments)
            {
                gl.Begin(OpenGL.GL_LINES);
                gl.Color(1.0f, 1.0f, 1.0f);
                gl.Vertex(segment.Begin.X, segment.Begin.Y);
                gl.Vertex(segment.End.X, segment.End.Y);
                gl.End();
            }
            
        }
    }
}
