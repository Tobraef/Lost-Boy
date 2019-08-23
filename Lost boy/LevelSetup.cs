using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Lost_boy
{
    namespace Setup
    {
        public class LevelSetup
        {
            private readonly string levelDescription;
            private int levelId;
            private int currentRoadId;
            private int maxSpeed = 25;
            private Vector currentPoint;
            private Color currentColor = Color.Blue;
            private List<KeyValuePair<Vector, int>> currentRoad = new List<KeyValuePair<Vector, int>>();
            private readonly Dictionary<int, List<KeyValuePair<Vector, int>>> roadsToIds = new Dictionary<int, List<KeyValuePair<Vector, int>>>();

            private  List<KeyValuePair<Point, Point>> currentDrawable = new List<KeyValuePair<Point, Point>>();
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

            public void CloseRoad()
            {
                roadsToIds.Add(++currentRoadId, currentRoad);
                currentDrawable = new List<KeyValuePair<Point, Point>>();
                currentRoad = new List<KeyValuePair<Vector, int>>();
            }

            public void BeginRoad(Vector where)
            {
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
        }
    }
}
