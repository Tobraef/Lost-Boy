using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lost_boy.Event
{
    /*
     * EventName
     * description
     * option(dispatch,onepath) ; optionName_nextEvent_Action ; optionName_nextEvent_Action
     * 
     */
    using EventOptions;

    public class EventScenario
    {
        
        public EventScenario(Action finisher)
        {
            IEventOption takeRisk = new SplitEventOption(() => {
                Form1.player.Backpack.Add(new BulletFactory.T1.BeamFactory(Direction.Up));
                return new EventInfo { description = "You grab loot and escape asap", options = new Dictionary<string, IEventOption> { { "Go", new FinishOption(finisher) } } };
            },
            () => {
                Form1.player.TakeTrueDamage(40);
                return new EventInfo { description = "You travel for hours, getting hit and find nothing",
                    options = new Dictionary<string, IEventOption> { { "Go", new FinishOption(finisher) } }
                };
            },
            50);

        }

    }
}
