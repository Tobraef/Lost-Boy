using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lost_boy
{
    public static class Getters
    {
        public static IItem Assembly()
        {
            return null;
        }

        public static Dictionary<Bonus, int> T1GetDrop()
        {
            Dictionary<Bonus, int> drop = new Dictionary<Bonus, int>();
            drop.Add(new BulletSpeedBonus(new Vector()), 4);
            drop.Add(new BurnBonus(new Vector()), 3);
            drop.Add(new HealthBonus(new Vector()), 15);
            drop.Add(new LaserDamageBonus(new Vector()), 5);
            drop.Add(new ShipSpeedBonus(new Vector()), 5);
            return drop;
        }

        public static Dictionary<Bonus, int> T2GetDrop()
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


        public static Dictionary<Bonus, int> T3GetDrop()
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