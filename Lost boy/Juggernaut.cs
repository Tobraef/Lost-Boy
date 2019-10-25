using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Lost_boy.OnShots;

namespace Lost_boy.Enemies
{

    class JuggerWeapon : IWeapon
    {
        private struct JuggerWeaponInfo
        {
            public int shootingChance;
            public Vector shootingPosition;
        }

        private class JuggerLaserWeapon : Weapon.Weapon
        {
            protected override void AddBullets(Vector launchPosition)
            {
                IBullet bullet = Ammo.Create(launchPosition);
                ImbueBullet(bullet);
                BulletAdder(bullet);
            }

            public JuggerLaserWeapon(IBulletFactory ammo) :
                base(ammo)
            { 
            }
        }

        // int for randomization
        private List<KeyValuePair<JuggerWeaponInfo, Weapon.Weapon>> weapons;

        public Action<IProjectile> BulletAdder
        {
            private get { return null; }
            set
            {
                foreach (var w in weapons)
                {
                    w.Value.BulletAdder = value;
                }
            }
        }

        public Action<IProjectile> RecycledBulletAdder
        {
            get { return null; }
            set
            {
                foreach (var w in weapons)
                {
                    w.Value.RecycledBulletAdder = value;
                }
            }
        }

        public IBulletFactory Ammo
        {
            get { throw new Exception("Look at me, I am the ammo now"); }
            set { throw new Exception("Look at me, I am the ammo now"); }
        }

        public int ReloadTime
        {
            get { return 0; }
            set { foreach (var w in weapons) { w.Value.ReloadTime += value; } }
        }

        public void AppendOnShot(OnShot e)
        {
            foreach (var w in weapons)
            {
                w.Value.AppendOnShot(e);
            }
        }

        public void Cleanup()
        {
            foreach (var w in weapons)
            {
                w.Value.Cleanup();
            }
        }

        public void PullTheTrigger(Vector launchPos)
        {
            foreach (var w in weapons)
            {
                if (VALUES.random.Next(100) < w.Key.shootingChance)
                {
                    w.Value.PullTheTrigger(launchPos + w.Key.shootingPosition);
                }
            }
        }

        public void SuckOnShots(IWeapon other)
        {
            var otherWeapons = ((JuggerWeapon)other).weapons;
            for (int i = 0; i < weapons.Count; ++i)
            {
                weapons[i].Value.SuckOnShots(otherWeapons[i].Value);
            }
        }

