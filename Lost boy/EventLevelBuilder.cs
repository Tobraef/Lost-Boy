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

        private static void EventResult_ReceiveLoot(List<IItem> items)
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

        private static class Getters
        {
            public static IItem ParseItem(string text)
            {
                switch (text)
                {
                    case "SWeapon": return new Weapon.T1.SprayWeapon(null);
                    case "DWeapon": return new Weapon.T2.DoubleWeapon(null);
                    case "TWeapon": return new Weapon.T3.TripleWeapon(null);

                    case "Laser1": return new BulletFactory.T1.FrostyLaserFactory(Direction.Up);
                    case "Laser2": return new BulletFactory.T2.HellHotFactory(Direction.Up);
                    case "Laser3": return new BulletFactory.T2.IcyLaserFactory(Direction.Up);
                    case "Laser4": return new BulletFactory.T3.AnnihilatorFactory(Direction.Up);
                    case "Beam1": return new BulletFactory.T1.BeamFactory(Direction.Up);
                    case "Beam2": return new BulletFactory.T2.MortalCoilFactory(Direction.Up);
                    case "Beam3": return new BulletFactory.T3.DisintegratorFactory(Direction.Up);
                    case "Plasma1": return new BulletFactory.T1.PlasmaFactory(Direction.Up);
                    case "Plasma2": return new BulletFactory.T2.StarPlasmaFactory(Direction.Up);
                    case "Plasma3": return new BulletFactory.T3.DecimatorFactory(Direction.Up);
                    case "Explosive1": return new BulletFactory.T1.ExplosiveBulletFactory(Direction.Up);
                    case "Explosive2": return new BulletFactory.T2.NapalmFactory(Direction.Up);
                    case "Explosive3": return new BulletFactory.T3.ArmaggedonFactory(Direction.Up);
                }
                throw new NotImplementedException("Error parsing item name, received " + text);
            }
        }
        private List<IItem> ParseLoot(IEnumerable<string> loots)
        {
            return loots
                .Select(name => Getters.ParseItem(name))
                .ToList();
        }

        private Action ParseAction(string text)
        {
            var action = text.Split('/');
            switch (action[0])
            {
                case "ChangeArmor": return () => EventResult_ChangeArmor(int.Parse(action[1]));
                case "ReceiveGold": return () => EventResult_ReceiveGold(int.Parse(action[1]));
                case "HealthChange": return () => EventResult_HealthChange(int.Parse(action[1]));
                case "ReceiveLoot": return () => EventResult_ReceiveLoot(ParseLoot(action.Skip(1)));
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