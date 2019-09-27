using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Drawing;

namespace Lost_boy.Enemies
{
    public abstract class EnemyShip : Mover, IShip
    {
        private event Modify onDamageTaken;
        private int maxHealth;
        private HPBar hpBar;
        private IMovementStrategy strategy;
        public Random shootingRandomizer = new Random(123);
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
            hpBar.Draw(g, p);
        }

        public override void Move()
        {
            this.MovementStrategy.ApplyStrategy(this);
            this.hpBar.UpdatePosition(Speed.X, Speed.Y);
            base.Move();
        }

        public virtual void Teleport(int x, int y)
        {
            this.hpBar.UpdatePosition(x - Position.X, y - Position.Y);
            this.Position = new Vector(x, y);
        }

        public abstract void SetDefaultMoveStrategy();

        public abstract bool IsHit(IMover projectile);

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
            this.Weapon = new Weapon.T1.SingleWeapon(new BulletFactory.T1.BasicLaserFactory(Direction.Down));
            this.MovementStrategy = new NormalMovementStrategy();
            this.hpBar = new HPBar(this);
            this.onDeath += () =>
            {
                MovementStrategy = null;
            };
        }
    }

    public abstract class SimpleShapedEnemy : EnemyShip
    {
        private Rectangle rectangle;
        protected Color color = Color.Red;

        public override bool IsHit(IMover projectile)
        {
            return
                this.Position.X < projectile.Position.X + projectile.Size.X &&
                this.Position.X + this.Size.X > projectile.Position.X &&
                this.Position.Y < projectile.Position.Y + projectile.Size.Y &&
                this.Position.Y + this.Size.Y > projectile.Position.Y;
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

        public override void Move()
        {
            if (MovementStrategy == null || MaxSpeed <= 0)
                return;
            base.Move();
            rectangle.X = Position.X;
            rectangle.Y = Position.Y;
        }

        public override void Draw(Graphics g, Pen p)
        {
            base.Draw(g, p);
            p.Color = color;
            g.DrawRectangle(p, rectangle);
        }

        public override void Teleport(int x, int y)
        {
            base.Teleport(x, y);
            rectangle.X = x;
            rectangle.Y = y;
        }

        public SimpleShapedEnemy(Vector where) :
            base(where)
        {
            this.rectangle = new Rectangle(Position.X, Position.Y, Size.X, Size.Y);
        }
    }

    public class RockyEnemy : SimpleShapedEnemy
    {
        public override void SetDefaultMoveStrategy()
        {
            MovementStrategy = new NormalMovementStrategy();
        }

        public override string ToString()
        {
            return base.ToString() + "Rocky";
        }

        public RockyEnemy(Vector position, Tier tier) :
            base(position)
        {
            switch (tier)
            {
                case Tier.T1:
                    this.Defence = 25;
                    this.Size = this.Size * 2;
                    this.MaxHealth = 2 * VALUES.ENEMY_HEALTH;
                    this.Speed = new Vector(10, 0);
                    this.color = Color.Brown;
                    this.MaxSpeed = 5;
                    this.ShootingChance = 25;
                    this.Weapon.AppendOnShot(new OnShots.ColorChage(color));
                    this.Weapon.AppendOnShot(new OnShots.SizeChange(5));
                    this.Weapon.AppendOnShot(new OnShots.SpeedChange(-5));
                    this.Weapon.Ammo.AppendDmgModifier((ref int val) =>
                    {
                        val *= 2;
                    });
                    this.Health = MaxHealth;
                    break;

                case Tier.T2:
                    this.Defence = 45;
                    this.Size = this.Size * 2;
                    this.MaxHealth = 4 * VALUES.ENEMY_HEALTH;
                    this.Speed = new Vector(5, 0);
                    this.color = Color.Brown;
                    this.MaxSpeed = 5;
                    this.ShootingChance = 35;
                    this.Weapon = new Weapon.T2.DoubleWeapon(
                        new BulletFactory.T1.BasicLaserFactory(Direction.Down));
                    this.Weapon.AppendOnShot(new OnShots.ColorChage(color));
                    this.Weapon.AppendOnShot(new OnShots.SizeChange(7));
                    this.Weapon.AppendOnShot(new OnShots.SpeedChange(-3));
                    this.Weapon.Ammo.AppendDmgModifier((ref int val) =>
                    {
                        val += 5;
                        val *= 2;
                    });
                    this.Health = MaxHealth;
                    break;

                case Tier.T3:
                    this.Defence = 75;
                    this.Size = this.Size * 2;
                    this.MaxHealth = 6 * VALUES.ENEMY_HEALTH;
                    this.color = Color.Brown;
                    this.MaxSpeed = 10;
                    this.ShootingChance = 50;
                    this.Weapon = new Weapon.T2.DoubleWeapon(
                        new BulletFactory.T1.BasicLaserFactory(Direction.Down));
                    this.Weapon.AppendOnShot(new OnShots.ColorChage(color));
                    this.Weapon.AppendOnShot(new OnShots.SizeChange(10));
                    this.Weapon.Ammo.AppendDmgModifier((ref int val) =>
                    {
                        val += 30;
                    });
                    this.Weapon.Ammo.AppendOnHit(new OnHits.SlowEffect(8));
                    this.Health = MaxHealth;
                    break;
            }
        }
    }

    public class StealthyEnemy : SimpleShapedEnemy
    {
        private Timer timerToBlack;
        private Timer timerToNormal;

        public override void SetDefaultMoveStrategy()
        {
            MovementStrategy = new DanceInRectangleStrategy(
                new Vector(200, 200), new Vector(VALUES.WIDTH - 200, 400));
        }

        public override void Draw(Graphics g, Pen p)
        {
            if (color != Color.Black)
                base.Draw(g, p);
        }

        private void GoColor()
        {
            color = Color.Cornsilk;
            timerToNormal.Stop();
            timerToBlack.Start();
        }

        private void GoBlack()
        {
            color = Color.Black;
            timerToBlack.Stop();
            timerToNormal.Start();
        }

        public override string ToString()
        {
            return base.ToString() + "Stealth";
        }

        public StealthyEnemy(Vector position, Tier tier) :
            base(position)
        {
            int stealthTime = 0;
            int stealthBreak = 0;
            this.color = Color.Cornsilk;
            switch (tier)
            {
                case Tier.T1:
                    this.Defence = 0;
                    this.MaxHealth = VALUES.ENEMY_HEALTH;
                    this.MaxSpeed = 15;
                    this.ShootingChance = 75;
                    this.Weapon.AppendOnShot(new OnShots.SpeedChange(10));
                    this.Weapon.AppendOnShot(new OnShots.ColorChage(color));
                    this.Weapon.Ammo.AppendDmgModifier((ref int i) => i += 10);
                    stealthBreak = 5000;
                    stealthTime = 3000;
                    break;

                case Tier.T2:
                    this.Defence = 0;
                    this.MaxHealth = 2 * VALUES.ENEMY_HEALTH;
                    this.MaxSpeed = 20;
                    this.ShootingChance = 80;
                    this.Weapon.AppendOnShot(new OnShots.SpeedChange(15));
                    this.Weapon.AppendOnShot(new OnShots.ColorChage(color));
                    this.Weapon.Ammo.AppendDmgModifier((ref int i) => i += 18);
                    stealthBreak = 4500;
                    stealthTime = 4000;
                    break;

                case Tier.T3:
                    this.Defence = 15;
                    this.MaxHealth = 2 * VALUES.ENEMY_HEALTH;
                    this.MaxSpeed = 25;
                    this.ShootingChance = 80;
                    this.Weapon.AppendOnShot(new OnShots.SpeedChange(15));
                    this.Weapon.AppendOnShot(new OnShots.SizeChange(7));
                    this.Weapon.AppendOnShot(new OnShots.ColorChage(color));
                    this.Weapon.Ammo.AppendDmgModifier((ref int i) => i += 30);
                    stealthBreak = 4000;
                    stealthTime = 5000;
                    break;
            }
            this.Health = MaxHealth;
            timerToBlack = new Timer(stealthBreak);
            timerToNormal = new Timer(stealthTime);
            timerToBlack.Elapsed += (s, e) => GoBlack();
            timerToNormal.Elapsed += (s, e) => GoColor();
            GoColor();
            onDeath += () => { timerToBlack.Stop(); timerToNormal.Stop(); };
        }
    }

    public class FrostyEnemy : SimpleShapedEnemy
    {
        public override void SetDefaultMoveStrategy()
        {
            MovementStrategy = new NormalMovementStrategy();
        }

        public override string ToString()
        {
            return base.ToString() + "Frosty";
        }

        public FrostyEnemy(Vector position, Tier tier) :
            base(position)
        {
            switch (tier)
            {
                case Tier.T1:
                    this.Defence = 0;
                    this.MaxHealth = VALUES.ENEMY_HEALTH;
                    this.MaxSpeed = 15;
                    this.color = Color.Blue;
                    this.ShootingChance = 65;
                    this.Weapon.AppendOnShot(new OnShots.SpeedChange(5));
                    this.Weapon.AppendOnShot(new OnShots.ColorChage(color));
                    this.Weapon.Ammo.AppendOnHit(new OnHits.SlowEffect(3));
                    this.Health = MaxHealth;
                    break;

                case Tier.T2:
                    this.Defence = 18;
                    this.Size = this.Size * 3 / 4;
                    this.MaxHealth = 2 * VALUES.ENEMY_HEALTH;
                    this.MaxSpeed = 15;
                    this.color = Color.Blue;
                    this.ShootingChance = 75;
                    this.Weapon.AppendOnShot(new OnShots.SpeedChange(8));
                    this.Weapon.AppendOnShot(new OnShots.ColorChage(color));
                    this.Weapon.Ammo.AppendOnHit(new OnHits.SlowEffect(5));
                    this.Weapon.Ammo.AppendDmgModifier((ref int i) =>
                    {
                        i += 10;
                    });
                    this.Health = MaxHealth;
                    break;

                case Tier.T3:
                    this.Defence = 18;
                    this.Size = this.Size * 3 / 4;
                    this.MaxHealth = VALUES.ENEMY_HEALTH * 3 / 2;
                    this.MaxSpeed = 15;
                    this.color = Color.Blue;
                    this.ShootingChance = 75;
                    this.Weapon = new Weapon.T3.TripleWeapon(
                        new BulletFactory.T1.PlasmaFactory(Direction.Down));
                    this.Weapon.AppendOnShot(new OnShots.SpeedChange(5));
                    this.Weapon.AppendOnShot(new OnShots.ColorChage(color));
                    this.Weapon.Ammo.AppendOnHit(new OnHits.SlowEffect(3));
                    this.Weapon.Ammo.AppendOnHit(new OnHits.BurnChance(10, 1, 100));
                    this.Weapon.Ammo.AppendDmgModifier((ref int i) =>
                    {
                        i += 10;
                    });
                    this.Health = MaxHealth;
                    break;
            }
        }
    }

    public class TrickyEnemy : SimpleShapedEnemy
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

        public TrickyEnemy(Mover playerShip, Vector position, Tier tier) :
            base(position)
        {
            this.playerShipWatcher = playerShip;
            switch (tier)
            {
                case Tier.T1:
                    this.Defence = -5;
                    this.MaxHealth = VALUES.ENEMY_HEALTH;
                    this.MaxSpeed = 20;
                    this.color = Color.DeepPink;
                    this.ShootingChance = 90;
                    this.Weapon.AppendOnShot(new OnShots.SpeedChange(15));
                    this.Weapon.AppendOnShot(new OnShots.ColorChage(color));
                    this.Weapon.Ammo.AppendOnHit(new OnHits.BurnChance(5, 3, 50));
                    this.Health = MaxHealth;
                    break;

                case Tier.T2:
                    this.playerShipWatcher = playerShip;
                    this.Defence = 10;
                    this.MaxHealth = VALUES.ENEMY_HEALTH * 3 / 2;
                    this.MaxSpeed = 23;
                    this.color = Color.DeepPink;
                    this.ShootingChance = 90;
                    this.Weapon.AppendOnShot(new OnShots.SpeedChange(20));
                    this.Weapon.AppendOnShot(new OnShots.ColorChage(color));
                    this.Weapon.AppendOnShot(new OnShots.SizeChange(5));
                    this.Weapon.Ammo.AppendOnHit(new OnHits.BurnChance(10, 3, 50));
                    this.Weapon.Ammo.AppendDmgModifier((ref int i) =>
                    {
                        i += 10;
                    });
                    this.Health = MaxHealth;
                    break;

                case Tier.T3:
                    this.playerShipWatcher = playerShip;
                    this.Defence = 10;
                    this.MaxHealth = VALUES.ENEMY_HEALTH * 2;
                    this.MaxSpeed = 30;
                    this.color = Color.DeepPink;
                    this.ShootingChance = 100;
                    this.Weapon = new Weapon.T2.DoubleWeapon(
                        new BulletFactory.T2.HellHotFactory(Direction.Down));
                    this.Weapon.AppendOnShot(new OnShots.SpeedChange(15));
                    this.Weapon.AppendOnShot(new OnShots.ColorChage(color));
                    this.Weapon.AppendOnShot(new OnShots.SizeChange(5));
                    this.Weapon.Ammo.AppendOnHit(new OnHits.BurnChance(10, 3, 100));
                    this.Weapon.Ammo.AppendDmgModifier((ref int i) =>
                    {
                        i += 10;
                    });
                    this.Health = MaxHealth;
                    break;
            }
        }
    }

    public class CasualEnemy : SimpleShapedEnemy
    {
        private Func<IMovementStrategy> msReturn;

        public override void SetDefaultMoveStrategy()
        {
            MovementStrategy = msReturn();
        }

        public override string ToString()
        {
            return base.ToString() + "Casual";
        }

        public CasualEnemy(Vector position, Tier tier) :
            base(position)
        {
            switch (tier)
            {
                case Tier.T1:
                    this.Defence = 5;
                    this.MaxHealth = VALUES.ENEMY_HEALTH;
                    this.MaxSpeed = 10;
                    this.color = Color.Chartreuse;
                    this.Weapon.AppendOnShot(new OnShots.ColorChage(color));
                    this.ShootingChance = 70;
                    this.Health = MaxHealth;
                    this.msReturn = () => new NormalMovementStrategy();
                    break;

                case Tier.T2:
                    this.Defence = 25;
                    this.MaxHealth = 2 * VALUES.ENEMY_HEALTH;
                    this.MaxSpeed = 15;
                    this.color = Color.Chartreuse;
                    this.Weapon.AppendOnShot(new OnShots.ColorChage(color));
                    this.ShootingChance = 80;
                    this.Weapon.AppendOnShot(new OnShots.SizeChange(3));
                    this.Weapon.AppendOnShot(new OnShots.SpeedChange(5));
                    this.Weapon.Ammo.AppendOnHit(new OnHits.BurnChance(5, 3, 100));
                    this.Weapon.Ammo.AppendDmgModifier((ref int i) =>
                    {
                        i += 5;
                    });
                    this.msReturn = () => new DanceInRectangleStrategy();
                    this.Health = MaxHealth;
                    break;

                case Tier.T3:
                    this.Defence = 45;
                    this.MaxHealth = 4 * VALUES.ENEMY_HEALTH;
                    this.MaxSpeed = 20;
                    this.color = Color.Chartreuse;
                    this.msReturn = () => new DanceInRectangleStrategy();
                    this.Weapon = new Weapon.T2.DoubleWeapon(
                        new BulletFactory.T2.HellHotFactory(Direction.Down));
                    this.Weapon.AppendOnShot(new OnShots.ColorChage(color));
                    this.ShootingChance = 90;
                    this.Weapon.AppendOnShot(new OnShots.SizeChange(5));
                    this.Weapon.AppendOnShot(new OnShots.SpeedChange(10));
                    this.Weapon.Ammo.AppendOnHit(new OnHits.BurnChance(10, 3, 100));
                    this.Weapon.Ammo.AppendDmgModifier((ref int i) =>
                    {
                        i += 15;
                    });
                    this.Health = MaxHealth;
                    break;
            }
        }
    }
}