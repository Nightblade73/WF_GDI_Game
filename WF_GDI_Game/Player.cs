using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WF_GDI_Game
{
    class Player
    {
        public float X { set; get; }
        public float Y { set; get; }
        public int Size { set; get; }
        public int speed = 1;

        public Player(float x, float y, int size)
        {
            X = x;
            Y = y;
            Size = size;
        }

        public void Draw(Graphics gr)
        {
            gr.FillEllipse(new SolidBrush(Color.Red), X - Size / 2, Y - Size / 2, Size, Size);
        }

        public void MoveUp(float cos, float sin)
        {
            //X += cos * speed;
            //Y += sin * speed;
            Y -= speed;
        }

        public void MoveDown()
        {
            Y += speed;
        }

        public void MoveRight()
        {
            X += speed;
        }

        public void MoveLeft()
        {
            X -= speed;
        }
    }
}
