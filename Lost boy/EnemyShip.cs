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
                return new Vector(Position.X + Size.X / 2, Position.Y + Size.Y);
            }
        }
        public void TakeDamage(int val)
        {
            onDamageTaken(ref val);
            this.Health -= val;
        }

        public void Shoot(Action<IProjectile> bulletAdder)
        {
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

        public bool IsHit(IProjectile projectile)
        {
            return
                projectile.Position.Y + projectile.Size.Y > this.Position.Y &&
                projectile.Position.Y < this.Position.Y + this.Size.Y &&
                projectile.Position.X + projectile.Size.X > this.Position.X &&
                projectile.Position.X < this.Position.X + this.Size.X;
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
            this.Ammo = new BasicLaser(ShootingPosition, Direction.Down);
            this.Weapon = new BasicWeapon(Ammo);
            this.rectangle = new Rectangle(Position.X, Position.Y, Size.X, Size.Y);
        }
    }
}
