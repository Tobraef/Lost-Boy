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
        IPlayAble level;
        private int lvlId;
        PlayerShip player;
        Timer timer;
        Setup.LevelSetup setup;
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
                case Keys.Tab:
                    // forbidden
                    LoadNextLevel();
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
                    string path = System.IO.Directory.GetCurrentDirectory();
                    setup.SaveLevelsToFile(path + @"\LevelFile.txt");
                    break;
                case Keys.Tab:
                    LoadNextLevel();
                    break;
                case Keys.N:
                    setup.FinishLevel();
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
            }
        }

        private void SetLevel(Setup.LevelInfoHolder info)
        {
            builder = new ClassicLevelBuilder();
            builder
                .SetPlayer(player)
                .SetDescription("Testing level")
                .SetDifficulty(Difficulty.Normal, lvlId)
                .SetDroppable(GetTestDrop())
                .SetFinishedAction(LevelFinishedAction)
                .SetContent(info);
            level = builder.Build();
        }

        private void InitializePlayer()
        {
            player = new PlayerShip();
            player.Weapon = new DoubleWeapon(new BasicLaserFactory(Direction.Up));
            // testing site below
            player.Weapon.Ammo = new FrostyLaserFactory(Direction.Up);
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
            drop.Add(new ShipSpeedBonus(new Vector()), 10);
            drop.Add(new ArmorMeltBonus(new Vector()), 10);
            drop.Add(new FrostBonus(new Vector()), 10);
            return drop;
        }

        private void PLAY()
        {
            setup = null;
            this.KeyDown -= SetupKeyHandle;
            this.MouseClick -= MousePop;
            this.KeyDown += KeyHandle;
        }

        private void LoadNextLevel()
        {
            string pathing = System.IO.Directory.GetCurrentDirectory();
            var lvlInfo = Setup.LevelReader.ReadLevel(pathing + @"\LevelFile.txt", ++lvlId);
            SetLevel(lvlInfo);
            level.Begin();
        }

        private void LevelFinishedAction(bool playerWon)
        {
            if (playerWon)
            {
                LoadNextLevel();
            }
            else
            {
                throw new NotImplementedException("You lose");
            }
        }

        public Form1()
        {
            InitializeComponent();
            this.BackColor = Color.Black;
            this.Size = new Size(VALUES.WIDTH, VALUES.HEIGHT + VALUES.PLAYER_HEIGHT);
            if (false)
            {
                setup = new Setup.LevelSetup();
                this.MouseClick += MousePop;
                this.KeyDown += SetupKeyHandle;
            }
            InitializePlayer();
            // LoadNextLevel();
            level = new Meteor.MeteorLevel();
            ((Meteor.MeteorLevel)level).Player = player;
            ((Meteor.MeteorLevel)level).SetDroppables(GetTestDrop(), Difficulty.Hard);
            level.Begin();
            PLAY();
            this.KeyUp += KeyUps;
            this.Paint += PaintGame;
            timer = new Timer();
            timer.Interval = 100;
            timer.Tick += new EventHandler(Elapse);
            timer.Start();
        }
    }
}