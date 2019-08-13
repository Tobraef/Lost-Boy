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
                    player.Speed = new Vector(-VALUES.PLAYER_SPEED, 0);
                    break;
                case Keys.D:
                    player.Speed = new Vector(VALUES.PLAYER_SPEED, 0);
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
            foreach (var enemy in toRemoveEnemies)
            {
                enemies.Remove(enemy);
            }
            toRemoveEnemies.Clear();
            foreach (var enemy in enemies)
            {
                enemy.Move();
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

        public Form1()
        {
            InitializeComponent();
            this.BackColor = Color.Black;
            this.Size = new Size(VALUES.WIDTH, VALUES.HEIGHT + 200);
            player = new PlayerShip();
            player.bulletAdder += PlayerBulletAdder;
            player.Weapon.Ammo.AppendOnHit(new OnHits.GoldDig(EnemyBulletAdder, VALUES.GOLD_AVERAGE_VALUE, VALUES.GOLD_DROP_CHANCE));
            enemies = new List<EnemyShip>
            {
                new EnemyShip(new Vector(50,50), new Vector()),
                new EnemyShip(new Vector(150,50), new Vector()),
                new EnemyShip(new Vector(250,50), new Vector()),
                new EnemyShip(new Vector(350,50), new Vector()),
                new EnemyShip(new Vector(450,50), new Vector()),
                new EnemyShip(new Vector(550,50), new Vector()),
                new EnemyShip(new Vector(650,50), new Vector()),
                new EnemyShip(new Vector(750,50), new Vector()),
            };
            int i = 1;

            foreach (var e in enemies)
            {
                e.bulletAdder += EnemyBulletAdder;
                e.ShootingChance = new Random(i++);
                e.onDeath += () => toRemoveEnemies.Add(e);
                e.onDeath += () =>
                {
                    if (VALUES.BONUS_DROP_CHANCE > VALUES.random.Next(100))
                    {
                        var bonus = new LaserDamageBonus(e.Position);
                        bonus.onDeath += () =>
                        {
                            this.toRemoveProjectiles.Add(bonus);
                        };
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
            }
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