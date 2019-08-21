using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Lost_boy
{
    public class Bonus : Mover, IProjectile
    {
        private Rectangle drawable;
        protected Color color = Color.Blue;
        public event Action<IShip> onHits;
        public event Action onDeath;

        public Direction Direction
        {
            get { return Direction.Down; }
        }

        public void AffectShip(IShip ship)
        {
            onHits(ship);
            onDeath();
        }

        public override void Move()
        {
            base.Move();
            drawable.X = Position.X;
            drawable.Y = Position.Y;
        }

        public override void Draw(Graphics g, Pen p)
        {
            p.Color = color;
            g.DrawRectangle(p, drawable);
        }

        public Bonus(Vector position, Action<IShip> e) :
            base(
            position,
            new Vector(0, VALUES.BONUS_SPEED),
            new Vector(),
            new Vector(VALUES.BONUS_SIZE, VALUES.BONUS_SIZE))
        {
            onHits += e;
            drawable = new Rectangle(position.X, position.Y, Size.X, Size.Y);
        }
    }

    public class BulletSpeedBonus : Bonus
    {
        public BulletSpeedBonus(Vector position) :
            base(position,
            ship =>
            {
                ship.Weapon.AppendOnShot(new OnShots.SpeedChange(VALUES.BONUS_VALUE));
            })
        {
            this.color = Color.Yellow;
        }
    }

    public class LaserDamageBonus : Bonus
    {
        public LaserDamageBonus(Vector position) :
            base(position,
            ship =>
            {
                ship.Weapon.Ammo.AppendDmgModifier((ref int val) =>
                {
                    val += 2;
                });
            })
        {
            this.color = Color.Red;
        }
    }

    public class BulletSizeChangeBonus : Bonus
    {
        public BulletSizeChangeBonus(Vector position) :
            base(position,
            ship =>
            {
                ship.Weapon.AppendOnShot(new OnShots.SizeChange(1));
            })
        {
            this.color = Color.Pink;
        }
    }

    public class BurnBonus : Bonus
    {
        public BurnBonus(Vector position) :
            base(position,
            ship =>
            {
                ship.Weapon.Ammo.AppendOnHit(new OnHits.BurnChance(5, 3, 20));
            })
        {
            this.color = Color.Orange;
        }
    }

    public class WeaponReloadTimeBonus : Bonus
    {
        public WeaponReloadTimeBonus(Vector position) :
            base(position,
            ship =>
            {
                ship.Weapon.ReloadTime -= 500;
            })
        {
            this.color = Color.Gray;
        }
    }

    public class HealthBonus : Bonus
    {
        public HealthBonus(Vector position) :
            base(position,
            ship =>
            {
                ((PlayerShip)ship).Heal(50);
            })
        {
            this.color = Color.Green;
        }
    }
}