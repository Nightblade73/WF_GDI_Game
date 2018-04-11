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
        public PointF Begin = new Point();
        private int speed = 1;
        public PointF Mouse { set; get; }

        //public Ray(int x, int y)
        //{
        //    X = x;
        //    Y = y;
        //}

        public void Draw(Graphics gr)
        {
            gr.DrawLine(new Pen(Color.White, 1), Begin.X, Begin.Y, Mouse.X, Mouse.Y);
        }

        public void MoveUp(float cos, float sin)
        {
            //Begin.X += cos * speed;
            //Begin.Y += sin * speed;
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
