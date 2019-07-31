using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Lost_boy
{
    public class BasicWeapon : IWeapon
    {
        private int reloadTime;
        private event Action<IProjectile> onShot;
        private IProjectile ammoType;
        public void AppendOnShot(Action<IProjectile> onShot)
        {
            this.onShot += onShot;
        }

        public bool IsLoaded
        {
            get;
            private set;
        }

        public void SetAmmo(IProjectile ammo)
        {
            this.ammoType = ammo;
        }

        public IProjectile GetProjectile(Vector launchPosition)
        {
            ammoType.Position = launchPosition;
            IProjectile bullet = ammoType.Clone();
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

        public BasicWeapon(IProjectile ammo)
        {
            this.SetAmmo(ammo);
            this.IsLoaded = true;
            this.reloadTime = VALUES.BASIC_WEAPON_RELOAD_TIME;
        }

        public BasicWeapon(IProjectile ammo, List<Action<IProjectile>> onHits)
        {
            foreach(var f in onHits)
            {
                this.onShot += f;
            }
            this.SetAmmo(ammo);
            this.IsLoaded = true;
            this.reloadTime = VALUES.BASIC_WEAPON_RELOAD_TIME;
        }
    }
}