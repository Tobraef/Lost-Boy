using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lost_boy
{
    public delegate void Modify(ref int i);
    public class PlayerShip : Mover, IShip
    {
        private event Action<Vector> onMove;
        private event Modify onDamage;

        public IWeapon Weapon
        {
            get;
            set;
        }

        public int Health
        {
            get;
            set;
        }

        public int Defence
        {
            get;
            set;
        }

        public void TakeDamage(int val)
        {
            onDamage(ref val);
            this.Health -= val;
        }

        public void Shoot(Action<IProjectile> bulletAdder)
        {
            if (Weapon.IsLoaded)
                bulletAdder(Weapon.GetProjectile());
        }

        public bool IsHit(IProjectile projectile)
        {
            throw new NotImplementedException();
        }
    }
}
