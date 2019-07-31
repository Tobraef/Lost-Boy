using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Lost_boy
{
    delegate Action TresholdPass();
    delegate Action OnDeath();

    public enum Direction : int
    {
        Up = -1,
        Down = 1
    }
    public static class VALUES
    {
        public const int HEIGHT = 600;
        public const int WIDTH = 1000;
        public const int TICK_INTERVAL = 3000; //milis
        public const int BASIC_WEAPON_RELOAD_TIME = 500;
        public const int BASIC_LASER_DMG = 10;
        public const int BASIC_LASER_SPEED = 10;
        public const int BASIC_LASER_BURN_DMG = 5;
        public const int BASIC_LASER_BURN_CHANCE = 20;
    }

    interface IMover
    {
        Vector Position
        {
            get;
            set;
        }
        Vector Speed
        {
            get;
            set;
        }
        Vector Acceleration
        {
            get;
            set;
        }
        Vector Size
        {
            get;
            set;
        }
        void Move();
        void Draw(Graphics g, Pen p);
    }

    interface IEffect
    {
        public static implicit operator Action<IShip>(IEffect e);
        Action<IShip> WrappedAction
        {
            get;
        }
    }

    interface IProjectile : IMover
    {
        IProjectile Clone();
        void AppendEffect(Action<IShip> e);
        TresholdPass TresholdPass
        {
            set;
        }
        void AffectShip(IShip ship);
    }

    interface IWeapon
    {
        IProjectile GetProjectile();
        bool IsLoaded
        {
            get;
        }
        void AppendOnHit(Action<IShip> e);
    }

    interface IShip : IMover
    {
        IWeapon Weapon
        {
            get;
            set;
        }
        int Health
        {
            get;
            set;
        }
        int Defence
        {
            get;
            set;
        }
        void Shoot(Action<IProjectile> bulletAdder);
        void TakeDamage(int val);
        bool IsHit(IProjectile projectile);
    }
}
