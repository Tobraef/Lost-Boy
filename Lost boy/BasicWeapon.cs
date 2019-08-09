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

        public IBullet Ammo
        {
            set;
            get;
        }

        public IBullet GetBullet(Vector launchPosition)
        {
            Ammo.Position = launchPosition;
            IBullet bullet = Ammo.Clone();
            // TODO remove when onShot always holding an event
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

        public BasicWeapon(IBullet ammo)
        {
            this.Ammo = ammo;
            this.IsLoaded = true;
            this.reloadTime = VALUES.BASIC_WEAPON_RELOAD_TIME;
        }

        public BasicWeapon(IBullet ammo, List<OnShot> onHits)
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