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
        private List<IItem> backpack = new List<IItem>();
        public event Action onDeath;

        public IWeapon Weapon
        {
            get;
            set;
        }

        public List<IItem> Backpack
        {
            get { return backpack; }
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
            Weapon.PullTheTrigger(ShootingPosition);
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

        public bool IsHit(IMover projectile)
        {
            return
                this.Position.X < projectile.Position.X + projectile.Size.X &&
                this.Position.X + this.Size.X > projectile.Position.X &&
                this.Position.Y < projectile.Position.Y + projectile.Size.Y &&
                this.Position.Y + this.Size.Y > projectile.Position.Y;
        }

        void Sell(IItem item)
        {
            backpack.Remove(item);
            Gold += item.Price;
        }

        void Buy(IItem item)
        {
            if (Gold < item.Price)
                throw new Exception("Not enough gold to buy this item");
            Gold -= item.Price;
            item.AddToInventory(this);
        }

        public void CleanupAfterLvl()
        {
            Weapon.Cleanup();
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
            this.Weapon = new Weapon.T1.SingleWeapon(new BulletFactory.T1.BasicLaserFactory(Direction.Up));
            this.rectangle = new Rectangle(Position.X, Position.Y, Size.X, Size.Y);
            this.hpBar = new HPBar(this);
            this.Health = MaxHealth;
        }
    }
}