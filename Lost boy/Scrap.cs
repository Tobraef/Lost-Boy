using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Lost_boy
{
    public enum ScrapType
    {
        Steel,
        Carbon,
        Uranium,
        Plutonium
    }

    public class Scrap : Mover, IItem, IComparable, IBonus
    {
        public event Action<IShip> onHits;
        public event Action onDeath;
        public event Action<IProjectile> OnRecycle;

        private Rectangle drawable;

        public ScrapType Type
        {
            get;
            private set;
        }

        public int Price
        {
            get
            {
                switch (Type)
                {
                    case ScrapType.Carbon:
                        return 5;
                    case ScrapType.Steel:
                        return 10;
                    case ScrapType.Uranium:
                        return 20;
                    case ScrapType.Plutonium:
                        return 40;
                }
                return 100;
            }
        }

        public void AddToInventory(IHolder player)
        {
            //temporary hack
            var pair = player.Scraps
                .First(s => ((Scrap)s.Key).Type == this.Type);
            int newCount = pair.Value + 1;
            player.Scraps.Remove(pair.Key);
            player.Scraps.Add(pair.Key, newCount);
        }

        public Scrap(ScrapType type) :
            this(new Vector(), type)
        {}

        public Scrap(Vector position, ScrapType type) :
            base(
            position,
            new Vector(0, VALUES.BONUS_SPEED),
            new Vector(),
            new Vector(VALUES.BONUS_SIZE, VALUES.BONUS_SIZE))
        {
            this.drawable = new Rectangle(position.X, position.Y, Size.X, Size.Y);
            this.Type = type;
            this.onHits += s => AddToInventory((IHolder)s);
        }

        public void SellFrom(IHolder holder)
        {
            holder.Scraps[this]--;
            holder.Gold += this.Price;
        }

        public override string ToString()
        {
            return Type.ToString();
        }

        public int CompareTo(object obj)
        {
            Scrap other = obj as Scrap;
            if ((int)other.Type < (int)this.Type)
                return -1;
            else if ((int)other.Type == (int)this.Type)
                return 0;
            else
                return 1;
        }

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
            drawable.X = Position.X;
            drawable.Y = Position.Y;
        }

        public override void Draw(System.Drawing.Graphics g, System.Drawing.Pen p)
        {
            p.Color = Color.Gray;
            g.DrawRectangle(p, drawable);
        }

        public void Recycle()
        {
            OnRecycle(this);
        }

        public IBonus Clone(Vector newOne)
        {
            return new Scrap(newOne, this.Type);
        }
    }
}
