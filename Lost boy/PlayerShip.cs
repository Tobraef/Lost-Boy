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

        public IWeapon Weapon
        {
            get;
            set;
        }

        public IProjectile Ammo
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
            this.Health -= val;
        }

        public void Shoot(Action<IProjectile> bulletAdder)
        {
            if (Weapon.IsLoaded)
                bulletAdder(Weapon.GetProjectile(ShootingPosition));
        }

        public override void Draw(Graphics g, Pen p)
        {
            p.Color = color;
            g.DrawRectangle(p, rectangle);
        }

        public override void Move()
        {
            base.Move();
            rectangle.X = Position.X;
            rectangle.Y = Position.Y;
        }

        public void Move(int dx)
        {
            Position = new Vector(Position.X + dx, Position.Y);
            rectangle.X = Position.X;
        }

        public bool IsHit(IProjectile projectile)
        {
            return
                projectile.Position.Y + projectile.Size.Y > this.Position.Y &&
                projectile.Position.Y < this.Position.Y + this.Size.Y &&
                projectile.Position.X + projectile.Size.X > this.Position.X &&
                projectile.Position.X < this.Position.X + this.Size.X;
        }

        public PlayerShip() :
            base(new Vector(VALUES.WIDTH/2, VALUES.HEIGHT - VALUES.PLAYER_HEIGHT),
                 new Vector(),
                 new Vector(),
                 new Vector(VALUES.PLAYER_WIDTH, VALUES.PLAYER_HEIGHT))
        {
            this.Health = VALUES.PLAYER_HEALTH;
            this.Defence = 0;
            this.onDamageTaken += (ref int val) => val -= Defence;
            this.onDamageTaken += (ref int val) => { if (Health <= 0) System.Windows.Forms.Application.Exit(); };
            this.Ammo = new BasicLaser(ShootingPosition, Direction.Up);
            this.Weapon = new BasicWeapon(Ammo);
            this.rectangle = new Rectangle(Position.X, Position.Y, Size.X, Size.Y);
        }
    }
}