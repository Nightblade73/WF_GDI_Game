using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OGL_Game
{
    class Segment
    {
        public PointF Begin { set; get; }

        public PointF End { set; get; }

        public Segment(PointF begin, PointF end)
        {
            Begin = begin;
            End = end;
        }
    }
}
