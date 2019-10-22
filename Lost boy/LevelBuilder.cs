using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lost_boy
{
    using Enemies;
    public class ClassicLevelBuilder : ILevelBuilder
    {
        private ClassicLevel lvl;
        PlayerShip player;
        private List<EnemyShip> enemies = new List<EnemyShip>();
        private List<EnemyShip> enemiesWithSetStrategies = new List<EnemyShip>();
        private Vector[] formationPositions = new Vector[4] { new Vector(50, 50), new Vector(50, 100), new Vector(50, 150), new Vector(50, 200) };
        private int[] formationDistance = new int[4];

        public ILevelBuilder SetDescription(string description)
        {
            this.lvl.Description = description;
            return this;
        }

        public ILevelBuilder SetFinishedAction(Action<bool> action)
        {
            this.lvl.Finished += action;
            return this;
        }

        private void SetEnemyGroup(List<EnemyShip> group)
        {
            enemies.AddRange(group);
        }

        private EnemyShip ParseEnemy(string txt, PlayerShip p, Tier tier)
        {
            switch (txt)
            {
                case "Casual":
                    return new Enemies.CasualEnemy(new Vector(), tier);
                case "Frosty":
                    return new Enemies.FrostyEnemy(new Vector(), tier);
                case "Rocky":
                    return new Enemies.RockyEnemy(new Vector(), tier);
                case "Tricky":
                    return new Enemies.TrickyEnemy(p, new Vector(), tier);
                case "Stealthy":
                    return new Enemies.StealthyEnemy(new Vector(), tier);
            }
            return null;
        }

        private List<EnemyShip> ParseEnemies(List<string> enemyShips, Tier tier)
        {
            return enemyShips.Select(name => ParseEnemy(name, player, tier)).ToList();
        }

        private void CalculateFormation(List<List<string>> allEnemies)
        {
            int[] count = new int[4];
            foreach (var eg in allEnemies)
                foreach (var e in eg)
                {
                    switch (e)
                    {
                        case "Casual":
                            count[2]++;
                            break;
                        case "Frosty":
                            count[0]++;
                            break;
                        case "Tricky":
                            count[1]++;
                            break;
                        case "Rocky":
                            count[3]++;
                            break;
                        case "Stealthy":
                            break;
                    }
                }
            for (int i = 0; i < 4; ++i)
                formationDistance[i] = (VALUES.WIDTH - 150) / (count[i] - 1);
        }

        private IMovementStrategy GetGoToStrategy(EnemyShip e)
        {
            if (e is CasualEnemy)
            {
                var strategy = new GoToStrategy(formationPositions[2], 15);
                formationPositions[2].X += formationDistance[2];
                return strategy;
            }
            else if (e is FrostyEnemy)
            {
                var strategy = new GoToStrategy(formationPositions[2], 15);
                formationPositions[0].X += formationDistance[0];
                return strategy;
            }
            else if (e is RockyEnemy)
            {
                var strategy = new GoToStrategy(formationPositions[2], 15);
                formationPositions[3].X += formationDistance[3];
                return strategy;
            }
            else if (e is TrickyEnemy)
            {
                var strategy = new GoToStrategy(formationPositions[2], 15);
                formationPositions[1].X += formationDistance[1];
                return strategy;
            }
            else if (e is StealthyEnemy)
            {
                return null;
            }
            else throw new NotImplementedException("New enemy, add to GetGoToStrategy!");
        }

        private Action GetFormationCallback(EnemyShip e)
        {
            var strategy = GetGoToStrategy(e);
            return () =>
            {
                e.MovementStrategy = strategy;
            };
        }

        private void SetStrategyForCurrentEnemies(Vector start, IEnumerable<KeyValuePair<Vector, int>> ms, int delay)
        {
            foreach (var enemy in enemies)
            {
                enemy.Teleport(start.X, start.Y);
                enemy.MovementStrategy = new LevelInitialStrategy(
                    ms.GetEnumerator(), delay, GetFormationCallback(enemy));
                delay += 5;
            }
            enemiesWithSetStrategies.AddRange(enemies);
            enemies.Clear();
        }

        private void SetDropForLevel(Tier tier, Difficulty diff)
        {
            Dictionary<IBonus, int> set = new Dictionary<IBonus, int>();
            foreach (var pair in Getters.GetDrop(tier))
            {
                set.Add(pair.Key, pair.Value * (int)diff);
            }
            foreach (var pair in Getters.GetScrapDrop(tier))
            {
                set.Add(pair.Key, pair.Value * (int)diff);
            }
            set.Add(new GoldCoin(new Vector(), tier, diff), 33 * (int)diff);
            lvl.SetDroppables(set);
        }

        private List<KeyValuePair<Vector, List<KeyValuePair<Vector, int>>>> ReadRoad(IEnumerable<string> txt)
        {
            List<KeyValuePair<Vector, List<KeyValuePair<Vector, int>>>> roads
                = new List<KeyValuePair<Vector, List<KeyValuePair<Vector, int>>>>();
            foreach (var line in txt)
            {
                var words = line.Split(' ');
                if (words.Length != 3)
                {
                    roads.Add(new KeyValuePair<Vector, List<KeyValuePair<Vector, int>>>
                        (new Vector(Int32.Parse(words[2]), Int32.Parse(words[5])),
                        new List<KeyValuePair<Vector, int>>()));
                }
                else
                {
                    roads.Last().Value.Add(new KeyValuePair<Vector, int>
                        (new Vector(Int32.Parse(words[0]), Int32.Parse(words[1])),
                            Int32.Parse(words[2])));
                }
            }
            return roads;
        }

        private List<List<string>> ReadEnemies(IEnumerable<string> txt)
        {
            List<List<string>> toRet = new List<List<string>>();
            foreach (var line in txt)
            {
                toRet.Add(new List<string>());
                toRet.Last().AddRange(line.Split(' '));
            }
            return toRet;
        }

        public ILevelBuilder SetContent(Setup.LevelInfoHolder info)
        {
            var roads = ReadRoad(info.data.SkipWhile(line => line != "---").Skip(1));
            var es = ReadEnemies(info.data.TakeWhile(line => line != "---"));
            var enemiesIter = es.GetEnumerator();
            var roadsToStartsIter = roads.GetEnumerator();
            CalculateFormation(es);
            while (enemiesIter.MoveNext() && roadsToStartsIter.MoveNext())
            {
                SetEnemyGroup(ParseEnemies(enemiesIter.Current, info.tier));
                SetStrategyForCurrentEnemies(
                    roadsToStartsIter.Current.Key,
                    roadsToStartsIter.Current.Value,
                    5);
            }

            foreach (var leftEnemy in enemies)
            {
                leftEnemy.SetDefaultMoveStrategy();
            }
            enemiesWithSetStrategies.AddRange(enemies);
            enemies.Clear();

            enemiesWithSetStrategies.ForEach(s => lvl.AppendEnemy(s));
            SetDropForLevel(info.tier, info.difficulty);
            lvl.AdjustToDifficulty(info.difficulty);
            return this;
        }

        public ILevelBuilder SetPlayer(PlayerShip player)
        {
            this.lvl.Player = player;
            this.player = player;
            return this;
        }

        public ClassicLevelBuilder()
        {
            lvl = new ClassicLevel("classic");
        }

        public IPlayAble Build()
        {
            return lvl;
        }
    }
    namespace Meteor
    {
        public class MeteorLevelBuilder : ILevelBuilder
        {
            private MeteorLevel level = new MeteorLevel();

            private void SetDropForLevel(Tier tier, Difficulty diff)
            {
                Dictionary<IBonus, int> set = new Dictionary<IBonus, int>();
                foreach (var pair in Getters.GetDrop(tier))
                {
                    set.Add(pair.Key, pair.Value * (int)diff);
                }
                level.SetDroppables(set);
            }

            public ILevelBuilder SetDescription(string description)
            {
                this.level.Description = description;
                return this;
            }

            public ILevelBuilder SetPlayer(PlayerShip ship)
            {
                this.level.Player = ship;
                return this;
            }

            public ILevelBuilder SetContent(Setup.LevelInfoHolder info)
            {
                SetDropForLevel(info.tier, info.difficulty);
                this.level.AdjustToDifficulty(info.difficulty);
                return this;
            }

            public ILevelBuilder SetFinishedAction(Action<bool> action)
            {
                this.level.Finished += action;
                return this;
            }

            public IPlayAble Build()
            {
                return level;
            }
        }
    }

    public class GroceryLevelBuilder : ILevelBuilder
    {
        private Action<bool> finisher;
        private EquipmentView playersView;
        private EquipmentView shopsView;

        private class ShopItems : IHolder
        {
            public List<IEquipable> Backpack
            {
                get;
                set;
            }

            public Dictionary<IItem, int> Scraps
            {
                get;
                set;
            }

            public int Gold
            {
                get;
                set;
            }

            public ShopItems()
            {
                Scraps = new Dictionary<IItem, int>();
                Backpack = new List<IEquipable>();
            }
        }

        public ILevelBuilder SetDescription(string description)
        {
            return this;
        }

        public ILevelBuilder SetPlayer(PlayerShip ship)
        {
            playersView = new EquipmentView();
            playersView.SetForAllItems(ship, 50);
            return this;
        }

        private void AddBonuses(ShopItems items, List<Bonus> bonuses)
        {
            for (int i = 0; i < 3; ++i)
                items.Scraps.Add((Bonus)bonuses[VALUES.random.Next(0, bonuses.Count)].Clone(new Vector()), 3);
            items.Scraps.Add(new FuelBonus(new Vector()), 3);
        }

        private void AddScraps(ShopItems items, Tier tier)
        {
            int[] times = null;
            switch (tier)
            {
                case Tier.T1:
                    times = new int[] { 20, 10, 3, 1 };
                    break;
                case Tier.T2:
                    times = new int[] { 10, 10, 5, 3 };
                    break;
                case Tier.T3:
                    times = new int[] { 5, 10, 15, 7 };
                    break;
            }
            items.Scraps.Add(new Scrap(ScrapType.Carbon), times[0]);
            items.Scraps.Add(new Scrap(ScrapType.Steel), times[1]);
            items.Scraps.Add(new Scrap(ScrapType.Uranium), times[2]);
            items.Scraps.Add(new Scrap(ScrapType.Plutonium), times[3]);
        }

        private ShopItems GetRandomStuff(Tier tier)
        {
            ShopItems items = new ShopItems();
            AddBonuses(items, Getters.GetDrop(tier).Keys.ToList());
            AddScraps(items, tier);
            items.Backpack.Add(Getters.GetRandomAmmo(tier));
            return items;
        }

        public ILevelBuilder SetContent(Setup.LevelInfoHolder info)
        {
            shopsView = new EquipmentView();
            shopsView.SetForAllItems(GetRandomStuff(info.tier), VALUES.WIDTH / 2);
            return this;
        }

        public ILevelBuilder SetFinishedAction(Action<bool> action)
        {
            finisher = action;
            return this;
        }

        public IPlayAble Build()
        {
            Grocery shop = new Grocery(playersView, shopsView);
            shop.Finished += finisher;
            return shop;
        }
    }
}