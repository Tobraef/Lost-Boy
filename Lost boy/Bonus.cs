using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Lost_boy
{
    public abstract class Bonus : Mover, IProjectile
    {
        public event Action<IProjectile> OnRecycle;
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

        public abstract Bonus Clone(Vector position);

        public void Recycle()
        {
            OnRecycle(this);
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
        public override Bonus Clone(Vector position)
        {
            return new BulletSpeedBonus(position);
        }

        public BulletSpeedBonus(Vector position) :
            base(position,
            ship =>
            {
                ship.Weapon.AppendOnShot(new OnShots.SpeedChange(5));
            })
        {
            this.color = Color.Yellow;
        }
    }

    public class LaserDamageBonus : Bonus
    {
        public override Bonus Clone(Vector position)
        {
            return new LaserDamageBonus(position);
        }

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
        public override Bonus Clone(Vector position)
        {
            return new BulletSizeChangeBonus(position);
        }

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
        public override Bonus Clone(Vector position)
        {
            return new BurnBonus(position);
        }

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
        public override Bonus Clone(Vector position)
        {
            return new WeaponReloadTimeBonus(position);
        }

        public WeaponReloadTimeBonus(Vector position) :
            base(position,
            ship =>
            {
                ship.Weapon.ReloadTime -= 100;
                if (ship.Weapon.ReloadTime < 50)
                {
                    ship.Weapon.ReloadTime = 50;
                }
            })
        {
            this.color = Color.Gray;
        }
    }

    public class HealthBonus : Bonus
    {
        public override Bonus Clone(Vector position)
        {
            return new HealthBonus(position);
        }

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

    public class ShipSpeedBonus : Bonus
    {
        public override Bonus Clone(Vector position)
        {
            return new ShipSpeedBonus(position);
        }

        public ShipSpeedBonus(Vector position) :
            base(position,
                ship =>
                {
                    ship.MaxSpeed += 2;
                })
        {
            this.color = Color.Maroon;
        }
    }

    public class ArmorMeltBonus : Bonus
    {
        public override Bonus Clone(Vector position)
        {
            return new ArmorMeltBonus(position);
        }

        public ArmorMeltBonus(Vector position) :
            base(position,
                ship =>
                {
                    ship.Weapon.Ammo.AppendOnHit(new OnHits.ArmorMeltEffect(2));
                })
        { }
    }

    public class FrostBonus : Bonus
    {
        public override Bonus Clone(Vector position)
        {
            return new FrostBonus(position);
        }

        public FrostBonus(Vector position) :
            base(position,
                ship =>
                {
                    ship.Weapon.Ammo.AppendOnHit(new OnHits.SlowEffect(5));
                })
        { }
    }
}