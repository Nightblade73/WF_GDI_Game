using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    class Ray
    {
        public PointF Begin = new Point();
        public float speed = 0.1f;
        public PointF Mouse { set; get; }

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
