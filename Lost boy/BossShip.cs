using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lost_boy.Enemies
{
    public class Splitter : EnemyShip
    {
        private Rectangle drawable;
        private Action<EnemyShip> ShipAdder;

        private class SplitterWeapon : Weapon.Weapon
        {
            private int bulletCount;
            private readonly int speedDiff = 8;

            public void ShootHealth(Vector where)
            {
                var b = new HealthBonus(where);
                b.onDeath += b.Recycle;
                BulletAdder(b);
            }

            private void EvenBulletFormation(List<IBullet> bullets)
            {
                int furthest = -bulletCount / 2;
                foreach (var b in bullets)
                {
                    b.Speed = new Vector(furthest * speedDiff, b.Speed.Y);
                    furthest++;
                }
            }

            private void OddBulletFormation(List<IBullet> bullets)
            {
                int offSet = speedDiff / 2;
                int furthest = -bulletCount / 2;
                foreach (var b in bullets)
                {
                    b.Speed = new Vector(furthest * speedDiff + offSet, b.Speed.Y);
                    furthest++;
                }
            }

            protected override void AddBullets(Vector launchPosition)
            {
                List<IBullet> bullets = new List<IBullet>(bulletCount);
                for (int i = 0; i < bulletCount; ++i)
                {
                    bullets.Add(this.Ammo.Create(launchPosition));
                    ImbueBullet(bullets[i]);
                }
                if (bulletCount % 2 == 0)
                    EvenBulletFormation(bullets);
                else
                    OddBulletFormation(bullets);
                bullets.ForEach(BulletAdder);
            }

            public SplitterWeapon(SplitterWeapon parent) :
                base(parent.Ammo)
            {
                if (parent.bulletCount != 1)
                    parent.bulletCount -= 1;
                bulletCount = parent.bulletCount;
                BulletAdder = parent.BulletAdder;
                ReloadTime = parent.ReloadTime;
                SuckOnShots(parent);
            }

            public SplitterWeapon(Tier tier) :
                base(new BulletFactory.T1.BasicLaserFactory(Direction.Down))
            {
                this.ReloadTime = 1500;
                this.bulletCount = 4 + (int)tier * 1;
            }
        }

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
                drawable.Width = value.X;
                drawable.Height = value.Y;
            }
        }

        public override void Move()
        {
            if (MovementStrategy == null || MaxSpeed <= 0)
                return;
            base.Move();
            drawable.X = Position.X;
            drawable.Y = Position.Y;
        }

        public override void Draw(Graphics g, Pen p)
        {
            base.Draw(g, p);
            p.Color = Color.LawnGreen;
            g.DrawEllipse(p, drawable);
        }

        private Vector ShootingPosition
        {
            get { return new Vector(Position.X + Size.X / 2, Position.Y + Size.Y); }
        }

        public override void Shoot()
        {
            Weapon.PullTheTrigger(ShootingPosition);
        }

        public override void SetDefaultMoveStrategy()
        {
            MovementStrategy = new DanceInRectangleStrategy(
                new Vector(1, 1),
                new Vector(VALUES.WIDTH - 50, VALUES.HEIGHT - 200));
        }

        private Splitter(Splitter parent, int split) :
            base(parent.Position)
        {
            MaxHealth = parent.MaxHealth / 2;
            Defence = parent.Defence;
            MaxSpeed = parent.MaxSpeed * 4 / 3;
            Size = parent.Size * 3 / 4;
            Weapon = new SplitterWeapon((SplitterWeapon)parent.Weapon);
            drawable = new Rectangle(Position.X, Position.Y, Size.X, Size.Y);
            ShipAdder = parent.ShipAdder;
            if (split == 1)
                onDeath += () =>
                {
                    ((SplitterWeapon)Weapon).ShootHealth(Position);
                };
            if (split > 0)
                onDeath += () =>
                {
                    split--;
                    ShipAdder(new Splitter(this, split));
                    ShipAdder(new Splitter(this, split));
                };
            Health = MaxHealth;
            SetDefaultMoveStrategy();
        }
        public Splitter(Tier tier, Action<EnemyShip> adder) :
            base(new Vector(VALUES.WIDTH / 2, 50))
        {
            ShipAdder = adder;
            Size = new Vector(100, 100);
            drawable = new Rectangle(Position, new Size(Size.X, Size.Y));
            Weapon = new SplitterWeapon(tier);
            MaxSpeed = 7;
            int split = 1;
            switch(tier)
            {
                case Tier.T1:
                    MaxHealth = 200;
                    this.Weapon.AppendOnShot(new OnShots.SpeedChange(-10));
                    this.Weapon.Ammo.AppendOnHit(new OnHits.SlowEffect(1));
                    split = 2;
                    break;
                case Tier.T2:
                    MaxHealth = 300;
                    this.Weapon.AppendOnShot(new OnShots.SizeChange(3));
                    this.Weapon.AppendOnShot(new OnShots.SpeedChange(-10));
                    this.Weapon.Ammo.AppendDmgModifier((ref int val) =>
                    {
                        val += 5;
                    });
                    this.Weapon.Ammo.AppendOnHit(new OnHits.SlowEffect(2));
                    split = 3;
                    break;
                case Tier.T3:
                    MaxHealth = 400;
                    this.Weapon.AppendOnShot(new OnShots.SizeChange(5));
                    this.Weapon.AppendOnShot(new OnShots.SpeedChange(-10));
                    this.Weapon.Ammo.AppendDmgModifier((ref int val) =>
                    {
                        val += 8;
                    });
                    this.Weapon.Ammo.AppendOnHit(new OnHits.SlowEffect(3));
                    split = 4;
                    break;
            }
            onDeath += () =>
            {
                ShipAdder(new Splitter(this, split));
                ShipAdder(new Splitter(this, split));
            };
            this.Health = MaxHealth;
        }
    }
}
