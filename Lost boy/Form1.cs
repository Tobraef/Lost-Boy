﻿using System;
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
        Dictionary<Bonus, int> drop = new Dictionary<Bonus, int>();
        bool LevelSetup = true;
        List<IPlayAble> levels = new List<IPlayAble>();
        IPlayAble level;
        PlayerShip player;
        Timer timer;
        Setup.LevelSetup setup;
        private string instructions;
        ILevelBuilder builder;
        private void KeyHandle(object sender, KeyEventArgs args)
        {
            switch (args.KeyCode)
            {
                case Keys.A:
                    level.HandlePlayer('A');
                    break;
                case Keys.D:
                    level.HandlePlayer('D');
                    break;
                case Keys.S:
                    level.HandlePlayer('S');
                    break;
                default:
                    break;
            }
        }

        private void SetupKeyHandle(object sender, KeyEventArgs args)
        {
            switch (args.KeyCode)
            {
                case Keys.D1:
                    setup.AppendEnemyToRoad(Enemies.EnemyTypes.Casual);
                    break;
                case Keys.D2:
                    setup.AppendEnemyToRoad(Enemies.EnemyTypes.Frosty);
                    break;
                case Keys.D3:
                    setup.AppendEnemyToRoad(Enemies.EnemyTypes.Tricky);
                    break;
                case Keys.D4:
                    setup.AppendEnemyToRoad(Enemies.EnemyTypes.Rocky);
                    break;
                case Keys.Space:
                    setup.SaveLevelToFile(@"E:\bedzie\Stworzone aplikacje c++\Lost boy\LevelFile.txt");
                    break;
                case Keys.Tab:
                    setup.SetFromFile(@"E:\bedzie\Stworzone aplikacje c++\Lost boy\LevelFile.txt", player);
                    SetLevels();
                    break;
                case Keys.R:
                    PLAY();
                    break;
                default:
                    break;
            }
        }

        private void KeyUps(object sender, KeyEventArgs args)
        {
            switch (args.KeyCode)
            {
                case Keys.A:
                    level.HandlePlayer_KeyUp('A');
                    break;
                case Keys.D:
                    level.HandlePlayer_KeyUp('D');
                    break;
            }
        }

        private void MousePop(object sender, MouseEventArgs m)
        {
            if (setup == null)
                return;
            switch (m.Button)
            {
                case MouseButtons.Left:
                    setup.NotePoint(new Vector(m.X, m.Y));
                    break;
                case MouseButtons.Right:
                    setup.CloseRoad();
                    break;
                case MouseButtons.Middle:
                    setup.BeginRoad(new Vector(m.X, m.Y));
                    break;
            }
        }

        private void Elapse(object sender, EventArgs e)
        {
            if (setup == null)
                level.Elapse();
            this.Refresh();
        }

        private void PaintGame(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            Pen p = new Pen(Color.Black);
            if (setup == null)
                level.Draw(g, p);
            else
            {
                setup.Draw(g, p);
                g.DrawString(instructions, new Font("Arial", 14), new SolidBrush(Color.White), new PointF(10, 10));
            }
        }

        private void SetLevels()
        {
            var enemiesIter = setup.GetEnemies().GetEnumerator();
            var roadsToStartsIter = setup.GetRoads().GetEnumerator();
            builder = new LevelBuilder(LevelType.Classic);
                builder
                    .SetPlayer(player)
                    .SetDescription("Testing level")
                    .SetDifficulty(Difficulty.Normal)
                    .SetDroppable(GetTestDrop());
            while (enemiesIter.MoveNext() && roadsToStartsIter.MoveNext())
            {
                builder
                    .SetEnemyGroup(enemiesIter.Current)
                    .SetStrategyForCurrentEnemies(roadsToStartsIter.Current.Key, roadsToStartsIter.Current.Value, 5);
            }
            levels.Add(builder.Build());
        }

        private void InitializePlayer()
        {
            player = new PlayerShip();
            player.Weapon.AppendOnShot(new OnShots.ColorChage(Color.Peru));
            player.Weapon.Ammo.AppendOnHit(new OnHits.BurnChance(10, 3, 20));
        }

        private List<EnemyShip> GetTestEnemy()
        {
            return new List<EnemyShip>
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
        }

        private Dictionary<Bonus, int> GetTestDrop()
        {
            Dictionary<Bonus, int> drop = new Dictionary<Bonus, int>();
            drop.Add(new BulletSizeChangeBonus(new Vector()), 10);
            drop.Add(new BulletSpeedBonus(new Vector()), 10);
            drop.Add(new BurnBonus(new Vector()), 10);
            drop.Add(new HealthBonus(new Vector()), 10);
            drop.Add(new LaserDamageBonus(new Vector()), 10);
            drop.Add(new WeaponReloadTimeBonus(new Vector()), 10);
            return drop;
        }

        private void PLAY()
        {
            level = levels.First();
            level.Begin();
            setup = null;
            this.KeyDown -= SetupKeyHandle;
            this.KeyDown += KeyHandle;
        }

        public Form1()
        {
            InitializeComponent();
            this.BackColor = Color.Black;
            this.Size = new Size(VALUES.WIDTH, VALUES.HEIGHT + 200);
            if (LevelSetup)
            {
                instructions =
                    "1 - casual\n2 - frosty\n3 - triky\n4 - rocky\n" +
                    "Mouse - mid = begin, left = note point, right = close\n" +
                    "Space - save to file, tab - set from file\n" +
                    "r - PLAY";
                setup = new Setup.LevelSetup();
                this.MouseClick += new MouseEventHandler(MousePop);
                this.KeyDown += SetupKeyHandle;
            }
            InitializePlayer();
            this.KeyUp += KeyUps;
            this.Paint += PaintGame;
            timer = new Timer();
            timer.Interval = 100;
            timer.Tick += new EventHandler(Elapse);
            timer.Start();
        }
    }
}