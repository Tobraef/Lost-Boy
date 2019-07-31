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
        private Timer timer;

        private void PlayerBulletAdder(IProjectile bullet)
        {
            bullet.TresholdPass = () => playersProjectiles.Remove(bullet);
            playersProjectiles.Add(bullet);
        }
        private void EnemyBulletAdder(IProjectile bullet)
        {
            bullet.TresholdPass = () => playersProjectiles.Remove(bullet);
            enemyProjectiles.Add(bullet);
        }
        void KeyHandle(object sender, KeyEventArgs args)
        {
            switch(args.KeyCode)
            {
                case Keys.A:
                    player.Move(-5);
                    break;
                case Keys.D:
                    player.Move(5);
                    break;
                case Keys.S:
                    player.Shoot(PlayerBulletAdder);
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
                enemy.Shoot(EnemyBulletAdder);
                foreach (var bullet in playersProjectiles)
                {
                    bullet.Move();
                    if (enemy.IsHit(bullet))
                    {
                        bullet.AffectShip(enemy);
                    }
                }
            }

            foreach (var bullet in enemyProjectiles)
            {
                bullet.Move();
                if (player.IsHit(bullet))
                {
                    bullet.AffectShip(player);
                }
            }
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
            this.Size = new Size(VALUES.WIDTH, VALUES.HEIGHT + 200);
            player = new PlayerShip();
            enemies = new List<EnemyShip>
            {
                new EnemyShip(new Vector(50,50), new Vector()),
                new EnemyShip(new Vector(150,50), new Vector()),
                new EnemyShip(new Vector(250,50), new Vector()),
                new EnemyShip(new Vector(350,50), new Vector()),
            };
            int i = 1;
            foreach (var e in enemies)
            {
                e.ShootingChance = new Random(i++);
                e.OnDeath = (EnemyShip ship) => toRemoveEnemies.Add(ship);
            }
            this.KeyDown += KeyHandle;
            this.Paint += PaintGame;
            timer = new Timer();
            timer.Interval = 100;
            timer.Tick += new EventHandler(Elapse);
            timer.Start();
        }
    }
}
