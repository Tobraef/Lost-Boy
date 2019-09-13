using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lost_boy
{
    public class ClassicLevelBuilder : ILevelBuilder
    {
        private ClassicLevel lvl;
        PlayerShip player;
        private List<EnemyShip> enemies = new List<EnemyShip>();
        private List<EnemyShip> enemiesWithSetStrategies = new List<EnemyShip>();

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
                    return new CasualEnemy(new Vector());
                case "Frosty":
                    return new FrostyEnemy(new Vector());
                case "Rocky":
                    return new RockyEnemy(new Vector());
                case "Tricky":
                    return new TrickyEnemy(p, new Vector());
            }
            return null;
        }

        private List<EnemyShip> ParseEnemies(List<string> enemyShips, Tier tier)
        {
            return enemyShips.Select(name => ParseEnemy(name, player, tier)).ToList();
        }

        private void SetStrategyForCurrentEnemies(Vector start, IEnumerable<KeyValuePair<Vector, int>> ms, int delay)
        {
            foreach (var enemy in enemies)
            {
                enemy.Teleport(start.X, start.Y);
                enemy.MovementStrategy = new LevelInitialStrategy(ms.GetEnumerator(), delay);
                delay += 5;
            }
            enemiesWithSetStrategies.AddRange(enemies);
            enemies.Clear();
        }

        private void SetDropForLevel(Tier tier, Difficulty diff)
        {
            switch(tier)
            {
                case Tier.T1:
                    lvl.SetDroppables(T1.Getters.GetDrop(), diff);
                    break;
                case Tier.T2:
                    lvl.SetDroppables(T1.Getters.GetDrop(), diff);
                    break;
                case Tier.T3:
                    lvl.SetDroppables(T1.Getters.GetDrop(), diff);
                    break;
            }
        }

        public ILevelBuilder SetContent(Setup.LevelInfoHolder info)
        {
            var enemiesIter = info.enemyShips.GetEnumerator();
            var roadsToStartsIter = info.roadsToStarts.GetEnumerator();
            while (enemiesIter.MoveNext() && roadsToStartsIter.MoveNext())
            {
                SetEnemyGroup(ParseEnemies(enemiesIter.Current, info.tier));
                SetStrategyForCurrentEnemies(
                    roadsToStartsIter.Current.Key,
                    roadsToStartsIter.Current.Value,
                    5);
            }
            foreach (var enemy in enemies)
            {
                enemy.SetDefaultMoveStrategy();
            }
            enemiesWithSetStrategies.AddRange(enemies);
            enemies.Clear();
            lvl.Enemies = enemiesWithSetStrategies;
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

        public ILevel Build()
        {
            return lvl;
        }
    }
    namespace Meteor
    {
        public class MeteorLevelBuilder : ILevelBuilder
        {
            private MeteorLevel level = new MeteorLevel();
            private Difficulty difficulty;

            private void SetDropForLevel(Tier tier, Difficulty diff)
            {
                switch (tier)
                {
                    case Tier.T1:
                        level.SetDroppables(T1.Getters.GetDrop(), diff);
                        break;
                    case Tier.T2:
                        level.SetDroppables(T1.Getters.GetDrop(), diff);
                        break;
                    case Tier.T3:
                        level.SetDroppables(T1.Getters.GetDrop(), diff);
                        break;
                }
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

            public ILevel Build()
            {
                return level;
            }
        }
    }
}
