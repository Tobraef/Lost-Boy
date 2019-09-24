using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Lost_boy.OnHits;

namespace Lost_boy.Ammo
{
    namespace T1
    {
        public class BasicLaser : Bullet
        {
            public override event Action onDeath;
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

            public override void AffectShip(IShip ship)
            {
                base.AffectShip(ship);
                onDeath();
            }

            public override void Draw(Graphics g, Pen p)
            {
                p.Color = Color;
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
                this.Color = Color.Peru;
                this.size = Size;
                this.direction = dir;
                drawable = new Rectangle(Position.X, Position.Y, size.X, size.Y);
            }
        }
    }

   namespace T2
   {
       public class HellHotLaser : T1.BasicLaser
       {
           public HellHotLaser(Vector position, Direction dir) :
               base(position, dir)
           {
               Size = new Vector(Size.X + 2, Size.Y + 4);
               Speed = new Vector(Speed.X, Speed.Y + 5);
               Damage += 5;
               onHits += new OnHits.BurnChance(10, 3, 25);
           }
       }
   }

    namespace T3
    {
        public class Annihilator : T1.BasicLaser
        {
            public override event Modify dmgModifiers;
            public override event Action<IShip> onHits;

            public override void AffectShip(IShip ship)
            {
                int dmg = Damage;
                if (dmgModifiers != null)
                dmgModifiers(ref dmg);
                onHits(ship);
                ship.TakeTrueDamage(dmg);
            }

            public Annihilator(Vector position, Direction dir) :
                base(position, dir)
            {
                Damage += 10;
                onHits += new OnHits.BurnChance(10, 3, 25);
                Speed = new Vector(Speed.X, Speed.Y + 12);
                Size = new Vector(Size.X + 2, Size.Y * 2);
            }
        }
    }
}