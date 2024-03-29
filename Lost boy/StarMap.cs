﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace Lost_boy
{
    class Star
    {
        public bool isValid = true;
        public bool IsPressed(Vector where)
        {
            return where.DistanceFrom(MiddlePoint) <= 10;
        }

        public Difficulty Difficulty
        {
            get;
            private set;
        }

        public Tier Tier
        {
            get;
            private set;
        }

        public Color Color
        {
            get;
            private set;
        }

        public Vector MiddlePoint
        {
            get;
            private set;
        }

        public LevelType Type
        {
            get;
            private set;
        }

        public void Draw(Graphics g, Pen p)
        {
            p.Color = Color;
            g.DrawEllipse(p, MiddlePoint.X - 5, MiddlePoint.Y - 5, 10, 10);
        }

        public override string ToString()
        {
            return MiddlePoint.ToString() + " " + Color + " " + Tier + " " + Difficulty + " " + Type;
        }

        private Tier TierRandomizer()
        {
            switch (VALUES.random.Next(3))
            {
                case 0: return Tier.T1;
                case 1: return Tier.T2;
                case 2: return Tier.T3;
            }
            return Tier.T1;
        }

        private Difficulty DifficultyRandomizer()
        {
            switch (VALUES.random.Next(3))
            {
                case 0: return Difficulty.Easy;
                case 1: return Difficulty.Normal;
                case 2: return Difficulty.Hard;
            }
            return Difficulty.None;
        }

        private LevelType TypeRandomizer()
        {
            switch (VALUES.random.Next(12))
            {
                case 1: return LevelType.Meteor;
                case 2: return LevelType.Meteor;
                case 3: return LevelType.Event;
                case 4: return LevelType.Event;
                case 5: return LevelType.Shop;
                case 6: return LevelType.Shop;
                case 7: return LevelType.Boss;
                default:
                    return LevelType.Classic;
            }
        }

        private Color ColorRandomizer()
        {
            switch (VALUES.random.Next(10))
            {
                case 1: return Color.Blue;
                case 2: return Color.Green;
                case 3: return Color.Yellow;
                case 4: return Color.Aquamarine;
                case 5: return Color.Red;
                case 6: return Color.Orange;
                case 7: return Color.Purple;
                case 8: return Color.Pink;
                case 9: return Color.White;
                default:
                    return Color.Brown;
            }
        }

        public Star(Vector position, Color c, Tier tier, Difficulty diff, LevelType type, bool valid)
        {
            Color = c;
            MiddlePoint = position;
            isValid = valid;
            Type = type;
            Tier = tier;
            Difficulty = diff;
        }

        public Star(Vector position, Color color)
        {
            isValid = false;
            Color = color;
            MiddlePoint = position;
        }

        public Star(Vector position, Tier tier)
        {
            MiddlePoint = position;
            Tier = tier;
            Color = ColorRandomizer();
            Difficulty = DifficultyRandomizer();
            Type = TypeRandomizer();
        }

        public Star(Vector position)
        {
            MiddlePoint = position;
            Tier = TierRandomizer();
            Color = ColorRandomizer();
            Difficulty = DifficultyRandomizer();
            Type = TypeRandomizer();
        }
    }

    public class StarMap : IPlayAble
    {
        public event Action<bool> Finished;
        private KeyValuePair<int, Star> playerStar;
        private const int playerReach = 100;
        private readonly Dictionary<int, Star> starMap = new Dictionary<int, Star>();
        private IPlayAble secondView;
        private Action<MouseEventArgs> currentMouseHandler;
        private Action<Graphics, Pen> currentDraw;
        private string instructions = "C - equipment\nF - Assembly";

        private class Label
        {
            public string Text { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
        }
        private Label label = null;
        private Label fuelLabel;

        public void Begin()
        {
            secondView = null;
            currentDraw = DefaultDraw;
            currentMouseHandler = DefaultMouseHandler;
        }

        private void DefaultDraw(Graphics g, Pen p)
        {
            foreach (var pair in starMap)
            {
                pair.Value.Draw(g, p);
            }

            p.Color = Color.White;
            if (label != null)
                g.DrawString(label.Text, VALUES.FONT, p.Brush, label.X, label.Y);
            g.DrawString(instructions, VALUES.FONT, p.Brush, 0, 0);
            g.DrawString(fuelLabel.Text, VALUES.FONT, p.Brush, fuelLabel.X, fuelLabel.Y);

            p.Color = Color.Red;
            g.DrawRectangle(p, playerStar.Value.MiddlePoint.X - 20, playerStar.Value.MiddlePoint.Y - 20, 40, 40);
            p.Color = Color.Green;
            g.DrawEllipse(p,
                playerStar.Value.MiddlePoint.X - playerReach,
                playerStar.Value.MiddlePoint.Y - playerReach,
                playerReach * 2, playerReach * 2);
        }

        private void DelegateDraw(Graphics g, Pen p)
        {
            secondView.Draw(g, p);
        }

        public void Draw(Graphics g, Pen p)
        {
            currentDraw(g, p);
        }

        public void Elapse()
        {
        }

        private void GrantControlToSecondView()
        {
            currentMouseHandler = DelegateMouseHandler;
            currentDraw = DelegateDraw;
        }

        private void RetrieveControl()
        {
            currentMouseHandler = DefaultMouseHandler;
            currentDraw = DefaultDraw;
        }

        private void PrepareSecondView()
        {
            secondView.Finished += b => { RetrieveControl(); secondView = null; };
            GrantControlToSecondView();
            secondView.Begin();
        }

        public void HandlePlayer(char key)
        {
            if ((key == ' ' && playerStar.Value.isValid) || Form1.player.Fuel == 0)
                Finished(true);
            else if (key == 'c')
            {
                secondView = new ChangingRoom();
                PrepareSecondView();

            }
            else if (key == 'f')
            {
                secondView = new AssemblyRoom();
                PrepareSecondView();
            }
        }

        public void HandlePlayer_KeyUp(char key)
        {
        }

        public void PrepareNextStage()
        {
        }

        public void HandlePlayerTravel(Vector where)
        {
            if (Form1.player.Fuel == 0)
                return;
            try
            {
                KeyValuePair<int, Star> pressed = starMap.First(pair =>
                {
                    return pair.Value.IsPressed(where);
                });
                if (playerStar.Key != pressed.Key &&
                    pressed.Value.MiddlePoint.DistanceFrom(playerStar.Value.MiddlePoint) < playerReach)
                {
                    playerStar = pressed;
                    UpdateFuel();
                }
            }
            catch (Exception e) { label = null; }
        }

        public void HandlePlayerInfoDisplay(Vector where)
        {
            try
            {
                Star pressed = starMap.First(pair =>
                {
                    return pair.Value.IsPressed(where);
                }).Value;
                label = new Label
                {
                    Text = !pressed.isValid ?
                           "Empty" :
                           "Difficulty: " + pressed.Difficulty +
                           "\nTier: " + pressed.Tier +
                           "\nType: " + pressed.Type,
                    X = pressed.MiddlePoint.X,
                    Y = pressed.MiddlePoint.Y + 10
                };
            }
            catch (Exception e) { label = null; }
        }

        private void DefaultMouseHandler(MouseEventArgs m)
        {
            switch (m.Button)
            {
                case MouseButtons.Left:
                    HandlePlayerTravel(new Vector(m.X, m.Y));
                    break;
                case MouseButtons.Right:
                    HandlePlayerInfoDisplay(new Vector(m.X, m.Y));
                    break;
                case MouseButtons.Middle:
                    break;
            }
        }

        private void DelegateMouseHandler(MouseEventArgs m)
        {
            secondView.HandlePlayer_Mouse(m);
        }

        public void HandlePlayer_Mouse(MouseEventArgs m)
        {
            currentMouseHandler(m);
        }

        private static bool IsReachable(List<Star> stars, Vector starPosition)
        {
            return stars.Find(s =>
            {
                double distance = s.MiddlePoint.DistanceFrom(starPosition);
                return distance < playerReach && distance > 10;
            }) != null;
        }

        public static void GenerateRandomMap(string fileToSave, int density, Tier tier)
        {
            int i = 0;
            List<Star> stars = new List<Star>();
            for (; i < 10; ++i)
            {
                Vector where = new Vector(VALUES.random.Next(VALUES.WIDTH), VALUES.random.Next(VALUES.HEIGHT));
                stars.Add(new Star(where));
            }
            for (; i < density - 10; ++i)
            {
                Vector where = new Vector(VALUES.random.Next(VALUES.WIDTH), VALUES.random.Next(VALUES.HEIGHT));
                while (!IsReachable(stars, where))
                {
                    where = new Vector(VALUES.random.Next(VALUES.WIDTH), VALUES.random.Next(VALUES.HEIGHT));
                }
                stars.Add(new Star(where, tier));
            }
            for (; i < density; ++i)
            {
                Vector where = new Vector(VALUES.random.Next(VALUES.WIDTH), VALUES.random.Next(VALUES.HEIGHT));
                while (!IsReachable(stars, where))
                {
                    where = new Vector(VALUES.random.Next(VALUES.WIDTH), VALUES.random.Next(VALUES.HEIGHT));
                }
                stars.Add(new Star(where));
            }

            StringBuilder sb = new StringBuilder();
            foreach (var star in stars)
            {
                sb.AppendLine(star.ToString());
            }
            File.WriteAllText(fileToSave, sb.ToString());
        }

        private Setup.LevelInfoHolder FinishStarMap()
        {
            if (!playerStar.Value.isValid)
                return null;
            return new Setup.LevelInfoHolder
            {
                id = playerStar.Key,
                tier = playerStar.Value.Tier,
                difficulty = playerStar.Value.Difficulty,
                type = playerStar.Value.Type
            };
        }

        private Color ParseColor(string color)
        {
            switch (color)
            {
                case "[Blue]": return Color.Blue;
                case "[Green]": return Color.Green;
                case "[Yellow]": return Color.Yellow;
                case "[Aquamarine]": return Color.Aquamarine;
                case "[Red]": return Color.Red;
                case "[Orange]": return Color.Orange;
                case "[Purple]": return Color.Purple;
                case "[Pink]": return Color.Pink;
                case "[White]": return Color.White;
                default:
                    return Color.Brown;
            }
        }

        private Tier ParseTier(string tier)
        {
            switch (tier)
            {
                case "T1": return Tier.T1;
                case "T2": return Tier.T2;
                case "T3": return Tier.T3;
            }
            return Tier.T1;
        }

        private Difficulty ParseDifficulty(string diff)
        {
            switch (diff)
            {
                case "Easy": return Difficulty.Easy;
                case "Normal": return Difficulty.Normal;
                case "Hard": return Difficulty.Hard;
            }
            return Difficulty.None;
        }

        private LevelType ParseType(string type)
        {
            switch (type)
            {
                case "Classic": return LevelType.Classic;
                case "Event": return LevelType.Event;
                case "Meteor": return LevelType.Meteor;
                case "Shop": return LevelType.Shop;
                case "Boss": return LevelType.Boss;
            }
            throw new NotImplementedException("StarMap - Parse level type error");
        }

        private void UpdateFuel()
        {
            Form1.player.Fuel--;
            fuelLabel = new Label
            {
                X = 0,
                Y = 60,
                Text = "Fuel: " + Form1.player.Fuel
            };
        }

        public StarMap(int playerPosition, string file, List<int> emptied, Action<Setup.LevelInfoHolder> finisher)
        {
            UpdateFuel();
            int id = 0;
            Finished += b =>
            {
                finisher(FinishStarMap());
            };
            foreach (var line in File.ReadLines(file))
            {
                id++;
                if (emptied.Contains(id))
                {
                    var array = line.Split(' ');
                    starMap.Add(id, new Star(
                        new Vector(Int32.Parse(array[2]), Int32.Parse(array[5])),
                        ParseColor(array[7])));
                }
                else
                {
                    var array = line.Split(' ');
                    starMap.Add(id, new Star(
                        new Vector(Int32.Parse(array[2]), Int32.Parse(array[5])),
                        ParseColor(array[7]),
                        ParseTier(array[8]),
                        ParseDifficulty(array[9]),
                        ParseType(array[10]),
                        true));
                }
            }
            playerStar = new KeyValuePair<int, Star>(playerPosition, starMap[playerPosition]);
            currentDraw = DefaultDraw;
            currentMouseHandler = DefaultMouseHandler;
        }
    }
}