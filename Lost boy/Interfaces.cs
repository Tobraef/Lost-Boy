using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Lost_boy
{
    public delegate void Modify(ref int i);
    public enum Direction : int
    {
        Up = -1,
        Down = 1
    }
    public static class VALUES
    {
        public const int HEIGHT = 600;
        public const int WIDTH = 1000;

        public const int PLAYER_HEIGHT = 50;
        public const int PLAYER_WIDTH = 50;
        public const int PLAYER_HEALTH = 1000;

        public const int ENEMY_HEIGHT = 30;
        public const int ENEMY_WIDTH = 30;
        public const int ENEMY_HEALTH = 300;

        public const int TICK_INTERVAL = 3000; //milis

        public const int BASIC_WEAPON_RELOAD_TIME = 500;
        public const int BASIC_LASER_DMG = 10;
        public const int BASIC_LASER_SPEED = 10;
        public const int BASIC_LASER_BURN_DMG = 5;
        public const int BASIC_LASER_BURN_CHANCE = 20;
    }

    public interface IMover
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

    public interface IEffect
    {
        Action<IShip> WrappedAction
        {
            get;
        }
    }

    public interface IProjectile : IMover
    {
        IProjectile Clone();
        void AppendEffect(Action<IShip> e);
        Action TresholdPass
        {
            set;
        }
        void AffectShip(IShip ship);
    }

    public interface IWeapon
    {
        IProjectile GetProjectile(Vector launchPos);
        void SetAmmo(IProjectile p);
        bool IsLoaded
        {
            get;
        }
        void AppendOnShot(Action<IProjectile> e);
    }

    public interface IShip : IMover
    {
        IWeapon Weapon
        {
            get;
            set;
        }
        IProjectile Ammo
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