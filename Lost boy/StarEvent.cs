using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace Lost_boy.Event
{
    public class Event : IEvent
    {
        private string description;
        private Dictionary<TextBox, IEventOption> options =
            new Dictionary<TextBox, IEventOption>();
        private Action popCall;

        public event Action<int> TransitPopped;

        public void HandleChoice(Vector where)
        {
            IEventOption selected = null;
            foreach (var pair in options)
            {
                var frame = pair.Key;
                if (frame.IsPressed(where))
                {
                    selected = pair.Value;
                    break;
                }
            }
            if (selected != null)
            {
                int nextEvent = selected.Trigger();
                TransitPopped(nextEvent);
            }
        }

        private void PopulateOptions(Dictionary<string, IEventOption> options)
        {
            int i = 0;
            foreach (var option in options)
            {
                Vector w = new Vector(50, 200 + 50 * i);
                this.options.Add(new TextBox(w, option.Key), option.Value);
                ++i;
            }
        }

        public void TriggerAction()
        {
            popCall();
        }

        public void Draw(Graphics g, Pen p)
        {
            p.Color = Color.White;
            g.DrawString(description, VALUES.FONT, p.Brush, 50, 50);
            foreach (var box in options.Keys)
            {
                box.Draw(g, p);
            }
        }

        public Event(string description, Dictionary<string, IEventOption> opts, Action trigger)
        {
            this.popCall = trigger;
            this.description = description;
            PopulateOptions(opts);
        }
    }

    public class NoActionEvent : IEvent
    {
        private string description;
        private Dictionary<TextBox, IEventOption> options =
            new Dictionary<TextBox, IEventOption>();

        public event Action<int> TransitPopped;

        public void HandleChoice(Vector where)
        {
            IEventOption selected = null;
            foreach (var pair in options)
            {
                var frame = pair.Key;
                if (frame.IsPressed(where))
                {
                    selected = pair.Value;
                    break;
                }
            }
            if (selected != null)
            {
                int nextEvent = selected.Trigger();
                TransitPopped(nextEvent);
            }
        }

        private void PopulateOptions(Dictionary<string, IEventOption> options)
        {
            int i = 0;
            foreach (var option in options)
            {
                Vector w = new Vector(50, 200 + 50 * i);
                this.options.Add(new TextBox(w, option.Key), option.Value);
                ++i;
            }
        }

        public void TriggerAction()
        {}

        public void Draw(Graphics g, Pen p)
        {
            p.Color = Color.White;
            g.DrawString(description, VALUES.FONT, p.Brush, 50, 50);
            foreach (var box in options.Keys)
            {
                box.Draw(g, p);
            }
        }

        public NoActionEvent(string description, Dictionary<string, IEventOption> opts)
        {
            this.description = description;
            PopulateOptions(opts);
        }
    }

    public class FinishEvent : IEvent
    {
        public event Action<int> TransitPopped;
        private Action finish;

        public void Draw(Graphics g, Pen p)
        {}

        public void HandleChoice(Vector where)
        {}

        public void TriggerAction()
        {
            finish();
        }

        public FinishEvent(Action finisher)
        {
            finish = finisher;
        }
    }

    public class TextBox
    {
        private Rectangle bounds;

        public string Text
        {
            get;
            set;
        }

        public Point Position
        {
            get { return bounds.Location; }
        }

        public Size Size
        {
            get { return bounds.Size; }
        }

        public void Draw(Graphics g, Pen p)
        {
            g.DrawString(Text, VALUES.FONT, p.Brush, bounds);
            g.DrawRectangle(p, bounds);
        }

        public bool IsPressed(Vector where)
        {
            return Position.X < where.X && Position.X + Size.Width > where.X &&
                   Position.Y < where.Y && Position.Y + Size.Height > where.Y;
        }

        public TextBox(Vector where, string txt)
        {
            Text = txt;
            bounds = new Rectangle(where, new Size(txt.Length * 12, 30));
        }
    }

    public class SingleEventOption : IEventOption
    {
        private int next;

        public int Trigger()
        {
            return next;
        }

        public SingleEventOption(int followUp)
        {
            next = followUp;
        }
    }

    public class SplitEventOption : IEventOption
    {
        private int succes;
        private int fail;
        private int chance;

        public int Trigger()
        {
            if (VALUES.random.Next(100) < chance)
                return succes;
            else
                return fail;
        }

        public SplitEventOption(int succesTransit, int failTransit, int chanceSucces)
        {
            chance = chanceSucces;
            this.succes = succesTransit;
            this.fail = failTransit;
        }
    }
}