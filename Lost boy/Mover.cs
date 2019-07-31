using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace Lost_boy
{
    public class Mover : IMover
    {

        public Vector Position
        {
            get;
            set;
        }

        public Vector Speed
        {
            get;
            set;
        }

        public Vector Acceleration
        {
            get;
            set;
        }

        public Vector Size
        {
            get;
            set;
        }

        public virtual void Move()
        {
            Position += Speed;
            Speed += Acceleration;
        }

        public Mover(Vector pos, Vector speed, Vector acc)
        {
            Position = pos;
            Speed = speed;
            Acceleration = acc;
        }
    }
}
