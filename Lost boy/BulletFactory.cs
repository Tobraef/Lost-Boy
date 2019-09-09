using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Lost_boy
{
    public abstract class BulletFactory : IBulletFactory
    {
        private event Action<IShip> onHits;
        private event Modify dmgModifiers;

        public abstract int RechargeTime { get; }

        public void AppendOnHit(Action<IShip> action)
        {
            onHits += action;
        }

        public void AppendDmgModifier(Modify m)
        {
            dmgModifiers += m;
        }

        public abstract IBullet Create(Vector where);

        protected void ApplyOnHits(IBullet bullet)
        {
            bullet.onHits += this.onHits;
        }

        protected void ApplyDmgModifier(IBullet bullet)
        {
            bullet.dmgModifiers += this.dmgModifiers;
        }
    }

    public class BasicLaserFactory : BulletFactory
    {
        private Direction direction;

        public override int RechargeTime { get { return VALUES.BASIC_LASER_RECHARGE; } }

        public override IBullet Create(Vector where)
        {
            BasicLaser bullet = new BasicLaser(where, direction);
            ApplyDmgModifier(bullet);
            ApplyOnHits(bullet);
            return bullet;
        }

        public BasicLaserFactory(Direction dir)
        {
            direction = dir;
        }
    }

    public class PlasmaFactory : BulletFactory
    {
        private Direction direction;

        public override int RechargeTime { get { return VALUES.BASIC_LASER_RECHARGE + VALUES.BASIC_LASER_RECHARGE / 2; } }

        public override IBullet Create(Vector where)
        {
            PlasmaBullet bullet = new PlasmaBullet(where, direction);
            ApplyDmgModifier(bullet);
            ApplyOnHits(bullet);
            return bullet;
        }

        public PlasmaFactory(Direction dir)
        {
            direction = dir;
        }
    }

    public class ExplosiveBulletFactory : BulletFactory
    {
        private Direction direction;
        public Action<IProjectile> BulletAdder
        {
            set;
            private get;
        }

        public override int RechargeTime { get { return VALUES.BASIC_LASER_RECHARGE * 5; } }

        public override IBullet Create(Vector where)
        {
            ExplosiveBullet bullet = new ExplosiveBullet(BulletAdder, direction, where);
            ApplyDmgModifier(bullet);
            ApplyOnHits(bullet);
            return bullet;
        }

        public ExplosiveBulletFactory(Direction dir)
        {
            this.direction = dir;
        }
    }

    public class FrostyLaserFactory : BulletFactory
    {
        private Direction direction;

        public override int RechargeTime { get { return VALUES.BASIC_LASER_RECHARGE; } }

        public override IBullet Create(Vector where)
        {
            BasicLaser bullet = new BasicLaser(where, direction);
            ApplyDmgModifier(bullet);
            ApplyOnHits(bullet);
            bullet.Color = Color.Blue;
            return bullet;
        }

        public FrostyLaserFactory(Direction dir)
        {
            direction = dir;
            this.AppendOnHit(s => s.MaxSpeed -= 3);
            this.AppendDmgModifier((ref int i) => i += 5);
        }
    }
}