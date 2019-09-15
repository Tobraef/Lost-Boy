using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Lost_boy.BulletFactory
{
    using Ammo;
    public abstract class BulletFactory : IBulletFactory
    {
        private event Action<IShip> onHits;
        private event Modify dmgModifiers;

        public Func<IBullet> Creator
        {
            set;
            get;
        }

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

        public int Price { get { 
            return 
                10 + 
                dmgModifiers.GetInvocationList().Length * 10 + 
                onHits.GetInvocationList().Length * 15; } }

        public void Equip(PlayerShip player)
        {
            player.Backpack.Add(player.Weapon.Ammo);
            player.Weapon.Ammo = this;
            player.Backpack.Remove(this);
        }

        public void AddToInventory(PlayerShip player)
        {
            player.Backpack.Add(this);
        }
    }
    namespace T1
    {
        using Ammo.T1;
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

        public class ExplosiveBulletFactory : BulletFactory, IExplosiveFactory
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

            public override int RechargeTime { get { return VALUES.BASIC_LASER_RECHARGE * 7/5; } }

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
                this.AppendOnHit(s => s.MaxSpeed -= 1);
                this.AppendDmgModifier((ref int i) => i += 5);
            }
        }

        public class BeamFactory : BulletFactory
        {
            private Direction direction;

            public override int RechargeTime { get { return VALUES.BASIC_LASER_RECHARGE * 3; } }

            public override IBullet Create(Vector where)
            {
                Beam bullet = new Beam(where, direction);
                ApplyDmgModifier(bullet);
                ApplyOnHits(bullet);
                return bullet;
            }

            public BeamFactory(Direction dir)
            {
                direction = dir;
                this.AppendOnHit(new OnHits.ArmorMeltEffect(10));
            }
        }
    }

    namespace T2
    {
        using Ammo.T2;
        public class HellHotFactory : BulletFactory
        {
            private Direction direction;

            public override int RechargeTime { get { return VALUES.BASIC_LASER_RECHARGE * 4 / 5; } }

            public override IBullet Create(Vector where)
            {
                HellHotLaser bullet = new HellHotLaser(where, direction);
                ApplyDmgModifier(bullet);
                ApplyOnHits(bullet);
                return bullet;
            }

            public HellHotFactory(Direction dir)
            {
                direction = dir;
            }
        }

        public class StarPlasmaFactory : BulletFactory
        {
            private Direction direction;

            public override int RechargeTime { get { return VALUES.BASIC_LASER_RECHARGE * 3 / 2; } }

            public override IBullet Create(Vector where)
            {
                StarPlasma bullet = new StarPlasma(where, direction);
                ApplyDmgModifier(bullet);
                ApplyOnHits(bullet);
                return bullet;
            }

            public StarPlasmaFactory(Direction dir)
            {
                direction = dir;
            }
        }

        public class NapalmFactory : BulletFactory, IExplosiveFactory
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
                Napalm bullet = new Napalm(BulletAdder, direction, where);
                ApplyDmgModifier(bullet);
                ApplyOnHits(bullet);
                return bullet;
            }

            public NapalmFactory(Direction dir)
            {
                this.direction = dir;
            }
        }

        public class IcyLaserFactory : BulletFactory
        {
            private Direction direction;

            public override int RechargeTime { get { return VALUES.BASIC_LASER_RECHARGE; } }

            public override IBullet Create(Vector where)
            {
                Ammo.T1.BasicLaser bullet = new Ammo.T1.BasicLaser(where, direction);
                ApplyDmgModifier(bullet);
                ApplyOnHits(bullet);
                bullet.Color = Color.Blue;
                return bullet;
            }

            public IcyLaserFactory(Direction dir)
            {
                direction = dir;
                this.AppendOnHit(s => s.MaxSpeed -= 2);
                this.AppendDmgModifier((ref int i) => i += 10);
            }
        }

        public class MortalCoilFactory : BulletFactory
        {
            private Direction direction;

            public override int RechargeTime { get { return VALUES.BASIC_LASER_RECHARGE * 3; } }

            public override IBullet Create(Vector where)
            {
                MortalCoil bullet = new MortalCoil(where, direction);
                ApplyDmgModifier(bullet);
                ApplyOnHits(bullet);
                return bullet;
            }

            public MortalCoilFactory(Direction dir)
            {
                direction = dir;
                this.AppendOnHit(new OnHits.ArmorMeltEffect(10));
            }
        }
    }

    namespace T3
    {
        using Ammo.T3;
        public class AnnihilatorFactory : BulletFactory
        {
            private Direction direction;

            public override int RechargeTime { get { return VALUES.BASIC_LASER_RECHARGE * 3 / 5; } }

            public override IBullet Create(Vector where)
            {
                Annihilator bullet = new Annihilator(where, direction);
                ApplyDmgModifier(bullet);
                ApplyOnHits(bullet);
                return bullet;
            }

            public AnnihilatorFactory(Direction dir)
            {
                direction = dir;
            }
        }

        public class DecimatorFactory : BulletFactory
        {
            private Direction direction;

            public override int RechargeTime { get { return VALUES.BASIC_LASER_RECHARGE * 3 / 2; } }

            public override IBullet Create(Vector where)
            {
                Decimator bullet = new Decimator(where, direction);
                ApplyDmgModifier(bullet);
                ApplyOnHits(bullet);
                return bullet;
            }

            public DecimatorFactory(Direction dir)
            {
                direction = dir;
            }
        }

        public class ArmaggedonFactory : BulletFactory
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
                Armaggedon bullet = new Armaggedon(BulletAdder, direction, where);
                ApplyDmgModifier(bullet);
                ApplyOnHits(bullet);
                return bullet;
            }

            public ArmaggedonFactory(Direction dir)
            {
                this.direction = dir;
            }
        }

        public class DisintegratorFactory : BulletFactory
        {
            private Direction direction;

            public override int RechargeTime { get { return VALUES.BASIC_LASER_RECHARGE * 3; } }

            public override IBullet Create(Vector where)
            {
                Disintegrator bullet = new Disintegrator(where, direction);
                ApplyDmgModifier(bullet);
                ApplyOnHits(bullet);
                return bullet;
            }

            public DisintegratorFactory(Direction dir)
            {
                direction = dir;
                this.AppendOnHit(new OnHits.ArmorMeltEffect(10));
            }
        }
    }
}