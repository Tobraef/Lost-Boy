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
        PlayerShip player;
        Timer timer;
        Setup.LevelSetup setup;
        private int playerStar = 1;
        private readonly List<int> emptyStars = new List<int>();

        private readonly string STAR_MAP_FILE = System.IO.Directory.GetCurrentDirectory() + @"\StarMapFile.txt";

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
                case Keys.Space:
                    level.HandlePlayer(' ');
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

        private void LevelMouseHandle(object sender, MouseEventArgs m)
        {
            level.HandlePlayer_Mouse(m);
        }

        private void Elapse(object sender, EventArgs e)
        {
            level.Elapse();
            Refresh();
            //Task logic = new Task(level.Elapse);
            //Task drawing = new Task(this.Refresh);
            //Task.Run(level.Elapse);
            //Task.Run(Refresh);
            level.PrepareNextStage();
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

        private void SetLevel(Setup.LevelInfoHolder info, ILevelBuilder builder)
        {
            builder
                .SetPlayer(player)
                .SetDescription("Testing level")
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

        private void PLAY()
        {
            setup = null;
            this.KeyDown -= SetupKeyHandle;
            this.MouseClick -= MousePop;
            this.KeyDown += KeyHandle;
            this.MouseClick += LevelMouseHandle;
        }

        private void ActivateStarMap()
        {
            level = new StarMap(playerStar, STAR_MAP_FILE, emptyStars, PrepareNextLevel);
        }

        private void PrepareNextLevel(Setup.LevelInfoHolder info)
        {
            emptyStars.Add(playerStar);
            playerStar = info.id;
            LoadNextLevel(info);
        }

        private void LoadNextLevel(Setup.LevelInfoHolder info)
        {
            ILevelBuilder builder = null;
            if (info.type == LevelType.Classic)
            {
                string pathing = System.IO.Directory.GetCurrentDirectory();
                var lvlInfo = Setup.LevelReader.ReadLevel(pathing + @"\LevelFile.txt", VALUES.random.Next(1, VALUES.MAX_LVL_ID + 1));
                lvlInfo.id = info.id;
                lvlInfo.tier = info.tier;
                lvlInfo.type = info.type;
                builder = new ClassicLevelBuilder();
                SetLevel(lvlInfo, builder);
            }
            else
            {
                if (info.type == LevelType.Meteor)
                    builder = new Meteor.MeteorLevelBuilder();
                else if (info.type == LevelType.Event)
                    ;// builder = new EventLevelBuilder();
                SetLevel(info, builder);
            }
            level.Begin();
        }

        private void LevelFinishedAction(bool playerWon)
        {
            if (playerWon)
            {
                ActivateStarMap();
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
            StarMap.GenerateRandomMap(STAR_MAP_FILE, 100);
            level = new StarMap(playerStar, STAR_MAP_FILE, new List<int>{ }, PrepareNextLevel);
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