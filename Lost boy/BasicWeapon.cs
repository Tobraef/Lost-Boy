using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Lost_boy.OnShots;

namespace Lost_boy
{
    public abstract class Weapon : IWeapon
    {
        private event OnShot onShot;
        protected List<IProjectile> recycledShots = new List<IProjectile>();
        public virtual Action<IBullet> BulletAdder
        {
            protected get;
            set;
        }

        public virtual Action<IBullet> RecycledBulletAdder
        {
            protected get;
            set;
        }

        private bool isLoaded;

        public void AppendOnShot(OnShot onShot)
        {
            this.onShot += onShot;
        }

        public IBulletFactory Ammo
        {
            get;
            set;
        }

        public int ReloadTime
        {
            get;
            set;
        }

        public void Cleanup()
        {
            recycledShots = new List<IProjectile>();
            BulletAdder = null;
            RecycledBulletAdder = null;
            if (Ammo is ExplosiveBulletFactory)
            {
                ((ExplosiveBulletFactory)Ammo).BulletAdder = null;
            }
        }

        public void PullTheTrigger(Vector launchPosition)
        {
            if (isLoaded)
            {
                AddBullets(launchPosition);
                Reload();
            }
        }

        protected void ImbueBullet(IBullet bullet)
        {
            if (onShot != null)
                onShot(bullet);
            bullet.OnRecycle += RecyclingMethod;
            bullet.onDeath += bullet.Recycle;
        }

        protected abstract void AddBullets(Vector launchPosition);

        private void RecyclingMethod(IProjectile bullet)
        {
            recycledShots.Add(bullet);
        }

        private void Reload()
        {
            Thread th = new Thread(() =>
            {
                isLoaded = false;
                Thread.Sleep(this.ReloadTime + this.Ammo.RechargeTime);
                isLoaded = true;
            });
            th.Start();
        }

        public Weapon(IBulletFactory ammo)
        {
            this.Ammo = ammo;
            this.isLoaded = true;
            this.ReloadTime = VALUES.BASIC_WEAPON_RELOAD_TIME;
        }
    }

    public class SingleWeapon : Weapon
    {
        public override Action<IBullet> BulletAdder
        {
            protected get;
            set;
        }

        protected override void AddBullets(Vector launchPosition)
        {
            IBullet bullet;
            if (recycledShots.Count > 0)
            {
                bullet = (IBullet)recycledShots.Last();
                bullet.Position = launchPosition;
                recycledShots.RemoveAt(recycledShots.Count - 1);
                RecycledBulletAdder(bullet);
            }
            else
            {
                bullet = Ammo.Create(launchPosition);
                ImbueBullet(bullet);
                BulletAdder(bullet);
            }
        }

        public SingleWeapon(IBulletFactory ammo) :
            base(ammo)
        { }
    }

    public class DoubleWeapon : Weapon
    {
        public override Action<IBullet> BulletAdder
        {
            protected get;
            set;
        }

        private void SetBulletsPosition(IBullet leftBullet, IBullet rightBullet, Vector launchPosition)
        {
            leftBullet.Position = new Vector(launchPosition.X - leftBullet.Size.X, launchPosition.Y);
            rightBullet.Position = new Vector(launchPosition.X + rightBullet.Size.X, launchPosition.Y);
        }

        protected override void AddBullets(Vector launchPosition)
        {
            IBullet leftBullet;
            IBullet rightBullet;
            if (recycledShots.Count > 1)
            {
                leftBullet = (IBullet)recycledShots[recycledShots.Count - 1];
                rightBullet = (IBullet)recycledShots[recycledShots.Count - 2];
                recycledShots.RemoveRange(recycledShots.Count - 2, 2);
                SetBulletsPosition(leftBullet, rightBullet, launchPosition);
                RecycledBulletAdder(leftBullet);
                RecycledBulletAdder(rightBullet);
            }
            else
            {
                leftBullet = Ammo.Create(launchPosition);
                rightBullet = Ammo.Create(launchPosition);
                ImbueBullet(leftBullet);
                ImbueBullet(rightBullet);
                SetBulletsPosition(leftBullet, rightBullet, launchPosition);
                BulletAdder(leftBullet);
                BulletAdder(rightBullet);
            }
        }

        public DoubleWeapon(IBulletFactory ammo) :
            base(ammo)
        {
            ReloadTime *= 3;
            ReloadTime /= 2;
        }
    }

    public class SprayWeapon : Weapon
    {
        public override Action<IBullet> BulletAdder
        {
            protected get;
            set;
        }

        protected override void AddBullets(Vector launchPosition)
        {
            IBullet bullet;
            if (recycledShots.Count > 0)
            {
                bullet = (IBullet)recycledShots.Last();
                bullet.Position = launchPosition;
                recycledShots.RemoveAt(recycledShots.Count - 1);
                RecycledBulletAdder(bullet);
            }
            else
            {
                bullet = Ammo.Create(launchPosition);
                ImbueBullet(bullet);
                BulletAdder(bullet);
            }
        }

        public SprayWeapon(IBulletFactory ammo) :
            base(ammo)
        {
            AppendOnShot(bullet =>
            {
                ReloadTime = VALUES.BASIC_WEAPON_RELOAD_TIME / 2;
                bullet.Speed = new Vector(VALUES.random.Next(-15, 15), bullet.Speed.Y);
            });
        }
    }

    public class TripleWeapon : Weapon
    {
        public override Action<IBullet> BulletAdder
        {
            protected get;
            set;
        }

        protected override void AddBullets(Vector launchPosition)
        {
            IBullet leftBullet;
            IBullet middleBullet;
            IBullet rightBullet;
            if (recycledShots.Count > 2)
            {
                leftBullet = (IBullet)recycledShots[recycledShots.Count - 1];
                middleBullet = (IBullet)recycledShots[recycledShots.Count - 2];
                rightBullet = (IBullet)recycledShots[recycledShots.Count - 3];
                foreach (var b in recycledShots.Take(3)) b.Position = launchPosition;
                recycledShots.RemoveRange(recycledShots.Count - 3, 3);
                leftBullet.Speed = new Vector(-5, leftBullet.Speed.Y);
                rightBullet.Speed = new Vector(5, rightBullet.Speed.Y);
                RecycledBulletAdder(leftBullet);
                RecycledBulletAdder(middleBullet);
                RecycledBulletAdder(rightBullet);
            }
            else
            {
                leftBullet = Ammo.Create(launchPosition);
                middleBullet = Ammo.Create(launchPosition);
                rightBullet = Ammo.Create(launchPosition);
                leftBullet.Speed = new Vector(-5, leftBullet.Speed.Y);
                rightBullet.Speed = new Vector(5, rightBullet.Speed.Y);
                ImbueBullet(leftBullet);
                ImbueBullet(middleBullet);
                ImbueBullet(rightBullet);
                BulletAdder(leftBullet);
                BulletAdder(middleBullet);
                BulletAdder(rightBullet);
            }
        }

        public TripleWeapon(IBulletFactory ammo) :
            base(ammo)
        {
            ReloadTime *= 2;
        }
    }
}