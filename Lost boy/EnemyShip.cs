using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Lost_boy
{
    public class EnemyShip : Mover, IShip
    {
        private event Modify onDamageTaken;
        private Rectangle rectangle;
        private Color color = Color.Red;
        private IMovementStrategy strategy = null;
        private HPBar hpBar;

        public event Action<EnemyShip> OnDeath;

        public Random ShootingChance
        {
            private get;
            set;
        }

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

        private Vector ShootingPosition
        {
            get
            {
                return new Vector(Position.X + Size.X / 2, Position.Y + Size.Y);
            }
        }
        public void TakeDamage(int val)
        {
            onDamageTaken(ref val);
            this.Health -= val;
            this.hpBar.HpChanged(Health);
            if (Health <= 0)
                OnDeath(this);
        }

        public void Shoot(Action<IProjectile> bulletAdder)
        {
            if (ShootingChance.Next(100) > 90)
                bulletAdder(Weapon.GetBullet(ShootingPosition));
        }

        public override void Draw(Graphics g, Pen p)
        {
            p.Color = color;
            g.DrawRectangle(p, rectangle);
            hpBar.Draw(g, p);
        }

        public override void Move()
        {
            if (strategy != null)
                this.strategy.ApplyStrategy(this);
            this.hpBar.UpdatePosition(Speed.X);
            base.Move();
            rectangle.X = Position.X;
            rectangle.Y = Position.Y;
        }

        public bool IsHit(IProjectile projectile)
        {
            return
                this.Position.Y + this.Size.Y > projectile.Position.Y &&
                this.Position.X + this.Size.X > projectile.Position.X &&
                this.Position.X < projectile.Size.X + projectile.Position.X;
        }

        public EnemyShip(Vector position, Vector speed) :
            base(position,
                 speed,
                 new Vector(),
                 new Vector(VALUES.ENEMY_WIDTH, VALUES.ENEMY_HEIGHT))
        {
            this.Health = VALUES.ENEMY_HEALTH;
            this.Defence = 0;
            this.onDamageTaken += (ref int val) => val -= Defence;
            this.Weapon = new BasicWeapon(new BasicLaserFactory(Direction.Down));
            this.rectangle = new Rectangle(Position.X, Position.Y, Size.X, Size.Y);
            this.strategy = new NormalMovementStrategy();
            this.hpBar = new HPBar(this);
        }
    }
}