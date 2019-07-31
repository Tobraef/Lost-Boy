using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Lost_boy
{
    public class OverTimeEffect : IEffect
    {
        public Action<IShip> WrappedAction
        {
            get;
        }

        public static implicit operator Action<IShip>(OverTimeEffect e)
        {
            return e.WrappedAction;
        }

        public int Ticks
        {
            get;
            private set;
        }

        public static Action<IShip> Create(Action<IShip> action, int ticks)
        {
            return ship =>
            {
                Thread th = new Thread(() =>
                {
                    while (ticks > 0)
                    {
                        action(ship);
                        ticks--;
                        Thread.Sleep(VALUES.TICK_INTERVAL);
                    }
                });
                th.Join();
            };
        }

        public OverTimeEffect(Action<IShip> action, int ticks)
        {
            WrappedAction = ship =>
            {
                Thread th = new Thread(() =>
                {
                    while (ticks > 0)
                    {
                        action(ship);
                        ticks--;
                        Thread.Sleep(VALUES.TICK_INTERVAL);
                    }
                });
                th.Join();
            };
            Ticks = ticks;
        }
    }

    public class DamageEffect : IEffect
    {
        public Action<IShip> WrappedAction
        {
            get;
            private set;
        }

        public static Action<IShip> Create(int value)
        {
            return ship =>
            {
                ship.TakeDamage(value);
            };
        }

        public DamageEffect(int value)
        {
            WrappedAction = ship =>
            {
                ship.TakeDamage(value);
            };
        }

        public static implicit operator Action<IShip>(DamageEffect e)
        {
            return e.WrappedAction;
        }
    }


}