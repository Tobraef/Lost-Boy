using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Lost_boy
{
    public abstract class Bullet : Mover, IBullet
    {
        public virtual event Action onDeath;
        public event Action<IProjectile> OnRecycle;
        public event Action<IShip> onHits;
        public event Modify dmgModifiers;
        protected Direction direction;

        public Color Color
        {
            get;
            set;
        }

        public Direction Direction
        {
            get { return direction; }
        }

        public int Damage
        {
            get;
            set;
        }

        public virtual void AffectShip(IShip ship)
        {
            if (onHits != null)
                onHits(ship);
            int dmg = Damage;
            if (dmgModifiers != null)
                dmgModifiers(ref dmg);
            ship.TakeDamage(dmg);
        }

        public void Recycle()
        {
            OnRecycle(this);
        }

        public Bullet(Vector position, Vector size, Direction dir, int speed, int damage) :
            base(position, new Vector(0, (int)dir * speed), new Vector(0, 0), size)
        {
            this.Damage = damage;
        }
    }
}