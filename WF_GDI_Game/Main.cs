using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WF_GDI_Game
{
    public partial class Main : Form
    {
        List<Polygon> polygons;
        Player player;
        //   Ray ray;
        List<Ray> rays;
        Bitmap bmp;

        public Main()
        {
            InitializeComponent();
            player = new Player(20, 20, 20);
            rays = new List<Ray>();
            //      ray.Begin = new Point(300, 230);

            polygons = new List<Polygon>();

            for (double angle = 0; angle < Math.PI * 2; angle += (Math.PI * 2) / 50)
            {

                // Calculate dx & dy from angle
                int dx = Convert.ToInt32(Math.Cos(angle) * 10);
                int dy = Convert.ToInt32(Math.Sin(angle) * 10);

                // Ray from center of screen to mouse
                Ray ray = new Ray();
                ray.Begin = new Point(20, 20);
                ray.Mouse = new Point(20 + dx, 20 + dy);
                rays.Add(ray);
            }

            pictureBox.Width = SystemInformation.PrimaryMonitorSize.Width;
            pictureBox.Height = SystemInformation.PrimaryMonitorSize.Height;
            pictureBox.Location = new Point(0, 0);

            //         pictureBox.Invalidate();
        }

        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 115)
            {
                Close();
            }
            if (e.KeyValue == 37)
            {
                player.MoveLeft();
                for (int i = 0; i < rays.Count; i++)
                    rays[i].MoveLeft();

            }
            if (e.KeyValue == 38)
            {
                player.MoveUp();
                for (int i = 0; i < rays.Count; i++)
                    rays[i].MoveUp();
            }
            if (e.KeyValue == 39)
            {
                player.MoveRight();
                for (int i = 0; i < rays.Count; i++)
                    rays[i].MoveRight();
            }
            if (e.KeyValue == 40)
            {
                player.MoveDown();
                for (int i = 0; i < rays.Count; i++)
                    rays[i].MoveDown();
            }
        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (bmp != null)
                bmp.Dispose();
            bmp = new Bitmap(pictureBox.Width, pictureBox.Height);
            using (Graphics gr = Graphics.FromImage(bmp))
            {
                player.Draw(gr);
                foreach (Polygon pol in polygons)
                {
                    pol.Draw(gr);
                }
                foreach (Ray ray in rays)
                {
                    NewRay(ray);
                    ray.Draw(gr);
                }
            }
            pictureBox.Image = bmp;
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        { 
            //foreach (Ray ray in rays)
            //{
            //    ray.Begin.X = e.X;
            //    ray.Begin.Y = e.Y;
            //}

        }

        private void timer_Tick(object sender, EventArgs e)
        {
            //  pictureBox.Invalidate();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            using (StreamReader sr = new StreamReader("polygons.txt"))
            {
                while (!sr.EndOfStream)
                {
                    Polygon polygon = new Polygon(sr.ReadLine());
                    polygons.Add(polygon);
                }

            }
        }
        private void NewRay(Ray ray)
        {
            ParamPoint closestIntersect = null;
            for (int i = 0; i < polygons.Count; i++)
            {
                for (int j = 0; j < polygons[i].segments.Count; j++)
                {
                    ParamPoint intersect = Intersection.GetIntersection(ray, polygons[i].segments[j]);
                    if (intersect != null)
                    {
                        if (closestIntersect != null)
                        {
                            if (intersect.T1 < closestIntersect.T1)
                            {
                                closestIntersect = intersect;
                            }
                        }
                        else
                        {
                            closestIntersect = intersect;
                        }
                    }
                }
            }
            if (closestIntersect != null)
                ray.Mouse = new Point(closestIntersect.Intersection.X, closestIntersect.Intersection.Y);
        }
        
    }
}
