using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WF_GDI_Game
{
    public static class RandomGenerate
    {
        public static PointF GetCoord(int i)
        {
            return PointsStuct(i);
        }
        private static PointF PointsStuct(int i)
        {
            switch (i)
            {
                case 0:
                    return new PointF(200, 200);
                case 1:
                    return new PointF(400, 100);
            }
            return new PointF();
        }
    }
}




