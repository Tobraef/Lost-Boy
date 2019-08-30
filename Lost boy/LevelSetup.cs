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
            private readonly List<List<EnemyShip>> enemyShips = new List<List<EnemyShip>>();

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
                return enemyShips;
            }

            public KeyValuePair<Vector, List<KeyValuePair<Vector, int>>> CloseRoad()
            {
                currentDrawable = new List<KeyValuePair<Point, Point>>();
                currentRoad = new List<KeyValuePair<Vector, int>>();
                return roadsToStarts.Last();
            }

            public void BeginRoad(Vector where)
            {
                enemyShips.Add(new List<EnemyShip>());
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

            public void AppendEnemyToRoad(EnemyShip enemy)
            {
                enemyShips.Last().Add(enemy);
            }

            public LevelSetup()
            {
            }
        }
    }
}