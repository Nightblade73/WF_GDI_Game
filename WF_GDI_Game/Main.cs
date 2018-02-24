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

        //scene = {
        // polygons: [
        //     { coords: [ 70,50,190,70,170,140,100,130 ],
        //       colour: wallColour,
        //       stroke: 1,
        //       fill: true },
        //     { coords: [ 230,50,350,70,330,140,305,90 ],
        //       colour: wallColour,
        //       stroke: 1,
        //       fill: true },
        //     { coords: [ 475,56,475,360,616,360,616,56 ],
        //       colour: wallColour,
        //       stroke: 1,
        //       fill: true },
        //     { coords: [ 374,300,374,450 ],
        //       colour: wallColour,
        //       stroke: 1,
        //       fill: false },
        //     { coords: [188.57143,381.89539,
        //                167.824635,304.467285,
        //                328.25502,261.480095,
        //                268.205575,321.529535,
        //                330.053215,357.237285,
        //                207.155635,428.19224,
        //                226.618645,355.55529],
        //       color: wallColour,
        //       stroke: 1,
        //       fill: true },
        //     { coords: [ 100,200,120,250,60,300 ],
        //       colour: wallColour,
        //       stroke: 1,
        //       fill: true }
        //     ]
        //observer: { loc: vec2.fromValues(374, 203),
        //            dir: vec2.fromValues(-0.707106781186, 0.707106781186),
        //            colour: observerColour },
        //target: { loc: vec2.fromValues(293, 100),
        //          dir: vec2.fromValues(-1, 0),
        //          colour: targetColour }

        List<Polygon> polygons;
        Player player;

        public Main()
        {
            InitializeComponent();
            player = new Player(20, 20, 20);
            player.XView = 50;
            player.YView = 100;
            polygons = new List<Polygon>();
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
            }
            if (e.KeyValue == 38)
            {
                player.MoveUp();
            }
            if (e.KeyValue == 39)
            {
                player.MoveRight();
            }
            if (e.KeyValue == 40)
            {
                player.MoveDown();
            }


        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            Bitmap bmp = new Bitmap(pictureBox.Width, pictureBox.Height);
            using (Graphics gr = Graphics.FromImage(bmp))
            {
                player.Draw(gr);
                foreach(Polygon pol in polygons)
                {
                    pol.Draw(gr);
                }
                
            }
            pictureBox.Image = bmp;

        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            player.XView = e.X;
            player.YView = e.Y;
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
    }
}
