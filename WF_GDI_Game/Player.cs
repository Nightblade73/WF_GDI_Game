using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WF_GDI_Game.Properties;

namespace WF_GDI_Game
{
    class Player
    {
        public float X { set; get; }
        public float Y { set; get; }
        public int Size { set; get; }
        public int ВirectionOfSight { set; get; }
        public float speed = 0.1f;

        public Player(float x, float y, int size, int birectionOfSight)
        {
            X = x;
            Y = y;
            Size = size;
            ВirectionOfSight = birectionOfSight;
        }

        public void Draw(Graphics gr)
        {
            gr.TranslateTransform(X, Y);
            gr.RotateTransform(ВirectionOfSight);
            gr.DrawImage(Resources.hat, -Size / 2, -Size / 2, Size, Size);
       //     gr.TranslateTransform(-X, -Y);
            //    gr.FillEllipse(new SolidBrush(Color.Red), X - Size / 2, Y - Size / 2, Size, Size);
        }

        //public void Rotate()
        //{
        //    gr.RotateTransform(30);
        //}
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
