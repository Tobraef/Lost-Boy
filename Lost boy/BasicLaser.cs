using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Lost_boy.OnHits;

namespace Lost_boy
{
    public class BasicLaser : Laser
    {
        private Rectangle drawable;
        private Color color = Color.Red;
        private Vector size;

        public override Vector Size
        {
            get
            {
                return size;
            }
            set
            {
                drawable.Width = value.X;
                drawable.Height = value.Y;
                size = value;
            }
        }

        public override void Draw(Graphics g, Pen p)
        {
            p.Color = color;
            g.DrawRectangle(p, drawable);
        }

        public override void Move()
        {
            base.Move();
            drawable.X = this.Position.X;
            drawable.Y = this.Position.Y;
        }

        public BasicLaser(Vector pos, Direction dir) :
            base(pos, new Vector(5, 10), dir, VALUES.BASIC_LASER_SPEED, VALUES.BASIC_LASER_DMG)
        {
            this.size = Size;
            this.direction = dir;
            drawable = new Rectangle(Position.X, Position.Y, size.X, size.Y);
        }
    }
}