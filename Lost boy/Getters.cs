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

        public static Dictionary<Scrap, int> GetScrapDrop(Tier tier)
        {
            switch (tier)
            {
                case Tier.T1:
                    {
                        Dictionary<Scrap, int> drop = new Dictionary<Scrap, int>();
                        drop.Add(new Scrap(ScrapType.Carbon), 80);
                        drop.Add(new Scrap(ScrapType.Steel), 40);
                        drop.Add(new Scrap(ScrapType.Uranium), 10);
                        drop.Add(new Scrap(ScrapType.Plutonium), 0);
                        return drop;
                    }
                case Tier.T2:
                    {
                        
                        Dictionary<Scrap, int> drop = new Dictionary<Scrap, int>();
                        drop.Add(new Scrap(ScrapType.Carbon), 60);
                        drop.Add(new Scrap(ScrapType.Steel), 80);
                        drop.Add(new Scrap(ScrapType.Uranium), 20);
                        drop.Add(new Scrap(ScrapType.Plutonium), 5);
                        return drop;
                    }
                case Tier.T3:
                    {
                        Dictionary<Scrap, int> drop = new Dictionary<Scrap, int>();
                        drop.Add(new Scrap(ScrapType.Carbon), 40);
                        drop.Add(new Scrap(ScrapType.Steel), 100);
                        drop.Add(new Scrap(ScrapType.Uranium), 40);
                        drop.Add(new Scrap(ScrapType.Plutonium), 20);
                        return drop;
                    }
            }
            return null;
        }

        public static Dictionary<Bonus, int> GetDrop(Tier tier)
        {
            switch (tier)
            {
                case Tier.T1:
                    {
                        Dictionary<Bonus, int> drop = new Dictionary<Bonus, int>();
                        drop.Add(new BulletSpeedBonus(new Vector()), 4);
                        drop.Add(new BurnBonus(new Vector()), 3);
                        drop.Add(new HealthBonus(new Vector()), 15);
                        drop.Add(new LaserDamageBonus(new Vector()), 5);
                        drop.Add(new ShipSpeedBonus(new Vector()), 5);
                        return drop;
                    }
                case Tier.T2:
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
                case Tier.T3:
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
            return null;
        }
    }
}