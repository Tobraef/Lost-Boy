using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace Lost_boy
{
    using Enemies;
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
            EnemyShip enemy = (EnemyShip)ship;
            if (enemy.Position.Y > VALUES.HEIGHT)
                enemy.Teleport(enemy.Position.X, -enemy.Size.Y);

            if (enemy.Position.X < -enemy.Size.X - 10)
                enemy.Teleport(VALUES.WIDTH, enemy.Position.Y);
            else if (enemy.Position.X > VALUES.WIDTH + enemy.Size.X + 10)
                enemy.Teleport(0, enemy.Position.Y);

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
        private IEnumerator<KeyValuePair<Vector, int>> currentStep;
        private int delay;
        private Action finishCallback;

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
                finishCallback();
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

        public LevelInitialStrategy(IEnumerator<KeyValuePair<Vector, int>> iter, int delay,
            Action finishCallback)
        {
            this.finishCallback = finishCallback;
            currentStep = iter;
            this.delay = delay;
        }
    }

    public class GoToStrategy : IMovementStrategy
    {
        private Vector destination;
        private int moveTicks;
        private int flatSpeed;
        private Action<IShip> movefunction;

        private void ApplyAndGo(IShip s)
        {
            Vector speed = destination - s.Position;
            moveTicks = (int)speed.Length / flatSpeed;
            speed /= moveTicks;
            s.Speed = speed;
            moveTicks--;
            movefunction = GoToPoint;
        }

        private void GoToPoint(IShip s)
        {
            moveTicks--;
            if (moveTicks < 0)
            {
                ((EnemyShip)s).Teleport(destination.X, destination.Y);
                ((EnemyShip)s).MovementStrategy = null;
            }
        }

        public void ApplyStrategy(IShip m)
        {
            movefunction(m);
        }

        public void StopStrategy(IShip m)
        {
            moveTicks = 0;
        }

        public GoToStrategy(Vector to, int withSpeed)
        {
            flatSpeed = withSpeed;
            destination = to;
            movefunction = ApplyAndGo;
        }
    }

    public class DanceInRectangleStrategy : IMovementStrategy
    {
        private Vector leftBound;
        private Vector rightBound;
        private int ticks;
        private readonly int speed = 10;

        public void ApplyStrategy(IShip m)
        {
            if (ticks < 0)
            {
                Vector destiny = new Vector(VALUES.random.Next(leftBound.X, rightBound.X),
                                    VALUES.random.Next(leftBound.Y, rightBound.Y));
                double distance = m.Position.DistanceFrom(destiny);
                ticks = (int)distance / speed;
                m.Speed = destiny - m.Position;
                m.Speed /= ticks;
            }
            else
                ticks--;
        }

        public void StopStrategy(IShip m)
        {
            m.Speed = new Vector();
            ticks = 0;
        }

        public DanceInRectangleStrategy(Vector first, Vector second)
        {
            leftBound = first;
            rightBound = second;
        }

        public DanceInRectangleStrategy()
        {
            leftBound = new Vector(200, 200);
            rightBound = new Vector(VALUES.WIDTH - 200, 400);
        }
    }
}