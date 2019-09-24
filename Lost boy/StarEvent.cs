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

        public void HandleChoice(Vector where)
        {
            IEventOption selected = null;
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

        private void Transition(EventInfo nextStage)
        {
            this.description = nextStage.description;
            this.options.Clear();
            PopulateOptions(nextStage.options);
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

        public Event(string description, Dictionary<string, IEventOption> opts)
        {
            this.description = description;
            PopulateOptions(opts);
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

        public TextBox(Vector where, string txt)
        {
            Text = txt;
            bounds = new Rectangle(where, new Size(txt.Length * 5, 50));
        }
    }

    public class SingleEventOption : IEventOption
    {
        private Func<EventInfo> eventTrigger;

        public EventInfo Trigger()
        {
            return eventTrigger();
        }

        public SingleEventOption(Func<EventInfo> trigger)
        {
            eventTrigger = trigger;
        }
    }

    public class SplitEventOption : IEventOption
    {
        private Func<EventInfo> succes;
        private Func<EventInfo> fail;
        private int chance;

        public EventInfo Trigger()
        {
            if (VALUES.random.Next(100) < chance)
                return succes();
            else
                return fail();
        }

        public SplitEventOption(Func<EventInfo> OnSucces, Func<EventInfo> OnFail, int chanceSucces)
        {
            chance = chanceSucces;
            succes = OnSucces;
            fail = OnFail;
        }
    }

    namespace EventOptions
    {
        public class AffectPlayerOption : SplitEventOption
        {
            public AffectPlayerOption(EventInfo nextSuces, EventInfo nextFail,
                int chanceSucces,
                Action<PlayerShip> succes, Action<PlayerShip> fail) :
                base(
                    () =>
                    {
                        succes(Form1.player);
                        return nextSuces;
                    },
                    () =>
                    {
                        fail(Form1.player);
                        return nextFail;
                    }, chanceSucces)
            { }
        }

        public class FinishOption : SingleEventOption
        {
            public FinishOption(Action finisher) :
                base(() => { finisher(); return null; })
            { }
        }
    }

    public class EventInfo
    {
        public string description;
        public Dictionary<string, IEventOption> options;
    }
}