using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Properties;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace Game
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

        public void Draw(RenderTarget renderTarget)
        {
            using (var brush = new SolidColorBrush(renderTarget, SharpDX.Color.Red))
            {
                renderTarget.FillEllipse(new Ellipse(new RawVector2(X, Y), Size / 2, Size / 2), brush);
            }
        }


        public void MoveUp(float cos, float sin)
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
