using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Lost_boy
{
    public class BasicWeapon : IWeapon
    {
        private int reloadTime;
        private Vector launchPosition;
        private Direction shootingDirection;
        private event Action<IShip> onHits;
        private IProjectile ammoType;
        public void AppendOnHit(Action<IShip> onHit)
        {
            onHits += onHit;
        }

        public bool IsLoaded
        {
            get;
            private set;
        }

        public IProjectile GetProjectile()
        {
            IProjectile bullet = ammoType.Clone();
            bullet.AppendEffect(onHits);
            Reload();
            return bullet;
        }

        private void Reload()
        {
            Thread th = new Thread(() =>
            {
                IsLoaded = false;
                Thread.Sleep(this.reloadTime);
                IsLoaded = true;
            });
        }

        public void UpdatePosition(int dx)
        {
            launchPosition.X += dx;
        }

        public BasicWeapon(IProjectile ammo, Direction dir, Vector position)
        {
            this.shootingDirection = dir;
            this.launchPosition = position;
            this.ammoType = ammo;
            this.IsLoaded = true;
            this.reloadTime = VALUES.BASIC_WEAPON_RELOAD_TIME;
        }
    }
}
