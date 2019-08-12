﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Lost_boy
{
    public abstract class Laser : Mover, IBullet
    {
        public event Action<IShip> onHits;
        public event Modify dmgModifiers;
        protected Direction direction;

        public int Damage
        {
            get;
            set;
        }

        public Action TresholdPass
        {
            private get;
            set;
        }

        public void AffectShip(IShip ship)
        {
            if (onHits != null)
                onHits(ship);
            int dmg = Damage;
            if (dmgModifiers != null)
                dmgModifiers(ref dmg);
            ship.TakeDamage(dmg);
        }

        public override void Move()
        {
            base.Move();
            if (Position.Y > VALUES.HEIGHT || Position.Y < 0)
                TresholdPass();
        }

        public abstract IBullet Clone();

        public Laser(Vector position, Vector size, Direction dir, int speed, int damage) :
            base(position, new Vector(0, (int)dir * speed), new Vector(0, 0), size)
        {
            this.Damage = damage;
        }
    }
}