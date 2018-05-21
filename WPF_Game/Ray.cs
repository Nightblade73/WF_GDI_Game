using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPF_Game
{
    class Ray
    {
        public Point Begin = new Point();
        private int speed = 1;
        public Point Mouse { set; get; }


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
