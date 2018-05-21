using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using SharpDX;
using SharpDX.DXGI;
using D3D = SharpDX.Direct3D;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.Windows;
using Device = SharpDX.Direct3D11.Device;
using Factory2D = SharpDX.Direct2D1.Factory;
using FactoryDXGI = SharpDX.DXGI.Factory;
using Text = SharpDX.DirectWrite.TextFormat;
using System.Windows.Forms;
using System.IO;
using SharpDX.Mathematics.Interop;

namespace Game
{
    static class Graphics
    {
        public static RenderForm renderForm = null;
        private static string title = "Tutorial 1";
        private static Device device;
        private static SwapChain swapChain;
        private static RenderTarget renderTarget;
        private static Text textFormat;
        private static Factory2D factory2d;

        static bool up, down, left, right;
        static Random rand;
        static List<Polygon> polygons;
        static Player player;
        static List<Ray> rays;
        static List<Ray> raysFOG;
        static List<Ray> raysBACK;
        static List<ParamPoint> intersects;
        static List<UniquePoint> uniquePoints;
        static List<float> uniqueAngles;
        static List<float> uniqueAnglesNotOnFOV;
        static List<PointF> backViewSide;
        static Ray mouseRay;
        static List<Item> items;
        static float angleMouseRayLeft;
        static float angleMouseRayRight;
        static Exit exit;

