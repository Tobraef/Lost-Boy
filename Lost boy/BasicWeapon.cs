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
        public virtual Action<IBullet> BulletAdder
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
        }

        protected abstract void AddBullets(Vector launchPosition);

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
            IBullet bullet = Ammo.Create(launchPosition);
            ImbueBullet(bullet);
            BulletAdder(bullet);
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

        protected override void AddBullets(Vector launchPosition)
        {
            IBullet leftBullet = Ammo.Create(launchPosition);
            IBullet rightBullet = Ammo.Create(launchPosition);
            leftBullet.Position = new Vector(leftBullet.Position.X - leftBullet.Size.X, leftBullet.Position.Y);
            rightBullet.Position = new Vector(rightBullet.Position.X + rightBullet.Size.X, rightBullet.Position.Y);
            ImbueBullet(leftBullet);
            ImbueBullet(rightBullet);
            BulletAdder(leftBullet);
            BulletAdder(rightBullet);
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
            IBullet bullet = Ammo.Create(launchPosition);
            ImbueBullet(bullet);
            BulletAdder(bullet);
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
            IBullet leftBullet = Ammo.Create(launchPosition);
            IBullet middleBullet = Ammo.Create(launchPosition);
            IBullet rightBullet = Ammo.Create(launchPosition);
            leftBullet.Speed = new Vector(-5, leftBullet.Speed.Y);
            rightBullet.Speed = new Vector(5, rightBullet.Speed.Y);
            ImbueBullet(leftBullet);
            ImbueBullet(middleBullet);
            ImbueBullet(rightBullet);
            BulletAdder(leftBullet);
            BulletAdder(middleBullet);
            BulletAdder(rightBullet);
        }

        public TripleWeapon(IBulletFactory ammo) :
            base(ammo)
        {
            ReloadTime *= 2;
        }
    }
}