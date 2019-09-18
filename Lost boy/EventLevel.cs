using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lost_boy
{
    public class EventLevel : IPlayAble
    {
        private IEvent levelState;

        public event Action<bool> Finished;

        public void HandlePlayer(char key)
        {}

        public void HandlePlayer_KeyUp(char key)
        {}

        public void HandlePlayer_Mouse(System.Windows.Forms.MouseEventArgs m)
        {
            levelState.HandleChoice(new Vector(m.X, m.Y));
        }

        public void Begin()
        {}

        public void Elapse()
        {}

        private void NewEventStage(IEvent newEvent)
        {
            if (newEvent == null)
                Finished(true);
            levelState = newEvent;
            newEvent.NextStage = NewEventStage;
        }

        public void Draw(System.Drawing.Graphics g, System.Drawing.Pen p)
        {
            levelState.Draw(g, p);
        }

        public EventLevel(IEvent starter)
        {
            NewEventStage(starter);
            Finished += b => levelState = null;
        }
    }
}
