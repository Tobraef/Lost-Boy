using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lost_boy.Event
{
    public class EventLevelBuilder : ILevelBuilder
    {
        private static PlayerShip player = Form1.player;
        private List<IEvent> events;
        private readonly int finishEventNumber = 0;
        private Action<bool> finisher;

        private static void EventResult_HealthChange(int value)
        {
            if (value > 0)
                player.TakeDamage(value);
            else
                player.Heal(value);
        }

        private static void EventResult_ReceiveLoot(List<IEquipable> items)
        {
            foreach (var item in items)
            {
                item.AddToInventory(player);
            }
        }

        private static void EventResult_ReceiveGold(int ammount)
        {
            player.Gold += ammount;
        }

        private static void EventResult_ChangeArmor(int value)
        {
            player.Defence += value;
        }

        private Action ParseAction(string text)
        {
            var action = text.Split('/');
            switch (action[0])
            {
                case "ChangeArmor": return () => EventResult_ChangeArmor(int.Parse(action[1]));
                case "ReceiveGold": return () => EventResult_ReceiveGold(int.Parse(action[1]));
                case "HealthChange": return () => EventResult_HealthChange(int.Parse(action[1]));
                case "ReceiveLoot": return () => EventResult_ReceiveLoot(Getters.ParseLoot(action.Skip(1)));
            }
            return null;
        }

        /*
         * #eventInfo# eventNumber Description 
         * #options# description/numberSucces(/numberFail/chanceForSucces)_description/number -> passing number -1 means this option leads to the exit
         * #actions# name/arg1/arg2/arg3 name name -> None if no actions
         */

        private List<IEvent> GenerateEventScenario(List<string> info)
        {
            List<IEvent> events = new List<IEvent>(8);
            events.Add(new FinishEvent(() => finisher(true)));
            var iter = info.GetEnumerator();
            while (iter.MoveNext())
            {
                string description = iter.Current;
                int spaceIndex = description.IndexOf(' ');
                int index = int.Parse(description.Substring(0, spaceIndex));
                if (index > events.Capacity)
                    events.Capacity = index + 1;

                iter.MoveNext();
                Dictionary<string, IEventOption> opts = new Dictionary<string, IEventOption>(
                iter.Current
                    .Split('_')
                    .Select(o => o.Split('/'))
                    .ToDictionary(
                    line => line[0],
                    line => line.Length == 2 ?
                    line[1] == "-1" ?
                    new SingleEventOption(finishEventNumber) :
                    (IEventOption)new SingleEventOption(int.Parse(line[1])) :
                    new SplitEventOption(int.Parse(line[1]), int.Parse(line[2]), int.Parse(line[3]))));

                iter.MoveNext();
                if (iter.Current.Equals("None"))
                    events.Insert(index, new NoActionEvent(description.Substring(spaceIndex), opts));
                else
                    events.Insert(index, new Event(description.Substring(spaceIndex), opts,
                        iter.Current.Split(' ').Select(act => ParseAction(act)).Aggregate((a, b) => a + b)
                        ));
            }
            return events;
        }

        public IPlayAble Build()
        {
            EventLevel level = new EventLevel(events, finishEventNumber + 1);
            return level;
        }

        public ILevelBuilder SetContent(Setup.LevelInfoHolder info)
        {
            this.events = GenerateEventScenario(info.data);
            return this;
        }

        public ILevelBuilder SetDescription(string description)
        {
            // pls
            return this;
        }

        public ILevelBuilder SetFinishedAction(Action<bool> action)
        {
            finisher = action;
            return this;
        }

        public ILevelBuilder SetPlayer(PlayerShip ship)
        {
            // so far player turned to static
            return this;
        }
    }
}