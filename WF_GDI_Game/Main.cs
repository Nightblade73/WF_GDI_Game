﻿using System;
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
        bool up, down, left, right;
        List<Polygon> polygons;
        Player player;
        //   Ray ray;
        List<Ray> rays;
        List<ParamPoint> intersects;
        List<UniquePoint> uniquePoints;
        List<float> uniqueAngles;
        Bitmap bmp;
        Ray mouseRay;
        public Main()
        {
            InitializeComponent();
            player = new Player(20, 20, 20);
            polygons = new List<Polygon>();
            uniqueAngles = new List<float>();
            intersects = new List<ParamPoint>();
            mouseRay = new Ray
            {
                Begin = new PointF(player.X, player.Y),
                Mouse = new PointF(Cursor.Position.X, Cursor.Position.Y)
            };
            pictureBox.Width = SystemInformation.PrimaryMonitorSize.Width;
            pictureBox.Height = SystemInformation.PrimaryMonitorSize.Height;
        }

        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 27)
            {
                Close();
            }
            if (e.KeyCode == Keys.A)
            {
                left = true;
            }
            if (e.KeyCode == Keys.W)
            {
                up = true;
            }
            if (e.KeyCode == Keys.D)
            {
                right = true;
            }
            if (e.KeyCode == Keys.S)
            {
                down = true;
            }
        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            pictureBox.Invalidate();
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            uniqueAngles.Clear();
            float angleMouseRay = (float)Math.Atan2(mouseRay.Mouse.Y - player.Y, mouseRay.Mouse.X - player.X);
            float angleMouseRayLeft = angleMouseRay - 0.5f;
            float angleMouseRayRight = angleMouseRay + 0.5f;
            uniqueAngles.Add(angleMouseRayLeft);
            uniqueAngles.Add(angleMouseRayRight);
            //высчитываем уникальные углы
            for (int j = 0; j < uniquePoints.Count; j++)
            {
                var uniquePoint = uniquePoints[j];
                float angle = (float)Math.Atan2(uniquePoint.Point.Y - player.Y, uniquePoint.Point.X - player.X);
                if (angle > angleMouseRayLeft & angle < angleMouseRayRight)
                {
                    uniquePoint.Angle = angle; // в местах добавления углов доступна оптимизация по выбору полярности векторов
                    uniqueAngles.Add((float)(angle - 0.0002));
                    uniqueAngles.Add(angle);
                    uniqueAngles.Add((float)(angle + 0.0002));
                    continue;
                }
                if (angleMouseRayLeft < -Math.PI)
                {
                    angle -= (float)(2 * Math.PI);
                }
                else if (angleMouseRayRight > Math.PI)
                {
                    angle += (float)(2 * Math.PI);
                }
                else
                {
                    continue;
                }
                if (angle > angleMouseRayLeft & angle < angleMouseRayRight)
                {
                    uniquePoint.Angle = angle;
                    uniqueAngles.Add((float)(angle - 0.0002));
                    uniqueAngles.Add(angle);
                    uniqueAngles.Add((float)(angle + 0.0002));
                }
            }

            QuickSorting.Sorting(uniqueAngles, 0, uniqueAngles.Count - 1);
            foreach (Polygon pol in polygons)
            {
                pol.Draw(e.Graphics);
            }

            //расчитываешь для каждого уникального угла ближайшую точку пересечения
            for (var j = 0; j < uniqueAngles.Count; j++)
            {
                var angle = uniqueAngles[j];

                // Calculate dx & dy from angle
                var dx = Math.Cos(Convert.ToDouble(angle));
                var dy = Math.Sin(Convert.ToDouble(angle));

                // Ray from center of screen to mouse
                rays[j].Mouse = new PointF((float)(player.X + dx), (float)(player.Y + dy));
                NewRay(rays[j]);
                rays[j].Draw(e.Graphics);
            }
            for (var j = 0; j < uniqueAngles.Count - 1; j++)
            {
                PointF[] p = { new PointF(player.X, player.Y), rays[j].Mouse, rays[j + 1].Mouse };
                e.Graphics.FillPolygon(new SolidBrush(Color.FromArgb(50, 255, 255, 255)), p);
            }

            player.Draw(e.Graphics);

            if (left)
            {
                Player falsePlayer = new Player(player.X - player.speed*2, player.Y, player.Size);
                if (!Intersection.CheckIntersect(falsePlayer, polygons))
                {
                    player.MoveLeft();
                    mouseRay.MoveLeft();
                    for (int i = 0; i < rays.Count; i++)
                        rays[i].MoveLeft();
                }
            }
            if (up)
            {
                Player falsePlayer = new Player(player.X, player.Y - player.speed*2, player.Size);
                if (!Intersection.CheckIntersect(falsePlayer, polygons))
                {
                    player.MoveUp();
                    mouseRay.MoveUp();
                    for (int i = 0; i < rays.Count; i++)
                        rays[i].MoveUp();
                }
            }
            if (right)
            {
                Player falsePlayer = new Player(player.X + player.speed*2, player.Y, player.Size);
                if (!Intersection.CheckIntersect(falsePlayer, polygons))
                {
                    player.MoveRight();
                    mouseRay.MoveRight();
                    for (int i = 0; i < rays.Count; i++)
                        rays[i].MoveRight();
                }
            }
            if (down)
            {
                Player falsePlayer = new Player(player.X, player.Y + player.speed*2, player.Size);
                if (!Intersection.CheckIntersect(falsePlayer, polygons))
                {
                    player.MoveDown();
                    mouseRay.MoveDown();
                    for (int i = 0; i < rays.Count; i++)
                        rays[i].MoveDown();
                }
            }

        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            mouseRay.Mouse = new PointF(e.X, e.Y);
        }

        private void Main_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A)
            {
                left = false;
            }
            if (e.KeyCode == Keys.W)
            {
                up = false;
            }
            if (e.KeyCode == Keys.D)
            {
                right = false;
            }
            if (e.KeyCode == Keys.S)
            {
                down = false;
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            
        }

        private void Main_Load(object sender, EventArgs e)
        {
            uniquePoints = new List<UniquePoint>();
            rays = new List<Ray>();
            using (StreamReader sr = new StreamReader("polygons.txt"))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    string[] points = line.Split(';');
                    for (int i = 0; i < points.Length - 1; i++)
                    {
                        UniquePoint uniquePoint = new UniquePoint
                        {
                            Point = new PointF(float.Parse(points[i].Split(',')[0]), float.Parse(points[i].Split(',')[1]))
                        };
                        uniquePoints.Add(uniquePoint);
                    }
                    Polygon polygon = new Polygon(points);
                    polygons.Add(polygon);
                    for (int i = 0; i < points.Length * 3; i++)
                    {
                        Ray ray = new Ray
                        {
                            Begin = new Point(20, 20)
                        };
                        rays.Add(ray);
                    }
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
                ray.Mouse = new PointF(closestIntersect.Intersection.X, closestIntersect.Intersection.Y);
        }

    }
}