        private static void InitialDirectX()
        {
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
            backViewSide = new List<PointF>();
            mouseRay = new Ray
            {
                Begin = new PointF(player.X, player.Y),
                Mouse = new PointF(Cursor.Position.X, Cursor.Position.Y)
            };
            raysFOG = new List<Ray>();
            raysBACK = new List<Ray>();



            // creat form;
            renderForm = new RenderForm(title);
            // set windows size
            renderForm.Size = new System.Drawing.Size(892, 498);
            renderForm.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            // creat swapchain
            var swapChainDesc = new SwapChainDescription()
            {
                BufferCount = 4,
                Flags = SwapChainFlags.AllowModeSwitch,
                IsWindowed = true,
                ModeDescription = new ModeDescription(0, 0, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                OutputHandle = renderForm.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };

            D3D.FeatureLevel[] featureLevel = { D3D.FeatureLevel.Level_9_3, D3D.FeatureLevel.Level_10_1, D3D.FeatureLevel.Level_11_1 };
            Device.CreateWithSwapChain(D3D.DriverType.Hardware, DeviceCreationFlags.BgraSupport, featureLevel, swapChainDesc, out device, out swapChain);
            Surface backbuffer = Surface.FromSwapChain(swapChain, 0);

            factory2d = new Factory2D(FactoryType.MultiThreaded);

            var dpi = factory2d.DesktopDpi;

            renderTarget = new RenderTarget(factory2d, backbuffer, new RenderTargetProperties()
            {
                DpiX = dpi.Width,
                DpiY = dpi.Height,
                MinLevel = FeatureLevel.Level_9,
                PixelFormat = new PixelFormat(Format.R8G8B8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Ignore),
                Type = RenderTargetType.Default,
                Usage = RenderTargetUsage.None
            });


            // auto resize windows system
            renderForm.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            renderTarget.AntialiasMode = AntialiasMode.Aliased;
            renderTarget.TextAntialiasMode = TextAntialiasMode.Aliased;

            using (SharpDX.DirectWrite.Factory textFactory = new SharpDX.DirectWrite.Factory(SharpDX.DirectWrite.FactoryType.Shared))
            {
                textFormat = new Text(
                    textFactory,
                    "MS Sans Serif",
                    SharpDX.DirectWrite.FontWeight.SemiBold,
                    SharpDX.DirectWrite.FontStyle.Normal,
                    SharpDX.DirectWrite.FontStretch.Medium,
                    16.0f);
            }


            renderForm.Click += RenderForm_Click;
            renderForm.KeyUp += RenderForm_KeyUp;
            renderForm.KeyDown += RenderForm_KeyDown;
            renderForm.Load += RenderForm_Load;
            renderForm.MouseMove += RenderForm_MouseMove;
            
        }



        public static void Render()
        {
            // initial directx before
            InitialDirectX();
            // call render loop
            RenderLoop.Run(renderForm, () =>
            {
                if (renderForm.WindowState == FormWindowState.Minimized)
                    return;

                try
                {
                    Culc();
                    // clear all data in graphic card
                    renderTarget.BeginDraw();
                    renderTarget.Transform = Matrix3x2.Identity;
                    renderTarget.Clear(SharpDX.Color.White);
    
                    // ---- begin draw ----
                    InvalidatePictureBox();
                    using (var brush = new SolidColorBrush(renderTarget, SharpDX.Color.Black))
                    {
                        renderTarget.DrawText("Слитков золота найден: " + (5 - items.Count), textFormat, new RawRectangleF(650, 15, renderTarget.Size.Width, renderTarget.Size.Height), brush);
                        if (items.Count != 0)
                        {
                            renderTarget.DrawText("Найдите 5\nслитков золота", textFormat, new RawRectangleF(650, 410, renderTarget.Size.Width, renderTarget.Size.Height), brush);
                        }
                        if (items.Count == 0 && Math.Abs(exit.Position.X - player.X) < 40 && Math.Abs(exit.Position.Y - player.Y) < 40)
                        {
                            renderTarget.DrawText("Вы победили!", textFormat, new RawRectangleF(650, 410, renderTarget.Size.Width, renderTarget.Size.Height), brush);
                        }
                        else if (items.Count == 0)
                        {
                            renderTarget.DrawText("Найдите выход!.", textFormat, new RawRectangleF(650, 410, renderTarget.Size.Width, renderTarget.Size.Height), brush);
                        }

                    }
                    // ---- end draw ----
                    try
                    {
                        renderTarget.Flush();
                    }
                    catch (SharpDXException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    renderTarget.EndDraw();
                    swapChain.Present(0, PresentFlags.None);
                }
                catch (Exception ex)
                {
                    renderForm.Close();
                }
            });

            swapChain.IsFullScreen = false;
            textFormat.Dispose();
            renderTarget.Dispose();
            swapChain.Dispose();
            device.Dispose();
        }

        private static void InvalidatePictureBox()
        {

            //  gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;


            if (items.Count == 0)
            {
                exit.Draw(renderTarget);
            }

            for (int i = 0; i < items.Count; i++)
            {
                if (CheckCursor(i))
                {
                    if (CheckPlayerPosition(i))
                    {
                        items[i].Draw(renderTarget, 2);
                    }
                    else
                    {
                        items[i].Draw(renderTarget, 1);
                    }
                }
                else
                {
                    items[i].Draw(renderTarget);
                }
            }

            backViewSide.Clear();
            PointF last = new PointF();
            for (var j = 0; j < uniqueAnglesNotOnFOV.Count; j++)
            {

                if (uniqueAnglesNotOnFOV[j] < angleMouseRayLeft)
                {
                    PointF[] p = { new PointF(player.X, player.Y), raysFOG[j].Mouse, raysFOG[(j + 1) % uniqueAnglesNotOnFOV.Count].Mouse };
                    PathGeometry geo1;
                    GeometrySink sink1;
                    geo1 = new PathGeometry(factory2d);
                    sink1 = geo1.Open();
                    sink1.BeginFigure(new Vector2(player.X, player.Y), new FigureBegin());
                    sink1.AddLines(new RawVector2[] { new Vector2(raysFOG[j].Mouse.X, raysFOG[j].Mouse.Y),
                        new Vector2(raysFOG[(j + 1) % uniqueAnglesNotOnFOV.Count].Mouse.X, raysFOG[(j + 1) % uniqueAnglesNotOnFOV.Count].Mouse.Y) });
                    sink1.EndFigure(new FigureEnd());
                    sink1.Close();
                    using (var brush = new SolidColorBrush(renderTarget, SharpDX.Color.Black))
                    {
                        //  renderTarget.DrawGeometry(geo1, penBrush);
                        renderTarget.FillGeometry(geo1, brush);
                    }
                    geo1.Dispose();
                    sink1.Dispose();

                }
                else if (uniqueAnglesNotOnFOV[j] >= angleMouseRayRight)
                {
                    PointF[] p = { new PointF(player.X, player.Y), raysFOG[j].Mouse, raysFOG[(j + 1) % uniqueAnglesNotOnFOV.Count].Mouse };
                    PathGeometry geo1;
                    GeometrySink sink1;

                    geo1 = new PathGeometry(factory2d);
                    sink1 = geo1.Open();
                    sink1.BeginFigure(new Vector2(player.X, player.Y), new FigureBegin());
                    sink1.AddLines(new RawVector2[] { new Vector2(raysFOG[j].Mouse.X, raysFOG[j].Mouse.Y),
                        new Vector2(raysFOG[(j + 1) % uniqueAnglesNotOnFOV.Count].Mouse.X, raysFOG[(j + 1) % uniqueAnglesNotOnFOV.Count].Mouse.Y) });
                    sink1.EndFigure(new FigureEnd());
                    sink1.Close();
                    using (var brush = new SolidColorBrush(renderTarget, SharpDX.Color.Black))
                    {
                        //  renderTarget.DrawGeometry(geo1, penBrush);
                        renderTarget.FillGeometry(geo1, brush);
                    }
                    geo1.Dispose();
                    sink1.Dispose();

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
            {
                PathGeometry geo1;
                GeometrySink sink1;

                List<RawVector2> rwList = new List<RawVector2>();
                for (var j = 0; j < backViewSide.Count; j++)
                {
                    rwList.Add(new Vector2(backViewSide[j].X, backViewSide[j].Y));
                }
                geo1 = new PathGeometry(factory2d);
                sink1 = geo1.Open();
                sink1.BeginFigure(rwList[0], new FigureBegin());
                rwList.RemoveAt(0);
                sink1.AddLines(rwList.ToArray());
                sink1.EndFigure(new FigureEnd());
                sink1.Close();
                using (var brush = new SolidColorBrush(renderTarget, SharpDX.Color.Black))
                {
                    //  renderTarget.DrawGeometry(geo1, penBrush);
                    renderTarget.FillGeometry(geo1, brush);
                }
                geo1.Dispose();
                sink1.Dispose();

                //      gr.FillPolygon(new SolidBrush(Color.FromArgb(255, 0, 0, 0)), backViewSide.ToArray());
            }

            foreach (Polygon pol in polygons)
            {
                pol.Draw(renderTarget);
            }
                player.Draw(renderTarget);
        }

        private static void Culc()
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
        }

        private static void RenderForm_MouseMove(object sender, MouseEventArgs e)
        {
            mouseRay.Mouse = new PointF(e.X, e.Y);
        }

        private static void RenderForm_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (CheckPlayerPosition(i))
                    if (CheckCursor(i))
                    {
                        items.RemoveAt(i);
                        i--;
                        //        ItemCounts.Text = "Слитков золота найден: " + (5 - items.Count);
                    }
            }
        }

        private static void RenderForm_Load(object sender, EventArgs e)
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
                            Begin = new System.Drawing.Point(20, 20)
                        };
                        rays.Add(ray);
                    }
                }
            }
            for (int i = 0; i < uniquePoints.Count; i++)
            {
                Ray ray = new Ray
                {
                    Begin = new System.Drawing.Point(20, 20)
                };
                raysFOG.Add(ray);
            }
        }

        private static void RenderForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt && e.KeyCode == Keys.Enter)
            {
                swapChain.IsFullScreen = !swapChain.IsFullScreen;
            }
            if (e.KeyValue == 27)
            {
                renderForm.Close();
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

        private static void RenderForm_KeyUp(object sender, KeyEventArgs e)
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

        private static void NewRay(Ray ray)
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
        private static void NewRay(Ray ray, int i)
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

        private static bool CheckPlayerPosition(int i)
        {
            if (Math.Abs(items[i].Position.X - player.X) < 40 && Math.Abs(items[i].Position.Y - player.Y) < 40)
            {
                return true;
            }
            return false;
        }

        private static bool CheckCursor(int i)
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
