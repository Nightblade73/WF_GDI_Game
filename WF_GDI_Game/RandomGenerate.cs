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
                case 2:
                    return new PointF(97, 310);
                case 3:
                    return new PointF(259, 417);
                case 4:
                    return new PointF(608, 388);
                case 5:
                    return new PointF(367, 288);
                case 6:
                    return new PointF(579, 86);
                case 7:
                    return new PointF(542, 321);
                case 8:
                    return new PointF(296, 30);
                case 9:
                    return new PointF(620, 20);
                case 10:
                    return new PointF(20, 20);
                case 11:
                    return new PointF(620, 440);
                case 12:
                    return new PointF(20, 440);
            }
            return new PointF();
        }


    }
}




