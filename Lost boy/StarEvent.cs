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

        public Action NextStage
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
                if (frame.Position.X > where.X && frame.Position.X + frame.Size.Width < where.X &&
                    frame.Position.Y > where.Y && frame.Position.Y + frame.Size.Height < where.Y)
                {
                    selected = pair.Value;
                    break;
                }
            }
            if (selected != null)
            {
                Transition(selected.Trigger());
            }
        }

        private void Transition(EventInfo nextStage)
        {
            this.description = nextStage.description;
            this.options = nextStage.options;
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



        public Event(string description, Dictionary<TextBox, EventOption> opts)
        {
            this.description = description;
            foreach (var option in opts)
            {
                this.options.Add(option.Key, option.Value);
            }
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

        public TextBox(Vector where, string txt, Color c)
        {
            Text = txt;
            bounds = new Rectangle(where, new Size(txt.Length * 5, 50));
        }
    }

    interface IEventOption
    {
        EventInfo Trigger();
    }

    public class EventOption : IEventOption
    {
        private Func<EventInfo> eventTrigger;

        public EventInfo Trigger()
        {
            return eventTrigger();
        }

        public EventOption(Func<EventInfo> trigger)
        {
            eventTrigger = trigger;
        }
    }

    public class SplitEventOption : IEventOption
    {
        private Func<EventInfo> succes;
        private Func<EventInfo> fail;

        public EventInfo Trigger()
        {
            if (VALUES.random.Next(100) > 50)
                return succes();
            else
                return fail();
        }

        public SplitEventOption(Func<EventInfo> OnSucces, Func<EventInfo> OnFail)
        {
            succes = OnSucces;
            fail = OnFail;
        }
    }

    public class EventInfo
    {
        public string description;
        public Dictionary<TextBox, EventOption> options;
    }
}