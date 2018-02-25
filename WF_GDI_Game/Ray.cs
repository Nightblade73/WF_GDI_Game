using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WF_GDI_Game
{
    class Ray
    {
        public Point Begin = new Point();
        private int speed = 5;
        public Point Mouse { set; get; }

        //public Ray(int x, int y)
        //{
        //    X = x;
        //    Y = y;
        //}

        public void Draw(Graphics gr)
        {
            gr.DrawLine(new Pen(Color.Red, 1), Begin.X, Begin.Y, (float)Mouse.X, (float)Mouse.Y);
        }

        public void MoveUp()
        {
            Begin.Y -= speed;
        }

        public void MoveDown()
        {
            Begin. Y += speed;
        }

        public void MoveRight()
        {
            Begin.X += speed;
        }

        public void MoveLeft()
        {
            Begin.X -= speed;
        }
    }
}
