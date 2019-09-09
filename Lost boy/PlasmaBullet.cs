using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Lost_boy
{
    public class PlasmaBullet : Bullet
    {
        private Rectangle drawable;
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
            p.Color = Color;
            g.DrawEllipse(p, drawable);
        }

        public override void Move()
        {
            base.Move();
            drawable.X = this.Position.X;
            drawable.Y = this.Position.Y;
        }

        public PlasmaBullet(Vector pos, Direction dir) :
            base(pos, new Vector(10, 10), dir, VALUES.BASIC_LASER_SPEED * 2, VALUES.BASIC_LASER_DMG / 2)
        {
            this.size = Size;
            this.direction = dir;
            this.Color = Color.Pink;
            drawable = new Rectangle(Position.X, Position.Y, size.X, size.Y);
        }
    }
}