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
        public class LevelSetup
        {
            private readonly string levelDescription;
            private int levelId;
            private int maxSpeed = 10;
            private Vector currentPoint;
            private Color currentColor = Color.Blue;
            private List<KeyValuePair<Vector, int>> currentRoad = new List<KeyValuePair<Vector, int>>();
            private readonly List<KeyValuePair<Vector, List<KeyValuePair<Vector, int>>>> roadsToStarts =
                new List<KeyValuePair<Vector, List<KeyValuePair<Vector, int>>>>();
            private readonly List<List<string>> enemyShips = new List<List<string>>();

            private List<KeyValuePair<Point, Point>> currentDrawable = new List<KeyValuePair<Point, Point>>();
            private readonly List<List<KeyValuePair<Point, Point>>> drawables = new List<List<KeyValuePair<Point, Point>>>();

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

            public List<KeyValuePair<Vector, List<KeyValuePair<Vector, int>>>> GetRoads()
            {
                return roadsToStarts;
            }

            public List<List<EnemyShip>> GetEnemies()
            {
                List<List<EnemyShip>> toRet = new List<List<EnemyShip>>();
                foreach (var eGroup in enemyShips)
                {
                    toRet.Add(new List<EnemyShip>(eGroup.Select(name =>
                        {
                            return ParseEnemy(name, null);
                        })));
                }
                return toRet;
            }

            public KeyValuePair<Vector, List<KeyValuePair<Vector, int>>> CloseRoad()
            {
                currentDrawable = new List<KeyValuePair<Point, Point>>();
                currentRoad = new List<KeyValuePair<Vector, int>>();
                return roadsToStarts.Last();
            }

            public void BeginRoad(Vector where)
            {
                enemyShips.Add(new List<string>());
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

            public void AppendEnemyToRoad(Enemies.EnemyTypes enemy)
            {
                enemyShips.Last().Add(enemy.ToString());
            }

            public void SaveLevelToFile(string fileName)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var eGroup in enemyShips)
                {
                    sb.AppendLine(String.Join(" ", eGroup));
                }

                sb.AppendLine("---");

                foreach (var rToS in roadsToStarts)
                {
                    sb.AppendLine(rToS.Key.ToString());
                    foreach(var road in rToS.Value)
                    {
                        sb.AppendFormat("{0} {1} {2}", road.Key.X, road.Key.Y, road.Value);
                        sb.AppendLine();
                    }
                }

                sb.AppendLine("===");
                File.AppendAllText(fileName, sb.ToString());
            }

            private EnemyShip ParseEnemy(string txt, PlayerShip p)
            {
                switch (txt)
                {
                case "Casual":
                    return new CasualEnemy(new Vector());
                case "Frosty":
                    return new FrostyEnemy(new Vector());
                case "Rocky":
                    return new RockyEnemy(new Vector());
                case "Tricky":
                    return new TrickyEnemy(p, new Vector());
                }
                return null;
            }

            public void SetFromFile(string fileName, PlayerShip p)
            {
                var lines = File.ReadLines(fileName);
                var iter = lines.GetEnumerator();
                while (iter.MoveNext())
                {
                    if (iter.Current == "---")
                        break;
                    enemyShips.Add(new List<string>());
                    enemyShips.Last().AddRange(iter.Current
                        .Split(' '));
                }
                List<KeyValuePair<Vector, int>> road = null;
                while(iter.MoveNext())
                {
                    var txt = iter.Current.Split(' ');
                    if (txt.Equals("==="))
                        return;
                    if (txt.Length != 3)
                    {
                        road = new List<KeyValuePair<Vector, int>>();
                        roadsToStarts.Add(
                            new KeyValuePair<Vector, List<KeyValuePair<Vector, int>>>
                            (new Vector(Int32.Parse(txt[2]), Int32.Parse(txt[5])),
                            road));
                    }
                    else
                    {
                        road.Add(new KeyValuePair<Vector,int>
                            (new Vector(Int32.Parse(txt[0]), Int32.Parse(txt[1])),
                                Int32.Parse(txt[2])));
                    }
                }
            }

            public LevelSetup()
            {
            }
        }
    }
}