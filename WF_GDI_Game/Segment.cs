using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WF_GDI_Game
{
    class Segment
    {
        public Point Begin { set; get; }

        public Point End { set; get; }

        public Segment(Point begin, Point end)
        {
            Begin = begin;
            End = end;
        }
    }
}
