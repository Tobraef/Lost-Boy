using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lost_boy
{
    public abstract class BulletFactory : IBulletFactory
    {
        private event Action<IShip> onHits;
        private event Modify dmgModifiers;

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
    
}
