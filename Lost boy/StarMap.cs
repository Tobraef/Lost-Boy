using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace Lost_boy
{
    class Star
    {
        public bool IsPressed(Vector where)
        {
            return where.DistanceFrom(MiddlePoint) <= 10;
        }

        public Color Color
        {
            get;
        }

        public Vector MiddlePoint
        {
            get;
        }

        public void Draw(Graphics g, Pen p)
        {
            p.Color = Color;
            g.DrawEllipse(p, MiddlePoint.X - 5, MiddlePoint.Y - 5, 10, 10);
        }

        public override string ToString()
        {
            return MiddlePoint.ToString();
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

        public Star(Vector position, Color color)
        {
            Color = color;
            MiddlePoint = position;
        }

        public Star(Vector position)
        {
            Color = ColorRandomizer();
            MiddlePoint = position;
        }
    }

    public class StarMap : IPlayAble
    {
        public event Action<bool> Finished;
        Star playerStar;
        private const int playerReach = 100;
        private readonly Dictionary<int, Star> starMap = new Dictionary<int, Star>();

        public void Begin()
        {
        }

        public void Draw(Graphics g, Pen p)
        {
            foreach (var pair in starMap)
            {
                pair.Value.Draw(g, p);
            }

            p.Color = Color.Red;
            g.DrawRectangle(p, playerStar.MiddlePoint.X - 20, playerStar.MiddlePoint.Y - 20, 40, 40);
            p.Color = Color.Green;
            g.DrawEllipse(p, playerStar.MiddlePoint.X - playerReach, playerStar.MiddlePoint.Y - playerReach, playerReach*2, playerReach*2);
        }

        public void Elapse()
        {
        }

        public void HandlePlayer(char key)
        {
        }

        public void HandlePlayer_KeyUp(char key)
        {
        }

        public void PrepareNextStage()
        {
        }

        public void HandlePlayer_Mouse(Vector where)
        {
            try
            {
                Star pressed = starMap.First(pair =>
                {
                    return pair.Value.IsPressed(where);
                }).Value;
                if (pressed.MiddlePoint.DistanceFrom(playerStar.MiddlePoint) < playerReach)
                {
                    playerStar = pressed;
                }
            } catch (Exception e) { return; }
        }

        private static bool IsReachable(List<Star> stars, Vector starPosition)
        {
            return stars.Find(s =>
            {
                double distance = s.MiddlePoint.DistanceFrom(starPosition);
                return distance < playerReach && distance > 10;
            }) != null;
        }

        public static void GenerateRandomMap(string fileToSave, int density)
        {
            List<Star> stars = new List<Star>();
            stars.Add(new Star(new Vector(40, 40)));
            for (int i = 0; i < density; ++i)
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
                sb.AppendLine(star.ToString() + " " + star.Color);
            }
            File.WriteAllText(fileToSave, sb.ToString());
        }

        private Color ParseColor(string color)
        {
            switch(color)
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

        public StarMap(int playerPosition, string file)
        {
            int id = 0;
            foreach (var line in File.ReadLines(file))
            {
                var array = line.Split(' ');
                starMap.Add(id++, new Star(
                    new Vector(Int32.Parse(array[2]), Int32.Parse(array[5])),
                    ParseColor(array[7])));
            }
            playerStar = starMap[playerPosition];
        }
    }
}
