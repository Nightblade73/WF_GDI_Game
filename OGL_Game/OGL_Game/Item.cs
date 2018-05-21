using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OGL_Game
{
    class Item
    {
        public PointF Position { set; get; }
        public int size = 20;

        public Item(float x, float y)
        {
            Position = new PointF(x, y);
        }

        public void Draw(OpenGL gl, float width)
        {
            gl.Begin(OpenGL.GL_QUADS);
            gl.Color(1f, 0f, 0f);
            gl.Vertex(Position.X - size / 2 - width, Position.Y - size / 2 - width);
            gl.Vertex(Position.X - size / 2 - width, Position.Y + size / 2 + width);
            gl.Vertex(Position.X + size / 2 + width, Position.Y + size / 2 + width);
            gl.Vertex(Position.X + size / 2 + width, Position.Y - size / 2 - width);
            gl.Color(1f, 1f, 0f);
            gl.Vertex(Position.X - size / 2, Position.Y - size / 2);
            gl.Vertex(Position.X - size / 2, Position.Y + size / 2);
            gl.Vertex(Position.X + size / 2, Position.Y + size / 2);
            gl.Vertex(Position.X + size / 2, Position.Y - size / 2);
            gl.End();
        }
        public void Draw(OpenGL gl)
        {
            gl.Begin(OpenGL.GL_QUADS);
            gl.Color(1f, 1f, 0f);
            gl.Vertex(Position.X - size / 2, Position.Y - size / 2);
            gl.Vertex(Position.X - size / 2, Position.Y + size / 2);
            gl.Vertex(Position.X + size / 2, Position.Y + size / 2);
            gl.Vertex(Position.X + size / 2, Position.Y - size / 2);
            gl.End();
        }
    }
}
