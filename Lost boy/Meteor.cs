using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Lost_boy.Meteor
{
    public class Meteor : Mover, IProjectile, IBody
    {
        private int damage;
        public event Action<IShip> onHits;
        public event Action<IProjectile> OnRecycle;
        public event Action onDeath;

        public Direction Direction
        {
            get;
            private set;
        }

        public void AffectShip(IShip ship)
        {
            ship.TakeDamage(damage);
            onHits(ship);
            onDeath();
        }
        
        public void Recycle()
        {
            OnRecycle(this);
        }

        public override void Draw(System.Drawing.Graphics g, System.Drawing.Pen p)
        {
            p.Color = Color.Brown;
            g.DrawEllipse(p, Position.X, Position.Y, Size.X, Size.Y);
        }

        public int MaxHealth
        {
            get;
            set;
        }

        public int Health
        {
            get;
            private set;
        }

        public int Defence
        {
            get;
            set;
        }

        public void TakeDamage(int val)
        {
            val -= Defence;
            if (val > 0)
            {
                Health -= val;
                if (Health < 0)
                    onDeath();
            }
        }

        public void TakeTrueDamage(int val)
        {
            Health -= val;
            if (Health < 0)
                onDeath();
        }

        public void Heal(int val)
        {
            Health += val;
            if (Health > MaxHealth)
                Health = MaxHealth;
        }

        public Meteor(Vector position, Vector speed, Vector size, int dmg) :
            base(position, speed, new Vector(), size)
        {
            Direction = Direction.Down;
            onHits += new OnHits.SlowEffect(2);
            damage = dmg;
        }
    }
}
