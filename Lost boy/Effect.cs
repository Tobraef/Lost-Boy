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
            Ticks = ticks;
            WrappedAction = ship =>
            {
                Thread th = new Thread(() =>
                {
                    int times = Ticks;
                    while (times > 0)
                    {
                        action(ship);
                        times--;
                        Thread.Sleep(VALUES.TICK_INTERVAL);
                    }
                });
                th.Start();
            };
        }
    }

    public class Effect : IEffect
    {
        public Action<IShip> WrappedAction
        {
            get;
            private set;
        }

        public Action<IShip> Get()
        {
            return this.WrappedAction;
        }

        public Effect(Action<IShip> action)
        {
            WrappedAction = action;
        }

        public static implicit operator Action<IShip>(Effect e)
        {
            return e.WrappedAction;
        }
    }

    public class TemporaryEffect : IEffect
    {
        public Action<IShip> WrappedAction
        {
            get;
            private set;
        }

        public Action<IShip> Get()
        {
            return WrappedAction;
        }

        public static implicit operator Action<IShip>(TemporaryEffect e)
        {
            return e.WrappedAction;
        }

        public TemporaryEffect(Action<IShip> sub, Action<IShip> unSub, int milis)
        {
            WrappedAction = ship =>
            {
                Thread th = new Thread(() =>
                {
                    sub(ship);
                    Thread.Sleep(milis);
                    unSub(ship);
                });
                th.Start();
            };
        }
    }

    public class ShieldEffect : TemporaryEffect
    {
        public ShieldEffect()
            : base(
            ship =>
            {
                ship.Defence += 1000;
            },
            ship =>
            {
                ship.Defence -= 1000;
            },
            10000
            )
        { }
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
                    projectile.Speed = new Vector(projectile.Speed.X,
                        projectile.Speed.Y + (int)projectile.Direction*val);
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

    namespace OnHits
    {
        public class BurnChance : Effect
        {
            public BurnChance(int value, int ticks, int chance)
                : base(
                ship =>
                {
                    if (chance >= VALUES.random.Next(100))
                    {
                        new OverTimeEffect(s =>
                        {
                            if (s.Health > 0)
                                s.TakeTrueDamage(value);
                        }, ticks).WrappedAction(ship);
                    }
                })
            { }
        }

        public class GoldDig : Effect
        {
            public GoldDig(Action<IProjectile> adder, int value, int chance)
                : base(
            ship =>
            {
                if (chance > VALUES.random.Next(100))
                    ship.onDeath += () =>
                    {
                        adder(new GoldCoin(ship.Position, value));
                    };
            })
            { }
        }

        public class SlowEffect : TemporaryEffect
        {
            public SlowEffect(int value)
                : base(
            ship =>
                {
                    ship.MaxSpeed -= value;
                },
            ship =>
            {
                ship.MaxSpeed += value;
            },
                5000)
            { }
        }
    }
}