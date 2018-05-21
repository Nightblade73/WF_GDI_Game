using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OGL_Game
{
    class Exit
    {
        public PointF Position { set; get; }
        public int size = 40;

        public Exit(float x, float y)
        {
            Position = new PointF(x, y);
        }

        public void Draw(OpenGL gl)
        {
            gl.Begin(OpenGL.GL_QUADS);
            gl.Color(1f, 0f, 0f, 0.5f);
            gl.Vertex(Position.X - size / 2, Position.Y - size / 2);
            gl.Vertex(Position.X - size / 2, Position.Y + size / 2);
            gl.Vertex(Position.X + size / 2, Position.Y + size / 2);
            gl.Vertex(Position.X + size / 2, Position.Y - size / 2);
            gl.End();
        }
    }
}
