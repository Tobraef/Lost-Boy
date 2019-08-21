using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lost_boy
{
    public partial class Form1 : Form
    {
        private PlayerShip player;
        private List<EnemyShip> enemies;
        private List<EnemyShip> toRemoveEnemies = new List<EnemyShip>();
        private List<IProjectile> enemyProjectiles = new List<IProjectile>();
        private List<IProjectile> playersProjectiles = new List<IProjectile>();
        private List<IProjectile> toRemoveProjectiles = new List<IProjectile>();
        private Timer timer;

        private void PlayerBulletAdder(IProjectile bullet)
        {
            bullet.onDeath += () => toRemoveProjectiles.Add(bullet);
            playersProjectiles.Add(bullet);
        }

        private void EnemyBulletAdder(IProjectile bullet)
        {
            bullet.onDeath += () => toRemoveProjectiles.Add(bullet);
            enemyProjectiles.Add(bullet);
        }

        private void KeyHandle(object sender, KeyEventArgs args)
        {
            switch (args.KeyCode)
            {
                case Keys.A:
                    if (player.MaxSpeed > 0)
                        player.Speed = new Vector(-player.MaxSpeed, 0);
                    break;
                case Keys.D:
                    if (player.MaxSpeed > 0)
                        player.Speed = new Vector(player.MaxSpeed, 0);
                    break;
                case Keys.S:
                    player.Shoot();
                    break;
            }
        }

        private void KeyUps(object sender, KeyEventArgs args)
        {
            switch (args.KeyCode)
            {
                case Keys.A:
                    player.Speed = new Vector(0, 0);
                    break;
                case Keys.D:
                    player.Speed = new Vector(0, 0);
                    break;
            }
        }

        void Elapse(object sender, EventArgs e)
        {
            foreach (var enemy in enemies)
            {
                enemy.Move();
                if (enemy.Position.Y > VALUES.HEIGHT)
                    enemy.Teleport(enemy.Position.X, -enemy.Size.Y);

                if (enemy.Position.X < -enemy.Size.X - 10)
                    enemy.Teleport(VALUES.WIDTH, enemy.Position.Y);
                else if (enemy.Position.X > VALUES.WIDTH + enemy.Size.X + 10)
                    enemy.Teleport(0, enemy.Position.Y);

                enemy.Shoot();
                foreach (var bullet in playersProjectiles)
                {
                    bullet.Move();
                    if (bullet.Position.Y + bullet.Size.Y < 0)
                    {
                        toRemoveProjectiles.Add(bullet);
                    }
                    else if (enemy.IsHit(bullet))
                    {
                        bullet.AffectShip(enemy);
                    }
                }
            }

            player.Move();
            foreach (var bullet in enemyProjectiles)
            {
                bullet.Move();
                if (bullet.Position.Y > VALUES.HEIGHT)
                {
                    toRemoveProjectiles.Add(bullet);
                }
                else if (player.IsHit(bullet))
                {
                    bullet.AffectShip(player);
                }
            }

            foreach (var enemy in toRemoveEnemies)
            {
                enemies.Remove(enemy);
            }
            toRemoveEnemies.Clear();
            foreach (var bullet in toRemoveProjectiles)
            {
                playersProjectiles.Remove(bullet);
                enemyProjectiles.Remove(bullet);
            }
            toRemoveProjectiles.Clear();
            this.Refresh();
        }

        void PaintGame(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            Pen p = new Pen(Color.Black);
            player.Draw(g, p);
            foreach (var b in enemyProjectiles)
                b.Draw(g, p);
            foreach (var b in playersProjectiles)
                b.Draw(g, p);
            foreach (var b in enemies)
                b.Draw(g, p);
            player.Draw(g, p);

        }

        private Bonus GetRandomBonus(Vector position)
        {
            if (VALUES.BONUS_DROP_CHANCE > VALUES.random.Next(100))
            {
                switch (VALUES.random.Next(1,6))
                {
                    case 1:
                        return new LaserDamageBonus(position);
                    case 2:
                        return new BulletSizeChangeBonus(position);
                    case 3:
                        return new BurnBonus(position);
                    case 4:
                        return new BulletSpeedBonus(position);
                    case 5:
                        return new HealthBonus(position);
                    case 6:
                        return new WeaponReloadTimeBonus(position);
                    default:
                        return null;
                }
            }
            return null;
        }

        private void SetPlayer()
        {
            player = new PlayerShip();
            player.bulletAdder += PlayerBulletAdder;
            player.Weapon.Ammo.AppendOnHit(new OnHits.BurnChance(10, 3, 20));
        }

        private void SetEnemies()
        {
            enemies = new List<EnemyShip>
            {
                new RockyEnemy(new Vector(VALUES.WIDTH / 5,150)),
                new RockyEnemy(new Vector(VALUES.WIDTH * 2 / 5,150)),
                new RockyEnemy(new Vector(VALUES.WIDTH * 3 / 5,150)),
                new RockyEnemy(new Vector(VALUES.WIDTH * 4 / 5,150)),
                new FrostyEnemy(new Vector(450,50)),
                new FrostyEnemy(new Vector(550,50)),
                new FrostyEnemy(new Vector(650,50)),
                new FrostyEnemy(new Vector(750,50)),
                new TrickyEnemy(player, new Vector(50, 300)),
                new TrickyEnemy(player, new Vector(300, 300))
            };

            foreach (var e in enemies)
            {
                e.bulletAdder += EnemyBulletAdder;
                e.onDeath += () => toRemoveEnemies.Add(e);
                e.onDeath += () =>
                {
                    var bonus = GetRandomBonus(e.Position);
                    if (bonus != null)
                    {
                        bonus.onDeath += () => this.toRemoveProjectiles.Add(bonus);
                        enemyProjectiles.Add(bonus);
                    }
                };
                e.onDeath += () =>
                {
                    if (VALUES.GOLD_DROP_CHANCE > VALUES.random.Next(100))
                    {
                        var bonus = new GoldCoin(e.Position, VALUES.GOLD_AVERAGE_VALUE);
                        bonus.onDeath += () =>
                        {
                            this.toRemoveProjectiles.Add(bonus);
                        };
                        enemyProjectiles.Add(bonus);
                    }
                };
                e.SetDefaultMoveStrategy();
            }
        }

        public Form1()
        {
            InitializeComponent();
            this.BackColor = Color.Black;
            this.Size = new Size(VALUES.WIDTH, VALUES.HEIGHT + 200);
            SetPlayer();
            SetEnemies();
            this.KeyDown += KeyHandle;
            this.KeyUp += KeyUps;
            this.Paint += PaintGame;
            timer = new Timer();
            timer.Interval = 100;
            timer.Tick += new EventHandler(Elapse);
            timer.Start();
        }
    }
}