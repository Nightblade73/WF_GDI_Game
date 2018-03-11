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
        public int X { set; get; }
        public int Y { set; get; }
        public int Size { set; get; }
        private int speed = 1;

        public Player(int x, int y, int size)
        {
            X = x;
            Y = y;
            Size = size;
        }

        public void Draw(Graphics gr)
        {
            gr.FillEllipse(new SolidBrush(Color.Red), X - Size / 2, Y - Size / 2, Size, Size);
        }

        public void MoveUp()
        {
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
