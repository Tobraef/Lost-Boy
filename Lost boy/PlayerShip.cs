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
        private Color color = Color.Green;
        private HPBar hpBar;

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
                return new Vector(Position.X + Size.X / 2, Position.Y);
            }
        }

        public void TakeDamage(int val)
        {
            onDamageTaken(ref val);
            this.hpBar.HpChanged(Health);
            this.Health -= val;
        }

        public void Shoot(Action<IProjectile> bulletAdder)
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

        public override void Move()
        {
            base.Move();
            this.hpBar.UpdatePosition(Speed.X);
            rectangle.X = Position.X;
            rectangle.Y = Position.Y;
        }

        public void Move(int dx)
        {
            Position = new Vector(Position.X + dx, Position.Y);
            this.hpBar.UpdatePosition(dx);
            rectangle.X = Position.X;
        }

        public bool IsHit(IProjectile projectile)
        {
            return
                this.Position.Y < projectile.Position.Y + projectile.Size.Y &&
                this.Position.X + this.Size.X > projectile.Position.X &&
                this.Position.X < projectile.Size.X + projectile.Position.X;
        }

        public PlayerShip() :
            base(new Vector(VALUES.WIDTH / 2, VALUES.HEIGHT - VALUES.PLAYER_HEIGHT),
                 new Vector(),
                 new Vector(),
                 new Vector(VALUES.PLAYER_WIDTH, VALUES.PLAYER_HEIGHT))
        {
            this.Health = VALUES.PLAYER_HEALTH;
            this.Defence = 0;
            this.onDamageTaken += (ref int val) => val -= Defence;
            this.onDamageTaken += (ref int val) => { if (Health <= 0) System.Windows.Forms.Application.Exit(); };
            this.Weapon = new BasicWeapon(new BasicLaserFactory(Direction.Up));
            this.rectangle = new Rectangle(Position.X, Position.Y, Size.X, Size.Y);
            this.hpBar = new HPBar(this);
        }
    }
}