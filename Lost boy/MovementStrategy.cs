using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Lost_boy
{
    public class NormalMovementStrategy : IMovementStrategy
    {
        private const int speed = 15;
        Func<Mover, bool> currentMovement;
        private void Strategy(Mover ship)
        {
            if (!currentMovement(ship))
                return;
            if (currentMovement == MoveLeft)
                currentMovement = MoveRight;
            else
                currentMovement = MoveLeft;
        }

        private bool MoveLeft(Mover ship)
        {
            if (ship.Speed.X != -speed)
                ship.Speed = new Vector(-speed, 0);
            return ship.Position.X <= speed;
        }

        private bool MoveRight(Mover ship)
        {
            if (ship.Speed.Y != speed)
                ship.Speed = new Vector(speed, 0);
            return ship.Position.X + ship.Size.X >= VALUES.WIDTH;
        }

        public void ApplyStrategy(Mover ship)
        {
            Strategy(ship);
        }

        public void StopStrategy(Mover ship)
        {
            ship.Speed = new Vector();
        }

        public NormalMovementStrategy()
        {
            this.currentMovement = MoveLeft;
        }
    }
}
