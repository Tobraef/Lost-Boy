using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace Lost_boy
{
    namespace Setup
    {
        public class LevelInfoHolder
        {
            public int id;
            public Difficulty difficulty;
            public Tier tier;
            public LevelType type;
            public List<string> data;
        }

        class RoadSetup
        {
            private readonly int maxSpeed = 10;
            private Vector currentPoint;
            private Color currentColor = Color.Blue;
            private List<KeyValuePair<Vector, int>> currentRoad = new List<KeyValuePair<Vector, int>>();
            private readonly List<KeyValuePair<Vector, List<KeyValuePair<Vector, int>>>> roadsToStarts =
                new List<KeyValuePair<Vector, List<KeyValuePair<Vector, int>>>>();

            private List<KeyValuePair<Point, Point>> currentDrawable = new List<KeyValuePair<Point, Point>>();
            private readonly List<List<KeyValuePair<Point, Point>>> drawables = new List<List<KeyValuePair<Point, Point>>>();

            public List<KeyValuePair<Vector, List<KeyValuePair<Vector, int>>>> GetRoads()
            {
                return roadsToStarts;
            }

            public void CloseRoad()
            {
                currentDrawable = new List<KeyValuePair<Point, Point>>();
                currentRoad = new List<KeyValuePair<Vector, int>>();
            }

            public void BeginRoad(Vector where)
            {
                roadsToStarts.Add(new KeyValuePair<Vector, List<KeyValuePair<Vector, int>>>(where, currentRoad));
                currentPoint = where;
                currentDrawable.Add(new KeyValuePair<Point, Point>(where, where));
                drawables.Add(currentDrawable);
            }

            public void NotePoint(Vector where)
            {
                currentDrawable.Add(new KeyValuePair<Point, Point>(currentDrawable.LastOrDefault().Value, where));
                var length = Math.Sqrt(Math.Pow(where.X - currentPoint.X, 2) + Math.Pow(where.Y - currentPoint.Y, 2));
                int times = (int)length / maxSpeed;
                Vector speed = times == 0 ? new Vector() : (where - currentPoint) / times;
                currentPoint = where;
                currentRoad.Add(new KeyValuePair<Vector, int>(speed, times));
            }

            public void Draw(Graphics g, Pen p)
            {
                foreach (var road in drawables)
                {
                    p.Color = currentColor;
                    foreach (var line in road)
                    {
                        g.DrawLine(p, line.Key, line.Value);
                    }
                    NextColor();
                }
                currentColor = Color.Blue;
            }

            private void NextColor()
            {
                if (currentColor == Color.Blue)
                    currentColor = Color.Green;
                else if (currentColor == Color.Green)
                    currentColor = Color.Pink;
                else if (currentColor == Color.Pink)
                    currentColor = Color.Red;
                else
                    currentColor = Color.Blue;
            }
        }

        public class LevelSetup
        {
            private readonly List<LevelInfoHolder> levels = new List<LevelInfoHolder>();
            private List<List<string>> enemyShips = new List<List<string>>();
            private RoadSetup roadSetup;
            private readonly string instructions =
                    "1 - casual\n2 - frosty\n3 - triky\n4 - rocky\n5 - stealthy\n" +
                    "Mouse - mid = begin, left = note point, right = close\n" +
                    "Space - save to file\n" +
                    "N - finish level\n" +
                    "tab - next level\n" +
                    "R - PLAY";

            public List<LevelInfoHolder> GetLevels()
            {
                return levels;
            }

            public void Draw(Graphics g, Pen p)
            {
                roadSetup.Draw(g, p);
                g.DrawString(instructions, new Font("Arial", 14), new SolidBrush(Color.White), new PointF(10, 10));
            }

            public void BeginRoad(Vector where)
            {
                where.X += where.X - VALUES.WIDTH / 2;
                where.Y += where.Y - VALUES.HEIGHT / 2;
                enemyShips.Add(new List<string>());
                roadSetup.BeginRoad(where);
            }

            public void NotePoint(Vector where)
            {
                roadSetup.NotePoint(where);
            }

            public void CloseRoad()
            {
                roadSetup.CloseRoad();
            }
            
            private List<string> ConvertEnemies(List<List<string>> list)
            {
                List<string> toRet = new List<string>();
                StringBuilder ss = new StringBuilder();
                foreach (var egroup in list)
                {
                    foreach (var e in egroup)
                    {
                        ss.Append(e.ToString());
                    }
                    ss.AppendLine();
                    toRet.Add(ss.ToString());
                    ss.Clear();
                }
                return toRet;
            }

            private List<string> ConvertRoads()
            {
                List<string> lines = new List<string>();
                StringBuilder sb = new StringBuilder();
                foreach (var rToS in roadSetup.GetRoads())
                {
                    sb.AppendLine(rToS.Key.ToString());
                    foreach (var road in rToS.Value)
                    {
                        sb.AppendFormat("{0} {1} {2}", road.Key.X, road.Key.Y, road.Value);
                        sb.AppendLine();
                    }
                    lines.Add(sb.ToString());
                    sb.Clear();
                }
                return lines;
            }

            public void FinishLevel()
            {
                LevelInfoHolder level = new LevelInfoHolder();
                level.type = LevelType.Classic;
                var lines = ConvertEnemies(enemyShips);
                lines.Add("---");
                lines.AddRange(ConvertRoads());
                level.data = lines;
                levels.Add(level);
                roadSetup = new RoadSetup();
                enemyShips = new List<List<string>>();
            }

            public void ThisLvlAsMeteor()
            {
                roadSetup = new RoadSetup();
                enemyShips.Clear();
                LevelInfoHolder level = new LevelInfoHolder();
                level.type = LevelType.Meteor;
                level.data = new List<string> { new Meteor.MeteorDispenser(0).ToString() };
                levels.Add(level);
                roadSetup = new RoadSetup();
                enemyShips = new List<List<string>>();
            }

            public void AppendEnemyToRoad(Enemies.EnemyTypes enemy)
            {
                enemyShips.Last().Add(enemy.ToString());
            }

            public void SaveLevelsToFile(string fileName)
            {
                if (!File.Exists(fileName))
                    File.Create(fileName);

                if (enemyShips.Count != 0)
                    FinishLevel();
                var txt = File.ReadLines(fileName);
                int id = Int32.Parse(
                txt
                    .Where(line => line.Contains("==="))
                    .Last()
                    .Split(' ')
                    .ElementAt(1));
                StringBuilder sb = new StringBuilder();
                foreach (var lvl in levels)
                {
                    sb.AppendLine("=== " + ++id + lvl.type.ToString());

                    foreach (var line in lvl.data)
                    {
                        sb.Append(line);
                    }
                }

                File.AppendAllText(fileName, sb.ToString());
            }

            public LevelSetup()
            {
                roadSetup = new RoadSetup();
            }
        }

        public class LevelReader
        {
            private static LevelType ParseLevelType(string s)
            {
                switch (s)
                {
                    case "Classic":
                        return LevelType.Classic;
                    case "Meteor":
                        return LevelType.Meteor;
                    case "Event":
                        return LevelType.Classic;
                }
                throw new NotImplementedException("No such level type as " + s);
            }

            public static LevelInfoHolder ReadLevel(string fileName, int levelId)
            {
                var lines = File.ReadLines(fileName);
                var iter = lines.GetEnumerator();
                string id = levelId.ToString();

                LevelInfoHolder lvl = new LevelInfoHolder
                {
                    id = ++levelId,
                    data = new List<string>()
                };
                var begin = lines
                    .SkipWhile(line => !(line.Contains("===") && line.Contains(id)))
                    .ToList();
                lvl.type = ParseLevelType(begin.First().Split(' ').Last());
                lvl.data = begin
                    .Skip(1)
                    .TakeWhile(line => !line.Contains("==="))
                    .ToList();
                return lvl;
            }
        }
    }
}