        public JuggerWeapon(Tier tier)
        {
            switch (tier)
            {
                case Tier.T1:
                    weapons = new List<KeyValuePair<JuggerWeaponInfo, Weapon.Weapon>>
                    {
                        new KeyValuePair<JuggerWeaponInfo, Weapon.Weapon> (new JuggerWeaponInfo{ shootingChance = 90, shootingPosition = new Vector(-40, 10)},
                        new Weapon.T1.SprayWeapon(new BulletFactory.T1.BasicLaserFactory(Direction.Down))),
                        new KeyValuePair<JuggerWeaponInfo, Weapon.Weapon> (new JuggerWeaponInfo{ shootingChance = 80, shootingPosition = new Vector(40, 10)},
                        new Weapon.T1.SprayWeapon(new BulletFactory.T1.BasicLaserFactory(Direction.Down))),

                        new KeyValuePair<JuggerWeaponInfo, Weapon.Weapon> (new JuggerWeaponInfo{ shootingChance = 100, shootingPosition = new Vector()},
                        new Weapon.T3.TripleWeapon(new BulletFactory.T1.PlasmaFactory(Direction.Down))),

                        new KeyValuePair<JuggerWeaponInfo, Weapon.Weapon> (new JuggerWeaponInfo{ shootingChance = 100, shootingPosition = new Vector()},
                        new JuggerLaserWeapon(new BulletFactory.T1.BeamFactory(Direction.Down))),
                    };
                    weapons.ForEach(w => w.Value.Ammo.AppendDmgModifier((ref int i) => i *= 2));
                    weapons.Last().Value.ReloadTime += 1000; weapons.Last().Value.Ammo.AppendDmgModifier((ref int i) => i *= 2);
                    break;
                case Tier.T2:
                    weapons = new List<KeyValuePair<JuggerWeaponInfo, Weapon.Weapon>>
                    {
                        new KeyValuePair<JuggerWeaponInfo, Weapon.Weapon> (new JuggerWeaponInfo{ shootingChance = 75, shootingPosition = new Vector(-40, 10)},
                        new Weapon.T1.SprayWeapon(new BulletFactory.T1.BasicLaserFactory(Direction.Down))),
                        new KeyValuePair<JuggerWeaponInfo, Weapon.Weapon> (new JuggerWeaponInfo{ shootingChance = 80, shootingPosition = new Vector(-20, 10)},
                        new Weapon.T1.SprayWeapon(new BulletFactory.T1.BasicLaserFactory(Direction.Down))),
                        new KeyValuePair<JuggerWeaponInfo, Weapon.Weapon> (new JuggerWeaponInfo{ shootingChance = 85, shootingPosition = new Vector(20, 10)},
                        new Weapon.T1.SprayWeapon(new BulletFactory.T1.BasicLaserFactory(Direction.Down))),
                        new KeyValuePair<JuggerWeaponInfo, Weapon.Weapon> (new JuggerWeaponInfo{ shootingChance = 90, shootingPosition = new Vector(40, 10)},
                        new Weapon.T1.SprayWeapon(new BulletFactory.T1.BasicLaserFactory(Direction.Down))),

                        new KeyValuePair<JuggerWeaponInfo, Weapon.Weapon> (new JuggerWeaponInfo{ shootingChance = 100, shootingPosition = new Vector(40, 20)},
                        new Weapon.T3.TripleWeapon(new BulletFactory.T1.PlasmaFactory(Direction.Down))),
                        new KeyValuePair<JuggerWeaponInfo, Weapon.Weapon> (new JuggerWeaponInfo{ shootingChance = 100, shootingPosition = new Vector(-40, 20)},
                        new Weapon.T3.TripleWeapon(new BulletFactory.T1.PlasmaFactory(Direction.Down))),

                        new KeyValuePair<JuggerWeaponInfo, Weapon.Weapon> (new JuggerWeaponInfo{ shootingChance = 100, shootingPosition = new Vector()},
                        new JuggerLaserWeapon(new BulletFactory.T1.BeamFactory(Direction.Down))),
                    };
                    weapons.ForEach(w => w.Value.Ammo.AppendDmgModifier((ref int i) => i *= 3));
                    weapons.Last().Value.ReloadTime += 1000; weapons.Last().Value.Ammo.AppendDmgModifier((ref int i) => i *= 3);
                    break;
                case Tier.T3:
                    weapons = new List<KeyValuePair<JuggerWeaponInfo, Weapon.Weapon>>
                    {
                        new KeyValuePair<JuggerWeaponInfo, Weapon.Weapon> (new JuggerWeaponInfo{ shootingChance = 90, shootingPosition = new Vector(-40, 10)},
                        new Weapon.T1.SprayWeapon(new BulletFactory.T1.BasicLaserFactory(Direction.Down))),
                        new KeyValuePair<JuggerWeaponInfo, Weapon.Weapon> (new JuggerWeaponInfo{ shootingChance = 80, shootingPosition = new Vector(40, 10)},
                        new Weapon.T1.SprayWeapon(new BulletFactory.T1.BasicLaserFactory(Direction.Down))),

                        new KeyValuePair<JuggerWeaponInfo, Weapon.Weapon> (new JuggerWeaponInfo{ shootingChance = 100, shootingPosition = new Vector()},
                        new Weapon.T3.TripleWeapon(new BulletFactory.T1.PlasmaFactory(Direction.Down))),

                        new KeyValuePair<JuggerWeaponInfo, Weapon.Weapon> (new JuggerWeaponInfo{ shootingChance = 100, shootingPosition = new Vector()},
                        new JuggerLaserWeapon(new BulletFactory.T1.BeamFactory(Direction.Down))),
                    };
                    weapons.ForEach(w => w.Value.Ammo.AppendDmgModifier((ref int i) => i *= 2));
                    weapons.Last().Value.ReloadTime += 1000;
                    weapons.Last().Value.Ammo.AppendDmgModifier((ref int i) => i *= 2);
                    // base beam reduces armor permamently
                    weapons.Last().Value.Ammo.AppendOnHit(new OnHits.ArmorMeltEffect(-5));
                    break;
            }
        }
    }

