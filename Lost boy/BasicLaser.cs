using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Lost_boy
{
    public class BasicLaser : Laser
    {
        private Direction direction;
        private Rectangle drawable;
        private Color color = Color.Red;

        public void Draw(Graphics g, Pen p)
        {
            p.Color = color;
            g.DrawRectangle(p, drawable);
        }

        public BasicLaser Clone()
        {
            return new BasicLaser(this.Position, this.direction);
        }

        public BasicLaser(Vector pos, Direction dir) : 
            base(pos, dir, VALUES.BASIC_LASER_SPEED, VALUES.BASIC_LASER_DMG)
        {
            this.direction = dir;
            var function = OverTimeEffect.Create(DamageEffect.Create(VALUES.BASIC_LASER_BURN_DMG), 3);
            base.Size = new Vector(5, 10);
            base.shipEffects += ship =>
                {
                    if (VALUES.BASIC_LASER_BURN_CHANCE >= new Random(5).Next(100))
                    {
                        function(ship);
                    }
                };
            drawable = new Rectangle(Position.X, Position.Y, Size.X, Size.Y);
        }
    }
}
