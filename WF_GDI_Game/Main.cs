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
using WF_GDI_Game.Properties;

namespace WF_GDI_Game
{
    public partial class Main : Form
    {
        bool up, down, left, right;
        Random rand;
        List<Polygon> polygons;
        Player player;
        List<Ray> rays;
        List<Ray> raysFOG;
        List<Ray> raysBACK;
        List<ParamPoint> intersects;
        List<UniquePoint> uniquePoints;
        List<float> uniqueAngles;
        List<float> uniqueAnglesNotOnFOV;
        List<PointF> backViewSide;
        Ray mouseRay;
        List<Item> items;
        float angleMouseRayLeft;
        float angleMouseRayRight;
        Bitmap bmp;

        public Main()
        {
            InitializeComponent();
            rand = new Random();
            player = new Player(20, 20, 25, 0);
            items = new List<Item>();
            //     for(int i = 0; i< rand.Next(2,4); i++)
            for (int i = 0; i < 2; i++)
            {
                items.Add(new Item(RandomGenerate.GetCoord(i).X, RandomGenerate.GetCoord(i).Y));
            }
            polygons = new List<Polygon>();
            uniqueAngles = new List<float>();
            uniqueAnglesNotOnFOV = new List<float>();
            intersects = new List<ParamPoint>();
            backViewSide = new List<PointF>();
            mouseRay = new Ray
            {
                Begin = new PointF(player.X, player.Y),
                Mouse = new PointF(Cursor.Position.X, Cursor.Position.Y)
            };
            raysFOG = new List<Ray>();
            raysBACK = new List<Ray>();
   //         pictureBox.Width = SystemInformation.PrimaryMonitorSize.Width;
   //         pictureBox.Height = SystemInformation.PrimaryMonitorSize.Height;
            timer.Start();
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

        }
        private void InvalidatePictureBox()
        {
            if (bmp != null)
                bmp.Dispose();
            bmp = new Bitmap(Resources.игровое_поле);
            Graphics gr = Graphics.FromImage(bmp);
            //  gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;


            //расчитываешь для каждого уникального угла ближайшую точку пересечения

            for (var j = 0; j < uniqueAngles.Count - 1; j++)
            {
                PointF[] p = { new PointF(player.X, player.Y), rays[j].Mouse, rays[j + 1].Mouse };
     //           gr.FillPolygon(new SolidBrush(Color.FromArgb(50, 255, 255, 255)), p);
            }
            for (int i = 0; i < items.Count; i++)
            {
                if (CheckCursor(i))
                {
                    if (CheckPlayerPosition(i))
                    {
                        items[i].Draw(gr, 2);
                    }
                    else
                    {
                        items[i].Draw(gr, 1);
                    }
                }
                else
                {
                    items[i].Draw(gr);
                }
            }

            backViewSide.Clear();
            PointF last = new PointF();
            for (var j = 0; j < uniqueAnglesNotOnFOV.Count; j++)
            {
                //PointF[] p = { new PointF(player.X, player.Y), raysFOG[j].Mouse, raysFOG[(j + 1) % uniqueAnglesNotOnFOV.Count].Mouse };
                //gr.FillPolygon(new SolidBrush(Color.FromArgb(255, 0, 0, 0)), p);

                if (uniqueAnglesNotOnFOV[j] < angleMouseRayLeft)
                {
                    PointF[] p = { new PointF(player.X, player.Y), raysFOG[j].Mouse, raysFOG[(j + 1) % uniqueAnglesNotOnFOV.Count].Mouse };
                    gr.FillPolygon(new SolidBrush(Color.FromArgb(255, 0, 0, 0)), p);
                }
                else if (uniqueAnglesNotOnFOV[j] >= angleMouseRayRight)
                {
                    PointF[] p = { new PointF(player.X, player.Y), raysFOG[j].Mouse, raysFOG[(j + 1) % uniqueAnglesNotOnFOV.Count].Mouse };
                    gr.FillPolygon(new SolidBrush(Color.FromArgb(255, 0, 0, 0)), p);
                }

            }

            for (var j = 0; j < uniqueAngles.Count; j++)
            {
                //        rays[j].Draw(gr);
                backViewSide.Add(rays[j].Mouse);
            }
            for (var j = 0; j < uniqueAngles.Count; j++)
            {
                backViewSide.Add(raysBACK[j].Mouse);
            }
            if (backViewSide.Count > 0)
                gr.FillPolygon(new SolidBrush(Color.FromArgb(255, 0, 0, 0)), backViewSide.ToArray());
            //for (var j = 0; j < uniqueAnglesNotOnFOV.Count; j++)
            //{
            //    raysFOG[j].Draw(e.Graphics);
            //}
            //e.Graphics.FillPie();
            //fog of war
            //PointF[] points = { new PointF(player.X, player.Y), rays[0].Mouse, rays[uniqueAngles.Count].Mouse };

            // e.Graphics.FillPolygon(new SolidBrush(Color.FromArgb(50, 255, 255, 255)), points);
            //     e.Graphics.FillRectangle(new SolidBrush(Color.Black), 0,0,640,640);
            foreach (Polygon pol in polygons)
            {
                pol.Draw(gr);
            }
            player.Draw(gr);
            pictureBox.Image = bmp;
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
            uniqueAngles.Clear();
            uniqueAnglesNotOnFOV.Clear();
            //узнаём вектор взгляда
            float angleMouseRay = (float)Math.Atan2(mouseRay.Mouse.Y - player.Y, mouseRay.Mouse.X - player.X);
            player.ВirectionOfSight = (int)(angleMouseRay * 180f / Math.PI);
            //считаем косинус и синус угла вектора
            float dxАngleMouseRay = (float)Math.Cos(Convert.ToDouble(angleMouseRay));
            float dyАngleMouseRay = (float)Math.Sin(Convert.ToDouble(angleMouseRay));
            //выставляем угол обзора
            angleMouseRayLeft = angleMouseRay - 0.5f;
            angleMouseRayRight = angleMouseRay + 0.5f;
            uniqueAngles.Add(angleMouseRayLeft);
            uniqueAngles.Add(angleMouseRayRight);

            //высчитываем уникальные углы
            for (int i = 0; i < polygons.Count; i++)
            {
                for (int j = 0; j < polygons[i].segments.Count; j++)
                {
                    PointF uniquePoint = polygons[i].segments[j].End;
                    float angle = (float)Math.Atan2(uniquePoint.Y - player.Y, uniquePoint.X - player.X);
                    if (angle > angleMouseRayLeft & angle < angleMouseRayRight)
                    {
                        //  uniquePoint.Angle = angle; // в местах добавления углов доступна оптимизация по выбору полярности векторов
                        PointF directiveVector = new PointF(player.Y - uniquePoint.Y, uniquePoint.X - player.X);

                        Segment segBefor = polygons[i].segments[j];
                        Segment segAfter = polygons[i].segments[(j + 1) % polygons[i].segments.Count];
                        PointF firstVector = new PointF(segBefor.End.X - segBefor.Begin.X, segBefor.End.Y - segBefor.Begin.Y);
                        PointF secondVector = new PointF(segAfter.End.X - segAfter.Begin.X, segAfter.End.Y - segAfter.Begin.Y);

                        double directiveVectorLength = Math.Sqrt(directiveVector.X * directiveVector.X + directiveVector.Y * directiveVector.Y);
                        double firstProjection = (directiveVector.X * firstVector.X + directiveVector.Y * firstVector.Y) / directiveVectorLength;
                        double secondScalar = (directiveVector.X * secondVector.X + directiveVector.Y * secondVector.Y) / directiveVectorLength;

                        if (firstProjection > 0 & secondScalar > 0 || firstProjection < 0 & secondScalar < 0)
                        {
                            uniqueAngles.Add(angle);
                            continue;
                        }
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
                        //    uniquePoint.Angle = angle;
                        PointF directiveVector = new PointF(player.Y - uniquePoint.Y, uniquePoint.X - player.X);

                        Segment segBefor = polygons[i].segments[j];
                        Segment segAfter = polygons[i].segments[(j + 1) % polygons[i].segments.Count];
                        PointF firstVector = new PointF(segBefor.End.X - segBefor.Begin.X, segBefor.End.Y - segBefor.Begin.Y);
                        PointF secondVector = new PointF(segAfter.End.X - segAfter.Begin.X, segAfter.End.Y - segAfter.Begin.Y);

                        double directiveVectorLength = Math.Sqrt(directiveVector.X * directiveVector.X + directiveVector.Y * directiveVector.Y);
                        double firstProjection = (directiveVector.X * firstVector.X + directiveVector.Y * firstVector.Y) / directiveVectorLength;
                        double secondScalar = (directiveVector.X * secondVector.X + directiveVector.Y * secondVector.Y) / directiveVectorLength;

                        if (firstProjection > 0 & secondScalar > 0 || firstProjection < 0 & secondScalar < 0)
                        {
                            uniqueAngles.Add(angle);
                            continue;
                        }
                        uniqueAngles.Add((float)(angle - 0.0002));
                        uniqueAngles.Add(angle);
                        uniqueAngles.Add((float)(angle + 0.0002));
                        continue;
                    }
                }
            }
            // FOG
            float newAngleMouseRayRight = angleMouseRayRight;
            float newAngleMouseRayLeft = angleMouseRayLeft;
            if (angleMouseRayRight > Math.PI)
            {
                newAngleMouseRayRight -= (float)(2 * Math.PI);
            }
            if (angleMouseRayLeft < -Math.PI)
            {
                newAngleMouseRayLeft += (float)(2 * Math.PI);
            }

            for (int j = 0; j < polygons[0].segments.Count; j++)
            {
                PointF uniquePoint = polygons[0].segments[j].End;
                float angle = (float)Math.Atan2(uniquePoint.Y - player.Y, uniquePoint.X - player.X);
                if (angle > angleMouseRayLeft & angle < angleMouseRayRight)
                {                   
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
                    uniqueAnglesNotOnFOV.Add(angle);
                    continue;
                }
                if (angle > angleMouseRayLeft & angle < angleMouseRayRight)
                {
                    continue;
                }
                uniqueAnglesNotOnFOV.Add(angle);
            }

            uniqueAnglesNotOnFOV.Add(angleMouseRayRight);
            uniqueAnglesNotOnFOV.Add(angleMouseRayLeft);
            QuickSorting.Sorting(uniqueAnglesNotOnFOV, 0, uniqueAnglesNotOnFOV.Count - 1);
            QuickSorting.Sorting(uniqueAngles, 0, uniqueAngles.Count - 1);

            raysBACK.Clear();
            //расчитываешь для каждого уникального угла ближайшую точку пересечения
            for (var j = 0; j < uniqueAngles.Count; j++)
            {
                var dx = Math.Cos(Convert.ToDouble(uniqueAngles[j]));
                var dy = Math.Sin(Convert.ToDouble(uniqueAngles[j]));
                rays[j].Mouse = new PointF((float)(player.X + dx), (float)(player.Y + dy));
                NewRay(rays[j]);
                Ray clone = new Ray()
                {
                    Begin = rays[j].Begin,
                    Mouse = rays[j].Mouse
                };
                NewRay(clone, 0);
                raysBACK.Add(clone);
            }
            raysBACK.Reverse();
            for (var j = 0; j < uniqueAngles.Count; j++)
            {
                NewRay(rays[j]);
            }
            for (var j = 0; j < uniqueAnglesNotOnFOV.Count; j++)
            {
                var dx = Math.Cos(Convert.ToDouble(uniqueAnglesNotOnFOV[j]));
                var dy = Math.Sin(Convert.ToDouble(uniqueAnglesNotOnFOV[j]));
                raysFOG[j].Mouse = new PointF((float)(player.X + dx), (float)(player.Y + dy));
                NewRay(raysFOG[j], 0);

            }
            //куда идти
            if (left)
            {
                Player falsePlayer = new Player(player.X - player.speed * 2, player.Y, player.Size, 0);
                if (!Intersection.CheckIntersect(falsePlayer, polygons))
                {
                    player.MoveLeft();
                    mouseRay.MoveLeft();
                    for (int i = 0; i < rays.Count; i++)
                        rays[i].MoveLeft();
                    for (int i = 0; i < raysFOG.Count; i++)
                        raysFOG[i].MoveLeft();
                }
            }
            if (up)
            {
                Player falsePlayer = new Player(player.X, player.Y - player.speed * 2, player.Size, 0);
                if (!Intersection.CheckIntersect(falsePlayer, polygons))
                {
                    player.MoveUp(dxАngleMouseRay, dyАngleMouseRay);
                    mouseRay.MoveUp(dxАngleMouseRay, dyАngleMouseRay);
                    for (int i = 0; i < rays.Count; i++)
                        rays[i].MoveUp(dxАngleMouseRay, dyАngleMouseRay);
                    for (int i = 0; i < raysFOG.Count; i++)
                        raysFOG[i].MoveUp(dxАngleMouseRay, dyАngleMouseRay);
                }
            }
            if (right)
            {
                Player falsePlayer = new Player(player.X + player.speed * 2, player.Y, player.Size, 0);
                if (!Intersection.CheckIntersect(falsePlayer, polygons))
                {
                    player.MoveRight();
                    mouseRay.MoveRight();
                    for (int i = 0; i < rays.Count; i++)
                        rays[i].MoveRight();
                    for (int i = 0; i < raysFOG.Count; i++)
                        raysFOG[i].MoveRight();
                }
            }
            if (down)
            {
                Player falsePlayer = new Player(player.X, player.Y + player.speed * 2, player.Size, 0);
                if (!Intersection.CheckIntersect(falsePlayer, polygons))
                {
                    player.MoveDown();
                    mouseRay.MoveDown();
                    for (int i = 0; i < rays.Count; i++)
                        rays[i].MoveDown();
                    for (int i = 0; i < raysFOG.Count; i++)
                        raysFOG[i].MoveDown();
                }
            }
            InvalidatePictureBox();
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (CheckPlayerPosition(i))
                    if (CheckCursor(i))
                    {
                        items.RemoveAt(i);
                        i--;
                    }
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            uniquePoints = new List<UniquePoint>();
            rays = new List<Ray>();
            string[] points;
            using (StreamReader sr = new StreamReader("polygons.txt"))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    points = line.Split(';');
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
            for (int i = 0; i < uniquePoints.Count; i++)
            {
                Ray ray = new Ray
                {
                    Begin = new Point(20, 20)
                };
                raysFOG.Add(ray);
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
        private void NewRay(Ray ray, int i)
        {
            ParamPoint closestIntersect = null;

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
            if (closestIntersect != null)
                ray.Mouse = new PointF(closestIntersect.Intersection.X, closestIntersect.Intersection.Y);
        }
        private bool CheckPlayerPosition(int i)
        {
            if (Math.Abs(items[i].Position.X - player.X) < 40 && Math.Abs(items[i].Position.Y - player.Y) < 40)
            {
                return true;
            }
            return false;

        }
        private bool CheckCursor(int i)
        {
            if (items[i].Position.X - items[i].size / 2 < Cursor.Position.X && items[i].Position.X + items[i].size / 2 > Cursor.Position.X &&
                           items[i].Position.Y - items[i].size / 2 < Cursor.Position.Y && items[i].Position.Y + items[i].size / 2 > Cursor.Position.Y)
            {
                return true;
            }
            return false;
        }
    }
}
