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
                        drop.Add(new ArmorBonus(new Vector()), 5);
                        drop.Add(new HealthIncrease(new Vector()), 5);
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
                        drop.Add(new ArmorBonus(new Vector()), 10);
                        drop.Add(new HealthIncrease(new Vector()), 10);
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
                        drop.Add(new ArmorBonus(new Vector()), 15);
                        drop.Add(new HealthIncrease(new Vector()), 15);
                        return drop;
                    }
            }
            return null;
        }

        private static string RandomAmmo()
        {
            switch(VALUES.random.Next(4))
            {
                case 0: return "Laser";
                case 1: return "Beam";
                case 2: return "Explosive";
                case 3: return "Plasma";
            }
            throw new NotImplementedException("No Ammo");
        }

        public static IEquipable GetRandomAmmo(Tier tier)
        {
            return ParseItem(RandomAmmo() + tier.ToString()[1]);
        }

        public static IEquipable ParseItem(string text)
        {
            switch (text)
            {
                case "SWeapon": return new Weapon.T1.SprayWeapon(null);
                case "DWeapon": return new Weapon.T2.DoubleWeapon(null);
                case "TWeapon": return new Weapon.T3.TripleWeapon(null);

                case "Random1": return GetRandomAmmo(Tier.T1);
                case "Random2": return GetRandomAmmo(Tier.T2);
                case "Random3": return GetRandomAmmo(Tier.T3);
                case "Laser1": return new BulletFactory.T1.FrostyLaserFactory(Direction.Up);
                //case "Laser2": return new BulletFactory.T2.HellHotFactory(Direction.Up);
                case "Laser2": return new BulletFactory.T2.IcyLaserFactory(Direction.Up);
                case "Laser3": return new BulletFactory.T3.AnnihilatorFactory(Direction.Up);
                case "Beam1": return new BulletFactory.T1.BeamFactory(Direction.Up);
                case "Beam2": return new BulletFactory.T2.MortalCoilFactory(Direction.Up);
                case "Beam3": return new BulletFactory.T3.DisintegratorFactory(Direction.Up);
                case "Plasma1": return new BulletFactory.T1.PlasmaFactory(Direction.Up);
                case "Plasma2": return new BulletFactory.T2.StarPlasmaFactory(Direction.Up);
                case "Plasma3": return new BulletFactory.T3.DecimatorFactory(Direction.Up);
                case "Explosive1": return new BulletFactory.T1.ExplosiveBulletFactory(Direction.Up);
                case "Explosive2": return new BulletFactory.T2.NapalmFactory(Direction.Up);
                case "Explosive3": return new BulletFactory.T3.ArmaggedonFactory(Direction.Up);
            }
            throw new NotImplementedException("Error parsing item name, received " + text);
        }

        public static List<IEquipable> ParseLoot(IEnumerable<string> loots)
        {
            return loots
                .Select(name => Getters.ParseItem(name))
                .ToList();
        }
    }
}