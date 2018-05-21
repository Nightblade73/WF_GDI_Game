using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OGL_Game.Properties;
using SharpGL;

namespace OGL_Game
{
    class Player
    {
        public float X { set; get; }
        public float Y { set; get; }
        public int Size { set; get; }
        public int ВirectionOfSight { set; get; }
        public int speed = 2;

        public Player(float x, float y, int size, int birectionOfSight)
        {
            X = x;
            Y = y;
            Size = size;
            ВirectionOfSight = birectionOfSight;
        }

        public void Draw(OpenGL gl)
        {

            gl.Begin(OpenGL.GL_TRIANGLE_FAN);
                 gl.Color(1f, 0f, 0f);
            gl.Vertex(X, Y);
            //    gl.Rotate(ВirectionOfSight, 0, 0, 1);
            for (double i = -Math.PI; i <= Math.PI + 0.4; i += Math.PI / 6)
            {
                gl.Vertex(10f * Math.Cos(i) + X, 10f * Math.Sin(i) + Y);
            }
            gl.End();
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
