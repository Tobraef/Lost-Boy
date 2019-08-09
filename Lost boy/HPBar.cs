using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Lost_boy
{
    public class HPBar
    {
        private Rectangle redRectangle;
        private Rectangle greenRectangle;
        private int maxHp;
        private int barLength;

        public void HpChanged(int currentHp)
        {
            float ratio = (float)currentHp / (float)maxHp;
            int greenLength = (int)((float)barLength * ratio);
            int change = greenRectangle.Width - greenLength;
            greenRectangle.Width -= change;
            redRectangle.Width += change;
            redRectangle.X -= change;
        }

        public void Draw(Graphics g, Pen p)
        {
            p.Color = Color.Green;
            g.DrawRectangle(p, greenRectangle);
            p.Color = Color.Red;
            g.DrawRectangle(p, redRectangle);
        }

        public void UpdatePosition(int dx)
        {
            greenRectangle.X += dx;
            redRectangle.X += dx;
        }

        public HPBar(IShip ship)
        {
            this.barLength = ship.Size.X;
            this.maxHp = ship.Health;
            this.greenRectangle = new Rectangle(ship.Position.X,
                ship.Position.Y - VALUES.HP_BAR_HEIGHT,
                barLength,
                VALUES.HP_BAR_HEIGHT);
            this.redRectangle = new Rectangle(ship.Position.X + barLength,
                ship.Position.Y - VALUES.HP_BAR_HEIGHT,
                0,
                VALUES.HP_BAR_HEIGHT);
        }
    }
}
