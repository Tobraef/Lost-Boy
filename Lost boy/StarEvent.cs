using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace Lost_boy.Event
{
    public class PlayerDependentEvent : IEvent
    {
        private readonly Dictionary<Vector, TextBox> options = new Dictionary<Vector, TextBox>();
        private readonly List<Vector> correctOptions = new List<Vector>();
        public event Action<bool> PlayerChose;

        public string Description
        {
            get;
            private set;
        }

        public void Draw(Graphics g, Pen p)
        {
            p.Color = Color.White;
            g.DrawString(Description, VALUES.FONT, p.Brush, new PointF(50, 50));
            foreach (var box in options.Values)
            {
                box.Draw(g, p);
            }
        }

        public void AcceptPlayerChoice(Vector choice)
        {
            var clicked = options
                .Keys
                .Where(pos =>
                pos.X <= choice.X &&
                pos.X + 100 > choice.X &&
                pos.Y <= choice.Y &&
                pos.Y + 50 > choice.Y);
            if (clicked.Count() != 0)
            {
                if (correctOptions.Exists(correctOption =>
                    clicked.First().X == correctOption.X &&
                    clicked.First().Y == correctOption.Y))
                    PlayerChose(true);
                else
                    PlayerChose(false);
            }
        }

        public PlayerDependentEvent(string[] input, List<string> options, int correctAnswer, List<string> playerRequirements)
        {
            Description = input[0];
            PlayerChose += answer => Description = answer ? input[1] : input[2];
            int id = 0;
            foreach (var option in options)
            {
                Vector nextPos = new Vector(100, 200 + id * 50);
                id++;
                if (id == correctAnswer)
                    correctOptions.Add(nextPos);
                this.options.Add(nextPos, new TextBox(nextPos, option, Color.White));
            }

            foreach (var option in playerRequirements)
            {
                Vector nextPos = new Vector(100, 200 + id * 50);
                id++;
                correctOptions.Add(nextPos);
                this.options.Add(nextPos, new TextBox(nextPos, option, Color.Blue));
            }
        }
    }

    class TextBox
    {
        private Rectangle bounds;
        private readonly Color color;

        public string Text
        {
            get;
            set;
        }

        public void Draw(Graphics g, Pen p)
        {
            p.Color = color;
            g.DrawString(Text, VALUES.FONT, p.Brush, bounds);
            g.DrawRectangle(p, bounds);
        }

        public TextBox(Vector where, string txt, Color c)
        {
            color = c;
            Text = txt;
            bounds = new Rectangle(where, new Size(txt.Length * 5, 50));
        }
    }
}
