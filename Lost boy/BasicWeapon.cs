using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Lost_boy.OnShots;

namespace Lost_boy.Weapon
{
    public abstract class Weapon : IWeapon, IEquipable
    {
        private event OnShot onShot;
        protected List<IProjectile> recycledShots = new List<IProjectile>();
        private Action<IProjectile> bulletAdder;


        public Action<IProjectile> BulletAdder
        {
            protected get { return bulletAdder; }
            set
            {
                if (Ammo is IExplosiveFactory)
                    ((IExplosiveFactory)Ammo).BulletAdder = value;
                bulletAdder = value;
            }
        }

        public Action<IProjectile> RecycledBulletAdder
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
            if (Ammo is IExplosiveFactory)
            {
                ((IExplosiveFactory)Ammo).BulletAdder = null;
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

        public int Price
        {
            get
            {
                int price = 10;
                if (onShot != null)
                    price += onShot.GetInvocationList().Length * 15;
                return price;
            }
        }

        public void EquipOn(PlayerShip player)
        {
            Ammo = player.Weapon.Ammo;
            player.Backpack.Add((IEquipable)player.Weapon);
            player.Weapon = this;
            player.Backpack.Remove(this);
        }

        public void SellFrom(IHolder holder)
        {
            holder.Backpack.Remove(this);
            holder.Gold += Price;
        }

        public void AddToInventory(IHolder player)
        {
            player.Backpack.Add(this);
        }

        public Weapon(IBulletFactory ammo)
        {
            this.Ammo = ammo;
            this.isLoaded = true;
            this.ReloadTime = VALUES.BASIC_WEAPON_RELOAD_TIME;
        }
    }

    namespace T1
    {
        public class SingleWeapon : Weapon
        {
            protected override void AddBullets(Vector launchPosition)
            {
                IBullet bullet;
                if (recycledShots.Count > 0)
                {
                    bullet = (IBullet)recycledShots.Last();
                    launchPosition.Y += bullet.Size.Y * (int)bullet.Direction;
                    bullet.Position = launchPosition;
                    recycledShots.RemoveAt(recycledShots.Count - 1);
                    RecycledBulletAdder(bullet);
                }
                else
                {
                    bullet = Ammo.Create(launchPosition);
                    launchPosition.Y += bullet.Size.Y * (int)bullet.Direction;
                    bullet.Position = launchPosition;
                    ImbueBullet(bullet);
                    BulletAdder(bullet);
                }
            }

            public override string ToString()
            {
                return "Single weapon";
            }

            public SingleWeapon(IBulletFactory ammo) :
                base(ammo)
            { }
        }

        public class SprayWeapon : Weapon
        {
            protected override void AddBullets(Vector launchPosition)
            {
                IBullet bullet;
                if (recycledShots.Count > 0)
                {
                    bullet = (IBullet)recycledShots.Last();
                    launchPosition.Y += bullet.Size.Y * (int)bullet.Direction;
                    bullet.Position = launchPosition;
                    recycledShots.RemoveAt(recycledShots.Count - 1);
                    RecycledBulletAdder(bullet);
                }
                else
                {
                    bullet = Ammo.Create(launchPosition);
                    launchPosition.Y += bullet.Size.Y * (int)bullet.Direction;
                    bullet.Position = launchPosition;
                    ImbueBullet(bullet);
                    BulletAdder(bullet);
                }
            }

            public override string ToString()
            {
                return "Spray weapon";
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
    }

    namespace T2
    {
        public class DoubleWeapon : Weapon
        {
            private void SetBulletsPosition(IBullet leftBullet, IBullet rightBullet, Vector launchPosition)
            {
                launchPosition.Y += leftBullet.Size.Y * (int)leftBullet.Direction;
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

            public override string ToString()
            {
                return "Double weapon";
            }

            public DoubleWeapon(IBulletFactory ammo) :
                base(ammo)
            {
                ReloadTime *= 3;
                ReloadTime /= 2;
            }
        }
    }

    namespace T3
    {
        public class TripleWeapon : Weapon
        {
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
                    launchPosition.Y += leftBullet.Size.Y * (int)leftBullet.Direction;
                    leftBullet.Position = launchPosition; 
                    middleBullet.Position = launchPosition;
                    rightBullet.Position = launchPosition;
                    recycledShots.RemoveRange(recycledShots.Count - 3, 3);
                    leftBullet.Speed = new Vector(-5, leftBullet.Speed.Y);
                    middleBullet.Speed = new Vector(0, middleBullet.Speed.Y);
                    rightBullet.Speed = new Vector(5, rightBullet.Speed.Y);
                    RecycledBulletAdder(leftBullet);
                    RecycledBulletAdder(middleBullet);
                    RecycledBulletAdder(rightBullet);
                }
                else
                {
                    leftBullet = Ammo.Create(launchPosition);
                    launchPosition.Y += leftBullet.Size.Y * (int)leftBullet.Direction;
                    leftBullet.Position = launchPosition;
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

            public override string ToString()
            {
                return "Triple weapon";
            }

            public TripleWeapon(IBulletFactory ammo) :
                base(ammo)
            {
                ReloadTime *= 2;
            }
        }
    }
}