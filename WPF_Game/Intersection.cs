using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Game
{
    static class Intersection
    {
        // Find intersection of RAY & SEGMENT
        public static ParamPoint GetIntersection(Ray ray, Segment segment)
        {

            // RAY in parametric: Point + Direction*T1
            double r_px = ray.Begin.X;
            double r_py = ray.Begin.Y;
            double r_dx = ray.Mouse.X - ray.Begin.X;
            double r_dy = ray.Mouse.Y - ray.Begin.Y;

            // SEGMENT in parametric: Point + Direction*T2
            double s_px = segment.Begin.X;
            double s_py = segment.Begin.Y;
            double s_dx = segment.End.X - segment.Begin.X;
            double s_dy = segment.End.Y - segment.Begin.Y;

            // Are they parallel? If so, no intersect
            double r_mag = Math.Sqrt(r_dx * r_dx + r_dy * r_dy);
            double s_mag = Math.Sqrt(s_dx * s_dx + s_dy * s_dy);
            if (r_dx / r_mag == s_dx / s_mag && r_dy / r_mag == s_dy / s_mag)
            { // Направления совпадают
                return null;
            }

            // SOLVE FOR T1 & T2
            // r_px+r_dx*T1 = s_px+s_dx*T2 && r_py+r_dy*T1 = s_py+s_dy*T2
            // ==> T1 = (s_px+s_dx*T2-r_px)/r_dx = (s_py+s_dy*T2-r_py)/r_dy
            // ==> s_px*r_dy + s_dx*T2*r_dy - r_px*r_dy = s_py*r_dx + s_dy*T2*r_dx - r_py*r_dx
            // ==> T2 = (r_dx*(s_py-r_py) + r_dy*(r_px-s_px))/(s_dx*r_dy - s_dy*r_dx)
            double T2 = (r_dx * (s_py - r_py) + r_dy * (r_px - s_px)) / (s_dx * r_dy - s_dy * r_dx);
            double T1 = (s_px + s_dx * T2 - r_px) / r_dx;

            // Must be within parametic whatevers for RAY/SEGMENT
            if (T1 < 0) return null;
            if (T2 < 0 || T2 > 1) return null;

            // Деление на ноль возвращает такое интовское значение -2147483648
            // Избавляемся от него повторной операцией со смещением по Х
            if ((int)(r_px + r_dx * T1) == -2147483648)
            {
                r_dx += 0.000001;
                T2 = (r_dx * (s_py - r_py) + r_dy * (r_px - s_px)) / (s_dx * r_dy - s_dy * r_dx);
                T1 = (s_px + s_dx * T2 - r_px) / r_dx;
                if (T1 < 0) return null;
                if (T2 < 0 || T2 > 1) return null;
            }
            return new ParamPoint
            {
                Intersection = new PointF((float)(r_px + r_dx * T1), (float)(r_py + r_dy * T1)),
                T1 = T1
            };
        }

        public static bool CheckIntersect(Player player, Segment segment)
        {
            return СircleBySegment((float)segment.Begin.X, (float)segment.Begin.Y, (float)segment.End.X, (float)segment.End.Y, player.X, player.Y, player.Size / 2);
        }

        public static bool CheckIntersect(Player player, List<Polygon> polygons)
        {
            foreach (Polygon pol in polygons)
            {
                foreach (Segment seg in pol.segments)
                {
                    if (CheckIntersect(player, seg))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        static bool СircleBySegment(float x1, float y1, float x2, float y2, float x3, float y3, float radius)
        {
            var a = (x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1);
            var b = 2 * ((x2 - x1) * (x1 - x3) + (y2 - y1) * (y1 - y3));
            var c = x3 * x3 + y3 * y3 + x1 * x1 + y1 * y1 - 2 * (x3 * x1 + y3 * y1) - radius * radius;
            //т.е. если если есть отрицательные корни, то пересечение есть. анализируем теорему виета и формулу корней
            //на предмет отрицательных корней
            if (-b < 0)
            {
                return (c < 0);
            }
            if (-b < (2 * a))
            {
                return (4 * a * c - b * b < 0);
            }

            return (a + b + c < 0);
        }
    }
}
