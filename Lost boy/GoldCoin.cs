using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Lost_boy
{
    public class GoldCoin : Mover, IProjectile
    {
        private Color color = Color.Gold;
        private Rectangle drawable;
        public event Action<IShip> onHits;
        public event Action onDeath;

        public Direction Direction
        {
            get { return Direction.Down; }
        }

        public void AffectShip(IShip ship)
        {
            onHits(ship);
            onDeath();
        }

        public override void Move()
        {
            base.Move();
            drawable.X = this.Position.X;
            drawable.Y = this.Position.Y;
        }

        public override void Draw(Graphics g, Pen p)
        {
            p.Color = color;
            g.DrawEllipse(p, drawable);
        }

        public GoldCoin(Vector position, int ammount)
            : base(position,
                   new Vector(0, VALUES.BONUS_SPEED),
                   new Vector(),
                   new Vector(VALUES.BONUS_SIZE / 2, VALUES.BONUS_SIZE / 2))
        {
            this.onHits += ship => ship.Gold += ammount;
            this.drawable = new Rectangle(position.X, position.Y, Size.X, Size.Y);
        }
    }
}