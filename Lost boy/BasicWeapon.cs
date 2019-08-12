using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Lost_boy.OnShots;

namespace Lost_boy
{
    public class BasicWeapon : IWeapon
    {
        private int reloadTime;
        private event OnShot onShot;

        public void AppendOnShot(OnShot onShot)
        {
            this.onShot += onShot;
        }

        public bool IsLoaded
        {
            get;
            private set;
        }

        public IBulletFactory Ammo
        {
            get;
            set;
        }

        public IBullet GetBullet(Vector launchPosition)
        {
            IBullet bullet = Ammo.Create(launchPosition);
            if (onShot != null)
                onShot(bullet);
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

        public BasicWeapon(IBulletFactory ammo)
        {
            this.Ammo = ammo;
            this.IsLoaded = true;
            this.reloadTime = VALUES.BASIC_WEAPON_RELOAD_TIME;
        }

        public BasicWeapon(IBulletFactory ammo, List<OnShot> onHits)
        {
            foreach (var f in onHits)
            {
                this.onShot += f;
            }
            this.Ammo = ammo;
            this.IsLoaded = true;
            this.reloadTime = VALUES.BASIC_WEAPON_RELOAD_TIME;
        }
    }
}