using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lost_boy
{
    namespace T1
    {
        public static class Getters
        {
            public static Dictionary<Bonus, int> GetDrop()
            {
                Dictionary<Bonus, int> drop = new Dictionary<Bonus, int>();
                drop.Add(new BulletSpeedBonus(new Vector()), 4);
                drop.Add(new BurnBonus(new Vector()), 3);
                drop.Add(new HealthBonus(new Vector()), 5);
                drop.Add(new LaserDamageBonus(new Vector()), 5);
                drop.Add(new ShipSpeedBonus(new Vector()), 5);
                return drop;
            }
        }
    }

    namespace T2
    {
        public static class Getters
        {
            public static Dictionary<Bonus, int> GetDrop()
            {
                Dictionary<Bonus, int> drop = new Dictionary<Bonus, int>();
                drop.Add(new BulletSizeChangeBonus(new Vector()), 10);
                drop.Add(new BulletSpeedBonus(new Vector()), 5);
                drop.Add(new BurnBonus(new Vector()), 6);
                drop.Add(new HealthBonus(new Vector()), 12);
                drop.Add(new LaserDamageBonus(new Vector()), 10);
                drop.Add(new WeaponReloadTimeBonus(new Vector()), 5);
                drop.Add(new ShipSpeedBonus(new Vector()), 10);
                drop.Add(new ArmorMeltBonus(new Vector()), 5);
                drop.Add(new FrostBonus(new Vector()), 5);
                return drop;
            }
        }
    }

    namespace T3
    {
        public static class Getters
        {
            public static Dictionary<Bonus, int> GetDrop()
            {
                Dictionary<Bonus, int> drop = new Dictionary<Bonus, int>();
                drop.Add(new BulletSizeChangeBonus(new Vector()), 10);
                drop.Add(new BulletSpeedBonus(new Vector()), 10);
                drop.Add(new BurnBonus(new Vector()), 10);
                drop.Add(new HealthBonus(new Vector()), 10);
                drop.Add(new LaserDamageBonus(new Vector()), 10);
                drop.Add(new WeaponReloadTimeBonus(new Vector()), 10);
                drop.Add(new ShipSpeedBonus(new Vector()), 10);
                drop.Add(new ArmorMeltBonus(new Vector()), 10);
                drop.Add(new FrostBonus(new Vector()), 10);
                return drop;
            }
        }
    }
}
