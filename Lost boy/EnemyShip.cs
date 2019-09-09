﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Lost_boy
{
    public abstract class EnemyShip : Mover, IShip
    {
        private event Modify onDamageTaken;
        private int maxHealth;
        private Rectangle rectangle;
        private HPBar hpBar;
        private IMovementStrategy strategy;
        public Random shootingRandomizer = new Random(123);
        protected Color color = Color.Red;
        public event Action onDeath;

        public int ShootingChance
        {
            private get;
            set;
        }

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
                hpBar = new HPBar(this);
            }
        }

        public void Heal(int val)
        {
            Health += val;
            if (Health > MaxHealth)
                Health = MaxHealth;
            this.hpBar.HpChanged(Health);
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

        public IMovementStrategy MovementStrategy
        {
            get { return strategy; }
            set
            {
                if (strategy != null)
                    strategy.StopStrategy(this);
                strategy = value;
            }
        }

        public override Vector Size
        {
            get { return base.Size; }
            set
            {
                base.Size = value;
                rectangle.Width = value.X;
                rectangle.Height = value.Y;
            }
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
            if (val < 1)
            {
                val = 1;
            }
            this.Health -= val;
            this.hpBar.HpChanged(Health);
            if (Health <= 0)
                onDeath();
        }

        public void TakeTrueDamage(int val)
        {
            this.Health -= val;
            this.hpBar.HpChanged(Health);
            if (Health <= 0)
                onDeath();
        }

        public virtual void Shoot()
        {
            if (shootingRandomizer.Next(1000) < ShootingChance)
                Weapon.PullTheTrigger(ShootingPosition);
        }

        public override void Draw(Graphics g, Pen p)
        {
            p.Color = color;
            g.DrawRectangle(p, rectangle);
            hpBar.Draw(g, p);
        }

        public override void Move()
        {
            if (MovementStrategy == null || MaxSpeed <= 0)
                return;
            this.MovementStrategy.ApplyStrategy(this);
            this.hpBar.UpdatePosition(Speed.X, Speed.Y);
            base.Move();
            rectangle.X = Position.X;
            rectangle.Y = Position.Y;
        }

        public void Teleport(int x, int y)
        {
            this.hpBar.UpdatePosition(x - Position.X, y - Position.Y);
            this.Position = new Vector(x, y);
            rectangle.X = x;
            rectangle.Y = y;
        }

        public abstract void SetDefaultMoveStrategy();

        public bool IsHit(IMover projectile)
        {
            return
                this.Position.X < projectile.Position.X + projectile.Size.X &&
                this.Position.X + this.Size.X > projectile.Position.X &&
                this.Position.Y < projectile.Position.Y + projectile.Size.Y &&
                this.Position.Y + this.Size.Y > projectile.Position.Y;
        }

        public EnemyShip(Vector position) :
            base(position,
                 new Vector(),
                 new Vector(),
                 new Vector(VALUES.ENEMY_WIDTH, VALUES.ENEMY_HEIGHT))
        {
            this.shootingRandomizer = new Random(VALUES.random.Next());
            this.Health = VALUES.ENEMY_HEALTH;
            this.Defence = 0;
            this.onDamageTaken += (ref int val) => val -= Defence;
            this.Weapon = new SingleWeapon(new BasicLaserFactory(Direction.Down));
            this.rectangle = new Rectangle(Position.X, Position.Y, Size.X, Size.Y);
            this.MovementStrategy = new NormalMovementStrategy();
            this.hpBar = new HPBar(this);
            this.onDeath += () =>
            {
                MovementStrategy = null;
            };
        }
    }

    public class RockyEnemy : EnemyShip
    {
        public override void SetDefaultMoveStrategy()
        {
            MovementStrategy = null;
        }

        public override string ToString()
        {
            return base.ToString() + "Rocky";
        }

        public RockyEnemy(Vector position) :
            base(position)
        {
            this.Defence = 25;
            this.Size = this.Size * 2;
            this.MaxHealth = 2 * VALUES.ENEMY_HEALTH;
            this.Speed = new Vector(10, 0);
            this.color = Color.Brown;
            this.MaxSpeed = 5;
            this.ShootingChance = 25;
            this.Weapon.AppendOnShot(new OnShots.ColorChage(color));
            this.Weapon.AppendOnShot(new OnShots.SizeChange(10));
            this.Weapon.AppendOnShot(new OnShots.SpeedChange(-5));
            this.Weapon.Ammo.AppendDmgModifier((ref int val) =>
            {
                val *= 2;
            });
            this.Health = MaxHealth;
        }
    }

    public class FrostyEnemy : EnemyShip
    {
        public override void SetDefaultMoveStrategy()
        {
            MovementStrategy = new NormalMovementStrategy();
        }

        public override string ToString()
        {
            return base.ToString() + "Frosty";
        }

        public FrostyEnemy(Vector position) :
            base(position)
        {
            this.Defence = 0;
            this.Size = this.Size / 2;
            this.MaxHealth = VALUES.ENEMY_HEALTH;
            this.MaxSpeed = 15;
            this.color = Color.Blue;
            this.ShootingChance = 75;
            this.Weapon.AppendOnShot(new OnShots.SpeedChange(5));
            this.Weapon.AppendOnShot(new OnShots.ColorChage(color));
            this.Weapon.Ammo.AppendOnHit(new OnHits.SlowEffect(5));
            this.Health = MaxHealth;
        }
    }

    public class TrickyEnemy : EnemyShip
    {
        private Mover playerShipWatcher;
        public override void SetDefaultMoveStrategy()
        {
            MovementStrategy = new FindAndShootStrategy(playerShipWatcher);
        }

        public override void Shoot()
        {
            if (playerShipWatcher.Position.X < this.Position.X + 30 &&
                playerShipWatcher.Position.X > this.Position.X - 30)
                base.Shoot();
        }

        public override string ToString()
        {
            return base.ToString() + "Tricky";
        }

        public TrickyEnemy(Mover playerShip, Vector position) :
            base(position)
        {
            this.playerShipWatcher = playerShip;
            this.Defence = -10;
            this.MaxHealth = VALUES.ENEMY_HEALTH / 2;
            this.MaxSpeed = 20;
            this.color = Color.DeepPink;
            this.ShootingChance = 90;
            this.Weapon.AppendOnShot(new OnShots.SpeedChange(15));
            this.Weapon.AppendOnShot(new OnShots.ColorChage(color));
            this.Weapon.Ammo.AppendOnHit(new OnHits.BurnChance(5, 3, 50));
            this.Health = MaxHealth;
        }
    }

    public class CasualEnemy : EnemyShip
    {
        public override void SetDefaultMoveStrategy()
        {
            MovementStrategy = new NormalMovementStrategy();
        }

        public override string ToString()
        {
            return base.ToString() + "Casual";
        }

        public CasualEnemy(Vector position) :
            base(position)
        {
            this.Defence = 5;
            this.MaxHealth = VALUES.ENEMY_HEALTH;
            this.MaxSpeed = 10;
            this.color = Color.Chartreuse;
            this.Weapon.AppendOnShot(new OnShots.ColorChage(color));
            this.ShootingChance = 90;
            this.Health = MaxHealth;
        }
    }
}