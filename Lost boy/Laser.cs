using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Lost_boy
{
    public abstract class Laser : Mover, IBullet
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

        public Action TresholdPass
        {
            private get;
            set;
        }

        public void AffectShip(IShip ship)
        {
            shipEffects(ship);
            int modifiedDmg = dmg;
            Console.WriteLine("Modify is null : {0}", dmgModifiers == null);
            if (dmgModifiers != null)
                dmgModifiers(ref modifiedDmg);
            ship.TakeDamage(modifiedDmg);
        }
        
        public override void Move()
        {
            base.Move();
            if (Position.Y > VALUES.HEIGHT || Position.Y < 0)
                TresholdPass();
        }

        public abstract IBullet Clone();

        public Laser(Vector position, Vector size, Direction dir, int speed, int damage) :
            base(position, new Vector(0, (int)dir * speed), new Vector(0, 0), size)
        {
            this.dmg = damage;
        }
    }
}