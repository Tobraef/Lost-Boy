using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Lost_boy.Ammo
{
    namespace T1 
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

    namespace T2
    {
        public class StarPlasma : T1.PlasmaBullet
        {
            public StarPlasma(Vector position, Direction dir) :
                base(position, dir)
            {
                Speed = new Vector(Speed.X, Speed.Y - 3);
                Damage += 3;
                Size = new Vector(Size.X + 2, Size.Y + 2);
                onHits += new OnHits.ArmorMeltEffect(1);
            }
        }
    }

    namespace T3
    {
        public class Decimator : T1.PlasmaBullet
        {
            public Decimator(Vector position, Direction dir) :
                base(position, dir)
            {
                Speed = new Vector(Speed.X, Speed.Y - 5);
                Damage += 5;
                Size = new Vector(Size.X *2, Size.Y * 2);
                onHits += new OnHits.ArmorMeltEffect(1);
            }
        }
    }
}