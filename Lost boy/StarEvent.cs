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
        private Dictionary<TextBox, EventOption> options =
            new Dictionary<TextBox, EventOption>();

        public Action<IEvent> NextStage
        {
            get;
            set;
        }

        public void HandleChoice(Vector where)
        {
            EventOption selected = null;
            foreach (var pair in options)
            {
                var frame = pair.Key;
                if (frame.IsPressed(where))
                {
                    selected = pair.Value;
                    break;
                }
            }
            NextStage(selected.Trigger());
        }

        public void Draw(Graphics g, Pen p)
        {
            p.Color = Color.White;
            foreach (var box in options.Keys)
            {
                box.Draw(g,p);
            }
        }

        public Event(string description, Dictionary<TextBox, EventOption> options, PlayerShip player)
        {
            this.description = description;
            this.options = options;
        }
    }

    public class FinishEvent : IEvent
    {
        private string description;

        public Action<IEvent> NextStage
        {
            get;
            set;
        }

        public void HandleChoice(Vector where)
        {
            NextStage(null);
        }

        public void Draw(Graphics g, Pen p)
        {
            p.Color = Color.White;
            g.DrawString(description, VALUES.FONT, p.Brush, 50, 50);
        }

        public FinishEvent(string description)
        {
            this.description = description + "\nClick anywhere to continue";
        }
    }

    interface IEventOption
    {
        IEvent Trigger();
    }

    public class EventOption : IEventOption
    {
        private Func<IEvent> eventTrigger;

        public IEvent Trigger()
        {
            return eventTrigger();
        }

        public EventOption(Func<IEvent> trigger)
        {
            eventTrigger = trigger;
        }
    }

    public class SplitEventOption : IEventOption
    {
        private Func<IEvent> succes;
        private Func<IEvent> fail;

        public IEvent Trigger()
        {
            if (VALUES.random.Next(100) > 50)
                return succes();
            else
                return fail();
        }

        public SplitEventOption(Func<IEvent> OnSucces, Func<IEvent> OnFail)
        {
            succes = OnSucces;
            fail = OnFail;
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

}

