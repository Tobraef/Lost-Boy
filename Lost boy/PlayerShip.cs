using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Lost_boy
{
    public class PlayerShip : Mover, IShip
    {
        private event Modify onDamageTaken;
        private Rectangle rectangle;
        private int maxHealth;
        private Color color = Color.Green;
        private HPBar hpBar;
        public event Action onDeath;
        public event Action<IProjectile> bulletAdder;

        public IWeapon Weapon
        {
            get;
            set;
        }

        public int MaxHealth
        {
            get { return maxHealth; }
            set
            {
                Health += value - maxHealth;
                maxHealth = value;
                this.hpBar = new HPBar(this);
            }
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

        public int Gold
        {
            get;
            set;
        }

        public int MaxSpeed
        {
            get;
            set;
        }

        private Vector ShootingPosition
        {
            get
            {
                return new Vector(Position.X + Size.X / 2, Position.Y);
            }
        }

        public void TakeDamage(int val)
        {
            onDamageTaken(ref val);
            if (val > 0)
            {
                this.Health -= val;
                this.hpBar.HpChanged(Health);
            }
        }

        public void TakeTrueDamage(int val)
        {
            this.Health -= val;
            this.hpBar.HpChanged(Health);
            if (Health <= 0)
                onDeath();
        }

        public void Shoot()
        {
            if (Weapon.IsLoaded)
                bulletAdder(Weapon.GetBullet(ShootingPosition));
        }

        public override void Draw(Graphics g, Pen p)
        {
            p.Color = color;
            g.DrawRectangle(p, rectangle);
            hpBar.Draw(g, p);
        }

        public void Heal(int val)
        {
            Health += val;
            if (Health > MaxHealth)
                Health = MaxHealth;
            hpBar.HpChanged(Health);
        }

        public override void Move()
        {
            base.Move();
            this.hpBar.UpdatePosition(Speed.X, Speed.Y);
            rectangle.X = Position.X;
            rectangle.Y = Position.Y;
        }

        public bool IsHit(IProjectile projectile)
        {
            return
                this.Position.X < projectile.Position.X + projectile.Size.X &&
                this.Position.X + this.Size.X > projectile.Position.X &&
                this.Position.Y < projectile.Position.Y + projectile.Size.Y &&
                this.Position.Y + this.Size.Y > projectile.Position.Y;
        }

        public PlayerShip() :
            base(new Vector(VALUES.WIDTH / 2, VALUES.HEIGHT - VALUES.PLAYER_HEIGHT),
                 new Vector(),
                 new Vector(),
                 new Vector(VALUES.PLAYER_WIDTH, VALUES.PLAYER_HEIGHT))
        {
            this.MaxSpeed = VALUES.PLAYER_SPEED;
            this.MaxHealth = VALUES.PLAYER_HEALTH;
            this.Defence = 0;
            this.onDamageTaken += (ref int val) => val -= Defence;
            this.onDamageTaken += (ref int val) => { if (Health <= 0) System.Windows.Forms.Application.Exit(); };
            this.Weapon = new BasicWeapon(new BasicLaserFactory(Direction.Up));
            this.rectangle = new Rectangle(Position.X, Position.Y, Size.X, Size.Y);
            this.hpBar = new HPBar(this);
            this.Health = MaxHealth;
        }
    }
}