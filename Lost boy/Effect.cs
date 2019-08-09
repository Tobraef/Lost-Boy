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
            private set;
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
                th.Start();
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

namespace Lost_boy
{
    namespace OnShots
    {
        public class SpeedChange
        {
            private OnShot heldFunction;
            public static implicit operator OnShot(SpeedChange s)
            {
                return s.heldFunction;
            }

            public OnShot Get()
            {
                return heldFunction;
            }

            public SpeedChange(int val)
            {
                heldFunction = projectile =>
                {
                    projectile.Speed = new Vector(projectile.Speed.X, projectile.Speed.Y + val);
                };
            }

            public SpeedChange(float ratio)
            {
                heldFunction = projectile =>
                {
                    projectile.Speed = new Vector((int)((float)projectile.Speed.X * ratio), (int)((float)projectile.Speed.Y * ratio));
                };
            }
        }

        public class SizeChange
        {
            private OnShot heldFunction;
            public static implicit operator OnShot(SizeChange s)
            {
                return s.heldFunction;
            }

            public OnShot Get()
            {
                return heldFunction;
            }

            public SizeChange(int val)
            {
                heldFunction = projectile =>
                {
                    projectile.Size = new Vector(projectile.Size.X + val, projectile.Size.Y + val);
                };
            }

            public SizeChange(float ratio)
            {
                heldFunction = projectile =>
                {
                    projectile.Size = new Vector((int)((float)projectile.Size.X * ratio), (int)((float)projectile.Size.Y * ratio));
                };
            }
        }
    }
}