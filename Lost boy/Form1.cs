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
        public static PlayerShip player;
        IPlayAble level;
        Timer timer;
        Setup.LevelSetup setup;
        private int playerStar = 1;
        private readonly List<int> emptyStars = new List<int>();
        private Tier currentPlayerTier = Tier.T1;

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
                case Keys.F:
                    level.HandlePlayer('f');
                    break;
                case Keys.C:
                    level.HandlePlayer('c');
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
                case Keys.D5:
                    setup.AppendEnemyToRoad(Enemies.EnemyTypes.Stealthy);
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

        private void SetTierFinishLevel(Setup.LevelInfoHolder info, ILevelBuilder builder)
        {
            builder
                .SetPlayer(player)
                .SetDescription("Tier finish level")
                .SetFinishedAction(TierFinishedAction)
                .SetContent(info);
            level = builder.Build();
        }

        private void InitializePlayer()
        {
            player = new PlayerShip();
            player.Gold = 250;
            player.Fuel = 15;
            player.Weapon = new Weapon.T2.DoubleWeapon(new BulletFactory.T2.MortalCoilFactory(Direction.Up));
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
            emptyStars.Add(playerStar);
            level = new StarMap(playerStar, STAR_MAP_FILE, emptyStars, PrepareNextLevel);
        }

        private void PrepareNextLevel(Setup.LevelInfoHolder info)
        {
            //player stuck
            if (info == null)
            {
                FinishTierLevel();
                return;
            }
            playerStar = info.id;
            LoadNextLevel(info);
        }

        private void LoadNextLevel(Setup.LevelInfoHolder info)
        {
            ILevelBuilder builder = null;
            if (info.type == LevelType.Classic)
            {
                string pathing = System.IO.Directory.GetCurrentDirectory();
                var lvlInfo = Setup.LevelReader.ReadLevel(pathing + @"\LevelFile.txt", info.type);
                lvlInfo.id = info.id;
                lvlInfo.tier = info.tier;
                lvlInfo.type = info.type;
                lvlInfo.difficulty = info.difficulty;
                builder = new ClassicLevelBuilder();
                SetLevel(lvlInfo, builder);
            }
            else if (info.type == LevelType.Event)
            {
                string pathing = System.IO.Directory.GetCurrentDirectory();
                builder = new Event.EventLevelBuilder();
                var lvlInfo = Setup.LevelReader.ReadLevel(pathing + @"\LevelFile.txt", info.type);
                lvlInfo.id = info.id;
                SetLevel(lvlInfo, builder);
            }
            else
            {
                if (info.type == LevelType.Meteor)
                    builder = new Meteor.MeteorLevelBuilder();
                else if (info.type == LevelType.Shop)
                    builder = new GroceryLevelBuilder();
                else if (info.type == LevelType.Boss)
                    builder = new BossLevelBuilder();
                SetLevel(info, builder);
            }
            level.Begin();
        }

        private void FinishTierLevel()
        {
            ILevelBuilder builder = new BossLevelBuilder();
            SetTierFinishLevel(new Setup.LevelInfoHolder { tier = currentPlayerTier, type = LevelType.Boss }, builder);
            level.Begin();
        }

        private void TierFinishedAction(bool playerWon)
        {
            if (playerWon)
            {
                if (currentPlayerTier == Tier.T3)
                    throw new NotImplementedException("You won");
                currentPlayerTier = (Tier)((int)currentPlayerTier + 1);
                player.Fuel = 15;
                StarMap.GenerateRandomMap(STAR_MAP_FILE, 150, currentPlayerTier);
                level = new StarMap(playerStar, STAR_MAP_FILE, new List<int> { }, PrepareNextLevel);
                level.Begin();
            }
            else
                throw new NotImplementedException("You lose");
        }

        private void LevelFinishedAction(bool playerWon)
        {
            if (playerWon)
            {
                if (player.Fuel == 0)
                {
                    FinishTierLevel();
                }
                else
                {
                    ActivateStarMap();
                }
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
            StarMap.GenerateRandomMap(STAR_MAP_FILE, 150, currentPlayerTier);
            level = new StarMap(playerStar, STAR_MAP_FILE, new List<int> { }, PrepareNextLevel);
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