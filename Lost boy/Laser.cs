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
        protected event Action<IShip> shipEffects;

        public void AppendEffect(Action<IShip> e)
        {
            shipEffects += e;
        }

        public TresholdPass TresholdPass
        {
            private get;
            set;
        }
        
        public void AffectShip(IShip ship)
        {
            shipEffects(ship);
        }

        public override void Move()
        {
            base.Move();
            if (Position.Y > VALUES.HEIGHT)
                TresholdPass();
        }

        public Laser(Vector position, Direction dir, int speed, int damage) :
            base(position, new Vector(0, (int)dir*speed), new Vector(0,0))
        {
            shipEffects += new DamageEffect(damage).WrappedAction;
        }
    }
}
