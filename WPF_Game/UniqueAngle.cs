using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Game
{
    class UniqueAngle
    {
        public float PrevAngle { set; get; }
        public float Angle { set; get; }
        public float NextAngle { set; get; }

        public UniqueAngle(float prevAngle, float angle, float nextAngle)
        {
            PrevAngle = prevAngle;
            Angle = angle;
            NextAngle = nextAngle;
        }
       
    }
}
