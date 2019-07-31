using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace Lost_boy
{
    public abstract class Mover : IMover
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

        public abstract void Draw(Graphics g, Pen p);

        public Mover(Vector pos, Vector speed, Vector acc, Vector size)
        {
            Size = size;
            Position = pos;
            Speed = speed;
            Acceleration = acc;
        }
    }
}