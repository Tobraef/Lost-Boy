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
        Dictionary<Bonus, int> drop = new Dictionary<Bonus,int>();
        bool LevelSetup = true;
        IPlayAble level;
        PlayerShip player;
        Timer timer;
        Setup.LevelSetup setup;
        Dictionary<Vector, List<KeyValuePair<Vector, int>>> roads = 
            new Dictionary<Vector, List<KeyValuePair<Vector, int>>>();
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
                case Keys.L:
                    if (setup != null)
                        setup.BeginRoad(new Vector(MousePosition.X, MousePosition.Y));
                    break;
                case Keys.Space:
                    var r = setup.GetRoads();
                    foreach (var road in r)
                        roads.Add(road.Key, road.Value);
                    InitializeLevel();
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

        void Elapse(object sender, EventArgs e)
        {
            if (setup == null)
                level.Elapse();
            this.Refresh();
        }

        void PaintGame(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            Pen p = new Pen(Color.Black);
            if (setup == null)
                level.Draw(g, p);
            else
                setup.Draw(g, p);
        }

        private void InitializePlayer()
        {
            player = new PlayerShip();
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

        private void InitializeLevel()
        {
            setup = null;
            ILevelBuilder builder = new LevelBuilder(LevelType.Classic);
            level = builder
                .SetPlayer(player)
                .SetDescription("Testing level")
                .SetDifficulty(Difficulty.Easy)
                .SetDroppable(GetTestDrop())
                .CreateEnemy(Enemies.EnemyTypes.Casual)
                .CreateEnemy(Enemies.EnemyTypes.Casual)
                .CreateEnemy(Enemies.EnemyTypes.Casual)
                .CreateEnemy(Enemies.EnemyTypes.Casual)
                .SetStrategyForCurrentEnemies(
                    roads.First().Key, roads.First().Value, 10)
                .Build();
            level.Begin();
        }

        public Form1()
        {
            InitializeComponent();

            this.BackColor = Color.Black;
            this.Size = new Size(VALUES.WIDTH, VALUES.HEIGHT + 200);
            if (LevelSetup)
            {
                setup = new Setup.LevelSetup();
                this.MouseClick += new MouseEventHandler(MousePop);
            }
            InitializePlayer();
            var es = this.GetTestEnemy();
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