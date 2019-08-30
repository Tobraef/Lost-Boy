using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace Lost_boy
{
    public class FallDownStrategy : IMovementStrategy
    {
        private int circulationPointX;
        private const int circulatingAccelarationX = 1;
        Timer timer;
        public FallDownStrategy()
        {
            timer = new Timer(5000);
            timer.Elapsed += (o, args) =>
            {
                circulationPointX = VALUES.random.Next(VALUES.WIDTH);
            };
            timer.Start();
        }

        public void ApplyStrategy(IShip ship)
        {
            if (ship.Position.X > circulationPointX)
                ship.Acceleration = new Vector(circulatingAccelarationX, 0);
            else
                ship.Acceleration = new Vector(-circulatingAccelarationX, 0);
            if (Math.Abs(ship.Speed.X) < ship.MaxSpeed)
                ship.Speed = new Vector(ship.Speed.X, VALUES.ENEMY_FALLING_SPEED);
            else
            {
                if (ship.Speed.X > 0)
                    ship.Speed = new Vector(ship.MaxSpeed, VALUES.ENEMY_FALLING_SPEED);
                else
                    ship.Speed = new Vector(-ship.MaxSpeed, VALUES.ENEMY_FALLING_SPEED);
            }
        }

        public void StopStrategy(IShip ship)
        {
            timer.Enabled = false;
            ship.Speed = new Vector();
        }
    }

    public class NormalMovementStrategy : IMovementStrategy
    {
        private Func<IShip, bool> currentMovement;
        private void SwitchStrategy(IShip ship)
        {
            if (currentMovement == MoveLeft)
                currentMovement = MoveRight;
            else
                currentMovement = MoveLeft;
        }

        private bool MoveLeft(IShip ship)
        {
            if (ship.Speed.X != -ship.MaxSpeed)
                ship.Speed = new Vector(-ship.MaxSpeed, 0);
            return ship.Position.X <= 0;
        }

        private bool MoveRight(IShip ship)
        {
            if (ship.Speed.X != ship.MaxSpeed)
                ship.Speed = new Vector(ship.MaxSpeed, 0);
            return ship.Position.X + ship.Size.X >= VALUES.WIDTH;
        }

        public void ApplyStrategy(IShip ship)
        {
            if (currentMovement(ship))
                SwitchStrategy(ship);
        }

        public void StopStrategy(IShip ship)
        {
            ship.Speed = new Vector();
        }

        public NormalMovementStrategy()
        {
            this.currentMovement = MoveLeft;
        }
    }

    public class FindAndShootStrategy : IMovementStrategy
    {
        private Mover watchedShip;

        public FindAndShootStrategy(Mover m)
        {
            this.watchedShip = m;
        }

        public void ApplyStrategy(IShip ship)
        {
            if (ship.Position.X > watchedShip.Position.X)
                ship.Acceleration = new Vector(-1, 0);
            else
                ship.Acceleration = new Vector(1, 0);

            if (Math.Abs(ship.Speed.X) < ship.MaxSpeed)
                return;
            else
            {
                if (ship.Speed.X > 0)
                    ship.Speed = new Vector(ship.MaxSpeed, 0);
                else
                    ship.Speed = new Vector(-ship.MaxSpeed, 0);
            }
        }

        public void StopStrategy(IShip ship)
        {
            ship.Speed = new Vector();
        }
    }

    public class LevelInitialStrategy : IMovementStrategy
    {
        private int currentCount;
        IEnumerator<KeyValuePair<Vector, int>> currentStep;
        int delay;
        public void ApplyStrategy(IShip m)
        {
            if (delay > 0)
            {
                delay--;
                return;
            }
            if (currentCount > 0)
            {
                --currentCount;
                return;
            }
            if (!currentStep.MoveNext())
            {
                ((EnemyShip)m).SetDefaultMoveStrategy();
            }
            else
            {
                currentCount = currentStep.Current.Value;
                m.Speed = currentStep.Current.Key;
            }
        }

        public void StopStrategy(IShip m)
        {
            currentStep = null;
        }

        public LevelInitialStrategy(IEnumerator<KeyValuePair<Vector, int>> iter, int delay)
        {
            currentStep = iter;
            this.delay = delay;
        }
    }
}