using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OGL_Game
{
    class Ray
    {
        public PointF Begin = new Point();
        private int speed = 2;
        public PointF Mouse { set; get; }


        public void Draw(Graphics gr)
        {

            gr.DrawLine(new Pen(Color.White, 1), Begin.X, Begin.Y, Mouse.X, Mouse.Y);
        }

        public void MoveUp(float cos, float sin)
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
