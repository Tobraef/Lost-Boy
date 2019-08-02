using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Lost_boy
{
    public class NormalMovementStrategy : IMovementStrategy
    {
        private Thread thread;
        private void Strategy(IShip ship)
        {
            Func<IShip, bool> currentMovement = MoveLeft;
            
        }

        private bool MoveLeft(IShip ship)
        {
            ship.Speed = new Vector(-5, 0);
            return ship.Position.X <= 10;
        }

        private bool MoveRight(IShip ship)
        {
            ship.Speed = new Vector(5, 0);
            return ship.Position.X + ship.Size.X >= VALUES.WIDTH;
        }
        public void ApplyStrategy(IShip ship)
        {

        }

        public void StopStrategy(IShip ship)
        {
            thread.Abort();
        }
    }
}
