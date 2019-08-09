using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;

namespace Lost_boy
{
    public class Bonus : Mover, IProjectile
    {
        private event Action<IShip> effects;
        private Rectangle drawable;
        private readonly Color color = Color.Blue;

        public IProjectile Clone()
        { // LOL, GOOD CODING
            return null;
        }

        public void AppendEffect(Action<IShip> e)
        {
            effects += e;
        }

        public Action TresholdPass
        {
            private get;
            set;
        }

        public void AffectShip(IShip ship)
        {
            effects(ship);
        }

        public override void Move()
        {
            base.Move();
            if (Position.Y > VALUES.HEIGHT || Position.Y < 0)
                TresholdPass();
            drawable.X = Position.X;
            drawable.Y = Position.Y;
        }

        public override void Draw(Graphics g, Pen p)
        {
            p.Color = color;
            g.DrawRectangle(p, drawable);
        }

        public Bonus(Vector position, Action<IShip> e) :
            base (
            position,
            new Vector(0, VALUES.BONUS_SPEED),
            new Vector(),
            new Vector(VALUES.BONUS_SIZE, VALUES.BONUS_SIZE))
        {
            effects += e;
            drawable = new Rectangle(position.X, position.Y, Size.X, Size.Y);
        }
    }

    public class WeaponSpeedBonus : Bonus
    {
        public WeaponSpeedBonus(Vector position) :
            base(position, 
            ship =>
            {
                ship.Weapon.AppendOnShot(new OnShots.SpeedChange(VALUES.BONUS_VALUE));
            })
        { }
    }

    public class LaserDamageBonus : Bonus
    {
        public LaserDamageBonus(Vector position) :
            base(position,
            ship =>
            {
                ship.Weapon.Ammo.AppendDmgModifier((ref int val) =>
                {
                    Console.WriteLine("Before : {0}", val);
                    val += 10;
                    Console.WriteLine("After : {0}", val);
                });
            })
        { }
    }
}
