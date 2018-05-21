using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WPF_Game.Properties;

namespace WPF_Game
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer;
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
        List<Point> backViewSide;
        Ray mouseRay;
        List<Item> items;
        float angleMouseRayLeft;
        float angleMouseRayRight;
        Exit exit;

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
            if (e.Key == Key.A)
            {
                left = true;
            }
            if (e.Key == Key.W)
            {
                up = true;
            }
            if (e.Key == Key.D)
            {
                right = true;
            }
            if (e.Key == Key.S)
            {
                down = true;
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.A)
            {
                left = false;
            }
            if (e.Key == Key.W)
            {
                up = false;
            }
            if (e.Key == Key.D)
            {
                right = false;
            }
            if (e.Key == Key.S)
            {
                down = false;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            ItemCounts.Content = "Слитков золота найден: 0";
            rand = new Random();
            player = new Player(20, 20, 25, 0);
            items = new List<Item>();
            //     for(int i = 0; i< rand.Next(2,4); i++)
            int k = rand.Next(1, 2);
            int j = rand.Next(0, 1);

            for (int i = 0; i < 5; i++)
            {
                Item item = new Item(RandomGenerate.GetCoord((k * i + j) % 9).X, RandomGenerate.GetCoord((k * i + j) % 9).Y);
                items.Add(item);
            }
            exit = new Exit(RandomGenerate.GetCoord(rand.Next(9, 12)).X, RandomGenerate.GetCoord(rand.Next(9, 12)).Y);
            polygons = new List<Polygon>();
            uniqueAngles = new List<float>();
            uniqueAnglesNotOnFOV = new List<float>();
            intersects = new List<ParamPoint>();
            backViewSide = new List<Point>();
            mouseRay = new Ray
            {
                Begin = new Point(player.X, player.Y),
                Mouse = new Point(50, 50)
            };
            raysFOG = new List<Ray>();
            raysBACK = new List<Ray>();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
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
                            Point = new Point(int.Parse(points[i].Split(',')[0]), int.Parse(points[i].Split(',')[1]))
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

            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timerTick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            timer.Start();
        }

        private void timerTick(object sender, EventArgs e)
        {
            if (items.Count == 0)
            {
                Description.Content = "Найдите выход!";
            }
            if (items.Count == 0 && Math.Abs(exit.Position.X - player.X) < 40 && Math.Abs(exit.Position.Y - player.Y) < 40)
            {
                Description.Content = "Вы победили!";
                timer.Stop();
            }
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
                    Point uniquePoint = polygons[i].segments[j].End;
                    float angle = (float)Math.Atan2(uniquePoint.Y - player.Y, uniquePoint.X - player.X);
                    if (angle > angleMouseRayLeft & angle < angleMouseRayRight)
                    {
                        //  uniquePoint.Angle = angle; // в местах добавления углов доступна оптимизация по выбору полярности векторов
                        Point directiveVector = new Point(player.Y - uniquePoint.Y, uniquePoint.X - player.X);

                        Segment segBefor = polygons[i].segments[j];
                        Segment segAfter = polygons[i].segments[(j + 1) % polygons[i].segments.Count];
                        Point firstVector = new Point(segBefor.End.X - segBefor.Begin.X, segBefor.End.Y - segBefor.Begin.Y);
                        Point secondVector = new Point(segAfter.End.X - segAfter.Begin.X, segAfter.End.Y - segAfter.Begin.Y);

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
                        Point directiveVector = new Point(player.Y - uniquePoint.Y, uniquePoint.X - player.X);

                        Segment segBefor = polygons[i].segments[j];
                        Segment segAfter = polygons[i].segments[(j + 1) % polygons[i].segments.Count];
                        Point firstVector = new Point(segBefor.End.X - segBefor.Begin.X, segBefor.End.Y - segBefor.Begin.Y);
                        Point secondVector = new Point(segAfter.End.X - segAfter.Begin.X, segAfter.End.Y - segAfter.Begin.Y);

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
                Point uniquePoint = polygons[0].segments[j].End;
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
                rays[j].Mouse = new Point((float)(player.X + dx), (float)(player.Y + dy));
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
                raysFOG[j].Mouse = new Point((float)(player.X + dx), (float)(player.Y + dy));
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
            InvalidateGameField();
        }

        private void InvalidateGameField()
        {

            if (GameField.Children.Count > 0)
            {
                GameField.Children.Clear();
            }

            if (items.Count == 0)
            {
                GameField.Children.Add(exit.Draw());
            }

            for (int i = 0; i < items.Count; i++)
            {
                if (CheckCursor(i))
                {
                    if (CheckPlayerPosition(i))
                    {
                        GameField.Children.Add(items[i].Draw(2));
                    }
                    else
                    {
                        GameField.Children.Add(items[i].Draw(1));
                    }
                }
                else
                {
                    GameField.Children.Add(items[i].Draw());
                }
            }

            List<Point> pFOW = new List<Point>();
            for (var j = 0; j < uniqueAnglesNotOnFOV.Count; j++)
            {

                if (uniqueAnglesNotOnFOV[j] < angleMouseRayLeft)
                {
                    Point[] p = { new Point(player.X, player.Y), raysFOG[j].Mouse, raysFOG[(j + 1) % uniqueAnglesNotOnFOV.Count].Mouse };
                    System.Windows.Shapes.Polygon pol = new System.Windows.Shapes.Polygon
                    {
                        Points = new PointCollection(p),
                        Fill = System.Windows.Media.Brushes.Black
                    };
                    GameField.Children.Add(pol);
                }
                else if (uniqueAnglesNotOnFOV[j] >= angleMouseRayRight)
                {
                    Point[] p = { new Point(player.X, player.Y), raysFOG[j].Mouse, raysFOG[(j + 1) % uniqueAnglesNotOnFOV.Count].Mouse };
                    System.Windows.Shapes.Polygon pol = new System.Windows.Shapes.Polygon
                    {
                        Points = new PointCollection(p),
                        Fill = System.Windows.Media.Brushes.Black
                    };
                    GameField.Children.Add(pol);
                }

            }

            backViewSide.Clear();
            for (var j = 0; j < uniqueAngles.Count; j++)
            {
                backViewSide.Add(rays[j].Mouse);
            }
            for (var j = 0; j < uniqueAngles.Count; j++)
            {
                backViewSide.Add(raysBACK[j].Mouse);
            }

            if (backViewSide.Count > 0)
            {
                System.Windows.Shapes.Polygon pol = new System.Windows.Shapes.Polygon
                {
                    Points = new PointCollection(backViewSide),
                    Fill = System.Windows.Media.Brushes.Black
                };
                GameField.Children.Add(pol);
            }

            for (int i = 1; i < polygons.Count; i++)
            {
                GameField.Children.Add(polygons[i].Draw());
            }
            GameField.Children.Add(player.Draw());
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

        private void GameField_MouseMove(object sender, MouseEventArgs e)
        {
            mouseRay.Mouse = new Point((float)e.GetPosition(this).X, (float)e.GetPosition(this).Y);
        }

        private void GameField_MouseUp(object sender, MouseButtonEventArgs e)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (CheckPlayerPosition(i))
                    if (CheckCursor(i))
                    {
                        items.RemoveAt(i);
                        i--;
                        ItemCounts.Content = "Слитков золота найден: " + (5 - items.Count);
                    }
            }
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
                ray.Mouse = new Point(closestIntersect.Intersection.X, closestIntersect.Intersection.Y);
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
            if (items[i].Position.X - items[i].size / 2 < mouseRay.Mouse.X && items[i].Position.X + items[i].size / 2 > mouseRay.Mouse.X &&
                           items[i].Position.Y - items[i].size / 2 < mouseRay.Mouse.Y && items[i].Position.Y + items[i].size / 2 > mouseRay.Mouse.Y)
            {
                return true;
            }
            return false;
        }
    }
}
