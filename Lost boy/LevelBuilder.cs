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
        private Difficulty difficulty;
        private int difficultyId;
        PlayerShip player;
        private List<EnemyShip> enemies = new List<EnemyShip>();
        private List<EnemyShip> enemiesWithSetStrategies = new List<EnemyShip>();
        private Dictionary<Bonus, int> drop = new Dictionary<Bonus, int>();
        public ILevelBuilder SetDroppable(Dictionary<Bonus, int> set)
        {
            this.drop = set;
            return this;
        }

        public ILevelBuilder AppendEnemy(EnemyShip ship)
        {
            enemies.Add(ship);
            return this;
        }

        public ILevelBuilder CreateEnemy(Enemies.EnemyTypes type)
        {
            switch (type)
            {
                case Enemies.EnemyTypes.Casual:
                    enemies.Add(new CasualEnemy(new Vector()));
                    break;
                case Enemies.EnemyTypes.Frosty:
                    enemies.Add(new FrostyEnemy(new Vector()));
                    break;
                case Enemies.EnemyTypes.Rocky:
                    enemies.Add(new RockyEnemy(new Vector()));
                    break;
                case Enemies.EnemyTypes.Tricky:
                    enemies.Add(new TrickyEnemy(player, new Vector()));
                    break;
            }
            return this;
        }

        public ILevelBuilder SetDescription(string description)
        {
            this.lvl.Description = description;
            return this;
        }

        public ILevelBuilder SetDifficulty(Difficulty difficulty, int id)
        {
            this.difficultyId = id;
            this.difficulty = difficulty;
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

        private EnemyShip ParseEnemy(string txt, PlayerShip p)
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

        private List<EnemyShip> ParseEnemies(List<string> enemyShips)
        {
            return enemyShips.Select(name => ParseEnemy(name, player)).ToList();
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

        public ILevelBuilder SetContent(Setup.LevelInfoHolder info)
        {
            var enemiesIter = info.enemyShips.GetEnumerator();
            var roadsToStartsIter = info.roadsToStarts.GetEnumerator();
            while (enemiesIter.MoveNext() && roadsToStartsIter.MoveNext())
            {
                SetEnemyGroup(ParseEnemies(enemiesIter.Current));
                SetStrategyForCurrentEnemies(
                    roadsToStartsIter.Current.Key,
                    roadsToStartsIter.Current.Value,
                    5); 
            }
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
            foreach (var enemy in enemies)
            {
                enemy.SetDefaultMoveStrategy();
            }
            enemiesWithSetStrategies.AddRange(enemies);
            enemies.Clear();
            lvl.Enemies = enemiesWithSetStrategies;
            lvl.SetDroppables(drop, difficulty);
            lvl.AdjustToDifficulty(difficulty, difficultyId);
            return lvl;
        }
    }
    namespace Meteor
    {
        public class MeteorLevelBuilder : ILevelBuilder
        {

            public ILevelBuilder SetDescription(string description)
            {
                throw new NotImplementedException();
            }

            public ILevelBuilder SetPlayer(PlayerShip ship)
            {
                throw new NotImplementedException();
            }

            public ILevelBuilder AppendEnemy(EnemyShip ship)
            {
                throw new NotImplementedException();
            }

            public ILevelBuilder CreateEnemy(Enemies.EnemyTypes type)
            {
                throw new NotImplementedException();
            }

            public ILevelBuilder SetDroppable(Dictionary<Bonus, int> set)
            {
                throw new NotImplementedException();
            }

            public ILevelBuilder SetDifficulty(Difficulty difficulty, int id)
            {
                throw new NotImplementedException();
            }

            public ILevelBuilder SetContent(Setup.LevelInfoHolder info)
            {
                throw new NotImplementedException();
            }

            public ILevelBuilder SetFinishedAction(Action<bool> action)
            {
                throw new NotImplementedException();
            }

            public ILevel Build()
            {
                throw new NotImplementedException();
            }
        }
    }
}
