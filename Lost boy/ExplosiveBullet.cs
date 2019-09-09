using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Lost_boy
{
    class ExplosiveBullet : Bullet
    {
        Action<IProjectile> BulletAdder;
        private int explosionDamage;
        private Vector size;
        public override event Action onDeath;
        private Rectangle drawable;

        public override Vector Size
        {
            get
            {
                return size;
            }
            set
            {
                drawable.Width = value.X;
                drawable.Height = value.Y;
                size = value;
            }
        }

        private class Explosion : IProjectile
        {
            private bool HasBlown = false;
            private IEffect burn = new BurnEffect(10, 3);
            private int damage;
            public Direction Direction
            {
                get;
                private set;
            }

            public event Action<IShip> onHits;

            public void AffectShip(IShip ship)
            {
                ship.TakeDamage(damage);
                onHits(ship);
            }

            public event Action onDeath;
            public event Action<IProjectile> OnRecycle;

            public void Recycle()
            {
                OnRecycle(this);
            }

            public Vector Position
            {
                get;
                set;
            }

            public Vector Speed
            {
                get;
                set;
            }

            public Vector Acceleration
            {
                get;
                set;
            }

            public Vector Size
            {
                get;
                set;
            }

            public void Move()
            {
                if (HasBlown)
                    onDeath();
                HasBlown = true;
            }

            public void Draw(Graphics g, Pen p)
            {
                p.Color = Color.Red;
                g.DrawEllipse(p, Position.X, Position.Y, Size.X, Size.Y);
            }

            public Explosion(int dmg, Vector position, int radius)
            {
                onHits += burn.WrappedAction;
                damage = dmg;
                Position = position;
                Size = new Vector(radius, radius);
            }
        }

        private void Explode()
        {
            BulletAdder(new Explosion(explosionDamage, new Vector(
                this.Position.X - 3*this.Size.X,
                this.Position.Y - 3*this.Size.Y),
                this.Size.Y * 8));
        }

        public override void AffectShip(IShip ship)
        {
            base.AffectShip(ship);
            Explode();
            onDeath();
        }

        public override void Draw(Graphics g, Pen p)
        {
            p.Color = Color;
            g.DrawRectangle(p, drawable);
        }

        public override void Move()
        {
            base.Move();
            drawable.X = this.Position.X;
            drawable.Y = this.Position.Y;
        }

        public ExplosiveBullet(Action<IProjectile> bulletAdder, Direction dir, Vector position) :
            base(position, new Vector(25, 25), dir, VALUES.BASIC_LASER_SPEED /2, 30)
        {
            this.explosionDamage = 25;
            this.BulletAdder = bulletAdder;
            this.Color = Color.AliceBlue;
        }
    }
}
