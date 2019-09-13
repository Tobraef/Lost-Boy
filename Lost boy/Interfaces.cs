using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

/*TODOS
 * NEW FEATURES
 * implement shop
 * implement star map
 * implement boss
 * implement enemy that appears randomly, drops lots of stuff, disappears after specified time
 * implement graphics
 * implement new level types
 * implement new ammo types
 * implement events
 * 
 * REFACTOR
 * falling strategy -> infinite timer
 * writing to file -> increment last lvl id, not start over from 1 (done, needs testing)
 * recycling meteors
 * 
 * MAYBE
 * lvls with tiers
 * clear interfaces, not to expose unnecessary stuff
 * implement secondary weapon
 * synchronous draw with logic
*/

namespace Lost_boy
{
    public delegate void Modify(ref int i);
    namespace OnShots
    {
        public delegate void OnShot(IBullet p);
    }

    namespace OnHits
    {
        public delegate void OnHit(IShip ship);
    }

    namespace Enemies
    {
        public enum EnemyTypes
        {
            Casual,
            Frosty,
            Rocky,
            Tricky
        }
    }

    public enum Direction : int
    {
        Up = -1,
        Down = 1
    }

    public enum Difficulty : int
    {
        None,
        Easy,
        Normal,
        Hard
    }

    public enum Tier
    {
        T1,
        T2,
        T3
    }

    public enum LevelType
    {
        Classic,
        Meteor,
        Event
    }

    public static class VALUES
    {
        public static Random random = new Random(5);

        public const int HEIGHT = 600;
        public const int WIDTH = 1000;

        public const int HP_BAR_WIDTH = 100;
        public const int HP_BAR_HEIGHT = 10;

        public const int PLAYER_HEIGHT = 50;
        public const int PLAYER_WIDTH = 50;
        public const int PLAYER_HEALTH = 200;
        public const int PLAYER_SPEED = 15;

        public const int ENEMY_HEIGHT = 30;
        public const int ENEMY_WIDTH = 30;
        public const int ENEMY_HEALTH = 10;
        public const int ENEMY_FALLING_SPEED = 5;

        public const int TICK_INTERVAL = 3000; //milis

        public const int BASIC_WEAPON_RELOAD_TIME = 300;

        public const int BASIC_LASER_RECHARGE = 350;
        public const int BASIC_LASER_DMG = 10;
        public const int BASIC_LASER_SPEED = 20;
        public const int BASIC_LASER_BURN_DMG = 5;
        public const int BASIC_LASER_BURN_TICKS = 3;
        public const int BASIC_LASER_BURN_CHANCE = 20;

        public const int PLASMA_SPEED = 20;

        public const int BONUS_SPEED = 15;
        public const int BONUS_SIZE = 15;
        public const int BONUS_VALUE = 10;
        public const int BONUS_DROP_CHANCE = 100;

        public const int GOLD_DROP_CHANCE = 50;
        public const int GOLD_AVERAGE_VALUE = 50;

        public const int METEOR_MIN_SIZE = 25;
        public const int METEOR_MAX_SIZE = 100;
        public const int METEOR_AVG_DMG = 25;
        public const int METEOR_AVG_SPEED = 20;

        public const int MAX_LVL_ID = 3;
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
        Direction Direction
        {
            get;
        }
        event Action<IShip> onHits;
        void AffectShip(IShip ship);
        event Action onDeath;
        event Action<IProjectile> OnRecycle;
        void Recycle();
    }

    public interface IBullet : IProjectile
    {
        Color Color
        {
            set;
            get;
        }
        event Modify dmgModifiers;
        int Damage
        {
            get;
            set;
        }
    }

    public interface IBulletFactory
    {
        int RechargeTime
        {
            get;
        }
        void AppendOnHit(Action<IShip> e);
        void AppendDmgModifier(Modify m);
        IBullet Create(Vector where);
    }

    public interface IWeapon
    {
        void Cleanup();
        Action<IBullet> BulletAdder
        {
            set;
        }
        Action<IBullet> RecycledBulletAdder
        {
            set;
        }
        void PullTheTrigger(Vector launchPos);
        IBulletFactory Ammo
        {
            get;
            set;
        }
        int ReloadTime
        {
            get;
            set;
        }
        void AppendOnShot(OnShots.OnShot e);
    }

    public interface IBody
    {
        int MaxHealth
        {
            get;
            set;
        }
        int Health
        {
            get;
        }
        int Defence
        {
            get;
            set;
        }
        void TakeDamage(int val);
        void TakeTrueDamage(int val);
        void Heal(int val);
    }

    public interface IShip : IMover, IBody
    {
        event Action onDeath;
        IWeapon Weapon
        {
            get;
            set;
        }
        int Gold
        {
            get;
            set;
        }
        int MaxSpeed
        {
            get;
            set;
        }
        void Shoot();
        bool IsHit(IMover projectile);
    }

    public interface IMovementStrategy
    {
        void ApplyStrategy(IShip m);
        void StopStrategy(IShip m);
    }

    public interface IShopItem
    {
        int Price
        {
            get;
            set;
        }
        int X
        {
            get;
            set;
        }
        int Y
        {
            get;
            set;
        }
        Vector Size
        {
            get;
            set;
        }
        bool IsSelected(Vector mouse);
        string Description
        {
            get;
            set;
        }
    }

    public interface IPlayAble
    {
        event Action<bool> Finished;
        void HandlePlayer(char key);
        void HandlePlayer_KeyUp(char key);
        void HandlePlayer_Mouse(MouseEventArgs m);
        void Begin();
        void Elapse();
        void PrepareNextStage();
        void Draw(Graphics g, Pen p);
    }

    public interface ILevel : IPlayAble
    {
        PlayerShip Player
        {
            set;
        }
        string Description
        {
            set;
            get;
        }
        void AdjustToDifficulty(Difficulty diff);
        void SetDroppables(Dictionary<Bonus, int> set, Difficulty diff);
    }

    public interface ILevelBuilder
    {
        ILevelBuilder SetDescription(string description);
        ILevelBuilder SetPlayer(PlayerShip ship);
        ILevelBuilder SetContent(Setup.LevelInfoHolder info);
        ILevelBuilder SetFinishedAction(Action<bool> action);
        ILevel Build();

    }
}