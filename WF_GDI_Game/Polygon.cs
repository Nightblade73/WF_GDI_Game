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
        List<Point> points = new List<Point>();


        public Polygon(string points)
        {
            string[] point = points.Split(';');
            for (int i = 0; i < point.Length; i++)
            {
                this.points.Add(new Point(Convert.ToInt32(point[i].Split(',')[0]), Convert.ToInt32(point[i].Split(',')[1])));
            }
        }
        public void Draw(Graphics gr)
        {
            
            gr.FillPolygon(new SolidBrush(Color.FromArgb(170, 170, 170)), points.ToArray());
        }


    }
}