    public class Juggernaut : EnemyShip
    {
        /**
         * temporary/permament hack
         * take increased damage, when core is hit, but take damage is called when IsHit is true
         * and there is no way to tell, what part was hit
         * possible solutions:
         * -add separate ship Core and subscribe to its damage taken to also take damage here: no action weapon, adjusting speed etc
         * -refactor to add IBody interface to shipAdder instead of IShip, but thats a solid refactor, only for this usecase, too much work
         */
        private Rectangle leftWing;
        private Rectangle rightWing;
        private Rectangle core;

        private bool coreWasHit = false;

        public override void Move()
        {
            base.Move();
            leftWing.X = Position.X;
            leftWing.Y = Position.Y;
            rightWing.X = Position.X + VALUES.WING_OFFSET_X;
            rightWing.Y = Position.Y;
            core.X = Position.X + VALUES.CORE_OFFSET_X;
            core.Y = Position.Y + VALUES.CORE_OFFSET_Y;
        }

        private bool testCollision(IMover projectile, in Rectangle rect)
        {
            return
                rect.X < projectile.Position.X + projectile.Size.X &&
                rect.X + rect.Size.Width > projectile.Position.X &&
                rect.Y < projectile.Position.Y + projectile.Size.Y &&
                rect.Y + rect.Size.Height > projectile.Position.Y;
        }

        public override bool IsHit(IMover projectile)
        {
            if (testCollision(projectile, in core))
            {
                coreWasHit = true;
            }
            return
                coreWasHit ||
                testCollision(projectile, in leftWing) ||
                testCollision(projectile, in rightWing);
        }

        public override void Draw(Graphics g, Pen p)
        {
            p.Color = Color.Orange;
            g.DrawRectangle(p, leftWing);
            p.Color = Color.Green;
            g.DrawRectangle(p, core);
            p.Color = Color.Orange;
            g.DrawRectangle(p, rightWing);
            base.Draw(g, p);
        }

        public override void SetDefaultMoveStrategy()
        {
            MovementStrategy = new FallDownStrategy(MaxSpeed);
        }

        private Vector LaunchPosition
        {
            get { return new Vector(core.X + core.Width/2, core.Y + core.Height/2); }
        }

        public override void Shoot()
        {
            Weapon.PullTheTrigger(LaunchPosition);
        }

        public override void TakeDamage(int val)
        {
            if (coreWasHit)
            {
                val *= 2;
                base.TakeTrueDamage(val);
                coreWasHit = false;
            }
            else
            {
                base.TakeDamage(val);
            }
        }

        public Juggernaut(Tier tier) :
            base(new Vector(VALUES.WIDTH / 2, 100))
        {
            this.Weapon = new JuggerWeapon(tier);
            int x = Position.X;
            int y = Position.Y;
            this.leftWing = new Rectangle(x, y, 60, 40);
            this.core = new Rectangle(x + leftWing.Width, y + 10, 30, 20);
            this.rightWing = new Rectangle(x + leftWing.Width + core.Width, y, 60, 40);
            this.Size = new Vector(leftWing.Width + core.Width + rightWing.Width, leftWing.Height);
            switch (tier)
            {
                case Tier.T1:
                    this.Defence = 120;
                    this.MaxHealth = 15 * VALUES.ENEMY_HEALTH;
                    this.Speed = new Vector(10, 0);
                    this.MaxSpeed = 15;
                    break;

                case Tier.T2:
                    this.Defence = 250;
                    this.MaxHealth = 25 * VALUES.ENEMY_HEALTH;
                    this.Speed = new Vector(10, 0);
                    this.MaxSpeed = 20;
                    break;

                case Tier.T3:
                    this.Defence = 500;
                    this.MaxHealth = 40 * VALUES.ENEMY_HEALTH;
                    this.Speed = new Vector(10, 0);
                    this.MaxSpeed = 25;
                    break;
            }
            this.Health = MaxHealth;
        }
    }
}
