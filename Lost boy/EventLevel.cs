using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lost_boy
{
    public class EventLevel : IPlayAble
    {
        public event Action<bool> Finished;
        private List<IEvent> states;
        private IEvent currentState;

        public void HandlePlayer(char key)
        { }

        public void HandlePlayer_KeyUp(char key)
        { }

        public void HandlePlayer_Mouse(System.Windows.Forms.MouseEventArgs m)
        {
            currentState.HandleChoice(new Vector(m.X, m.Y));
        }

        public void Begin()
        { }

        public void Elapse()
        { }

        public void Draw(System.Drawing.Graphics g, System.Drawing.Pen p)
        {
            currentState.Draw(g, p);
        }

        private void TransitionHappened(int newState)
        {
            currentState = states[newState];
            currentState.TriggerAction();
        }

        public EventLevel(List<IEvent> states, int beginState)
        {
            this.states = states;
            this.states.ForEach(s => s.TransitPopped += TransitionHappened);
            this.currentState = states[beginState];
        }
    }
}
