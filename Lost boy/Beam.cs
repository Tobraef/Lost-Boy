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
        public class Beam : Bullet
        {
            public override event Action onDeath;
            private Vector size;
            private Rectangle drawable;
            private int lifeSpan = 1;

            public override void Move()
            {
                drawable.X = Position.X;
                drawable.Y = Position.Y;
                if (lifeSpan > 0)
                {
                    lifeSpan--;
                }
                else
                {
                    onDeath();
                }
            }

            public override Vector Size
            {
                get
                {
                    return size;
                }
                set
                {
                    size = value;
                    size.Y -= VALUES.PLAYER_HEIGHT;
                    drawable.Width = value.X;
                    drawable.Height = value.Y;
                }
            }

            public override void AffectShip(IShip ship)
            {
                if (lifeSpan > 0)
                    base.AffectShip(ship);
            }

            public override void Draw(Graphics g, Pen p)
            {
                p.Color = Color;
                g.DrawRectangle(p, drawable);
            }

            public Beam(Vector position, Direction dir) :
                base(
                position,
                new Vector(5, VALUES.HEIGHT),
                dir,
                0,
                30)
            {
                this.direction = dir;
                this.OnRecycle += t => lifeSpan = 2;
                this.drawable = new Rectangle
                {
                    X = position.X,
                    Y = position.Y,
                    Width = size.X,
                    Height = size.Y
                };
            }
        }
    }

    namespace T2
    {
        public class MortalCoil : T1.Beam
        {
            public MortalCoil(Vector position, Direction dir) :
                base(position, dir)
            {
                Color = Color.Aqua;
                Damage = 45;
                onHits += new OnHits.ArmorMeltEffect(3);
            }
        }
    }

    namespace T3
    {
        public class Disintegrator : T1.Beam
        {
            public Disintegrator(Vector position, Direction dir) :
                base(position, dir)
            {
                Color = Color.Purple;
                Damage = 60;
                onHits += new OnHits.ArmorMeltEffect(6);
            }
        }
    }
}
