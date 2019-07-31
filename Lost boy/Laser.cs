using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Lost_boy
{
    public abstract class Laser : Mover, IProjectile
    {
        private event Action<IShip> shipEffects;
        private event Modify dmgModifiers;
        private int dmg;
        protected Direction direction;

        public void AppendEffect(Action<IShip> e)
        {
            shipEffects += e;
        }

        public virtual void AppendDmgModifier(Modify modifier)
        {
            dmgModifiers += modifier;
        }

        public TresholdPass TresholdPass
        {
            private get;
            set;
        }

        public void AffectShip(IShip ship)
        {
            shipEffects(ship);
            int modifiedDmg = dmg;
            dmgModifiers(ref modifiedDmg);
            ship.TakeDamage(modifiedDmg);
        }

        public override void Move()
        {
            base.Move();
            if (Position.Y > VALUES.HEIGHT)
                TresholdPass();
        }

        public abstract IProjectile Clone();

        public Laser(Vector position, Vector size, Direction dir, int speed, int damage) :
            base(position, new Vector(0, (int)dir * speed), new Vector(0, 0), size)
        {
            this.dmg = damage;
        }
    }
}