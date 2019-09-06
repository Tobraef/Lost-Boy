using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;

namespace Lost_boy
{
    public class LevelBuilder : ILevelBuilder
    {
        private ILevel lvl;
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

        public ILevelBuilder SetEnemyGroup(List<EnemyShip> group)
        {
            enemies.AddRange(group);
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

        public ILevelBuilder SetStrategyForCurrentEnemies(Vector start, IEnumerable<KeyValuePair<Vector, int>> ms, int delay)
        {
            foreach (var enemy in enemies)
            {
                enemy.Teleport(start.X, start.Y);
                enemy.MovementStrategy = new LevelInitialStrategy(ms.GetEnumerator(), delay);
                delay += 5;
            }
            enemiesWithSetStrategies.AddRange(enemies);
            enemies.Clear();
            return this;
        }

        public ILevelBuilder SetPlayer(PlayerShip player)
        {
            this.lvl.Player = player;
            this.player = player;
            return this;
        }

        public LevelBuilder(LevelType type)
        {
            switch (type)
            {
                case LevelType.Classic:
                    lvl = new ClassicLevel("So classic");
                    break;
            }
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

    public class ClassicLevel : ILevel
    {
        public event Action<bool> Finished;
        private List<EnemyShip> toRemoveEnemies = new List<EnemyShip>();
        private List<IProjectile> enemyProjectiles = new List<IProjectile>();
        private List<IBullet> playersProjectiles = new List<IBullet>();
        private List<IProjectile> toRemoveEnemyProjectiles = new List<IProjectile>();
        private List<IBullet> toRemovePlayerProjectiles = new List<IBullet>();

        public PlayerShip Player
        {
            set;
            private get;
        }

        public IMovementStrategy InitialMovementStrategy
        {
            set;
            private get;
        }

        public List<EnemyShip> Enemies
        {
            set;
            private get;
        }

        public string Description
        {
            get;
            set;
        }

        public void AdjustToDifficulty(Difficulty diff, int difficultyStage)
        {
            switch (diff)
            {
                case Difficulty.None:
                case Difficulty.Normal:
                    break;
                case Difficulty.Easy:
                    foreach (var enemy in Enemies)
                    {
                        enemy.Defence /= 2;
                        enemy.MaxHealth *= 3;
                        enemy.MaxHealth /= 4;
                        enemy.Weapon.Ammo.AppendDmgModifier((ref int damage) =>
                        {
                            damage *= 3;
                            damage /= 4;
                        });
                    }
                    break;
                case Difficulty.Hard:
                    foreach (var enemy in Enemies)
                    {
                        enemy.MaxHealth *= 3;
                        enemy.MaxHealth /= 2;
                        enemy.Weapon.Ammo.AppendDmgModifier((ref int damage) => damage *= 2);
                    }
                    break;
            }
            difficultyStage += 10;
            foreach (var enemy in Enemies)
            {
                enemy.MaxHealth *= difficultyStage;
                enemy.MaxHealth /= 10;
                enemy.Weapon.Ammo.AppendDmgModifier((ref int damage) =>
                {
                    damage *= difficultyStage;
                    damage /= 10;
                });
                enemy.Defence += difficultyStage;
            }
        }

        public void HandlePlayer(char key)
        {
            switch (key)
            {
                case 'A':
                    if (Player.MaxSpeed > 0)
                        Player.Speed = new Vector(-Player.MaxSpeed, 0);
                    break;
                case 'D':
                    if (Player.MaxSpeed > 0)
                        Player.Speed = new Vector(Player.MaxSpeed, 0);
                    break;
                case 'S':
                    Player.Shoot();
                    break;
            }
        }

        public void HandlePlayer_KeyUp(char key)
        {
            switch (key)
            {
                case 'A':
                    Player.Speed = new Vector(0, 0);
                    break;
                case 'D':
                    Player.Speed = new Vector(0, 0);
                    break;
            }
        }

        public void SetDroppables(Dictionary<Bonus, int> set, Difficulty diff)
        {
            foreach (var enemy in Enemies)
            {
                foreach (var bonus in set)
                {
                    if (bonus.Value * (int)diff > VALUES.random.Next(100))
                    {
                        enemy.onDeath += () =>
                        {
                            EnemyBulletAdder(bonus.Key.Clone(enemy.Position
                            + new Vector(VALUES.random.Next(-10, 10),
                                        VALUES.random.Next(-10, 10)))); //offset
                        };
                    }
                }

            }
        }

        public void Elapse()
        {
            foreach (var enemy in Enemies)
            {
                enemy.Move();
                enemy.Shoot();
                foreach (var bullet in playersProjectiles)
                {
                    if (enemy.IsHit(bullet))
                    {
                        bullet.AffectShip(enemy);
                    }
                }
            }

            foreach (var bullet in playersProjectiles)
            {
                bullet.Move();
                if (bullet.Position.Y + bullet.Size.Y < 0)
                {
                    toRemovePlayerProjectiles.Add(bullet);
                }
            }

            Player.Move();
            foreach (var bullet in enemyProjectiles)
            {
                bullet.Move();
                if (bullet.Position.Y > VALUES.HEIGHT)
                {
                    toRemoveEnemyProjectiles.Add(bullet);
                }
                else if (Player.IsHit(bullet))
                {
                    bullet.AffectShip(Player);
                }
            }

            foreach (var enemy in toRemoveEnemies)
            {
                Enemies.Remove(enemy);
            }
            toRemoveEnemies.Clear();

            foreach (var bullet in toRemoveEnemyProjectiles)
            {
                enemyProjectiles.Remove(bullet);
            }
            toRemoveEnemyProjectiles.Clear();

            if (Player.Health < 1)
                Finished(true);
            if (Enemies.Count + enemyProjectiles.Count == 0)
                Finished(false);
            
        }

        private void RandomEnemyFalldown()
        {
            Enemies
                .Where(s => !(s.MovementStrategy is LevelInitialStrategy))
                .ToList()
                .ForEach(s =>
                {
                    if (VALUES.random.Next(100) < 70)
                        s.MovementStrategy = new FallDownStrategy();
                });
        }

        public void Begin()
        {
            SetEnemies();
            Player.Weapon.BulletAdder = PlayerBulletAdder;
            new Thread(() =>
            {
                Thread.Sleep(30000);
                RandomEnemyFalldown();
            }).Start();
        }

        public void Draw(Graphics g, Pen p)
        {
            foreach (var e in Enemies)
                e.Draw(g, p);
            foreach (var b in enemyProjectiles)
                b.Draw(g, p);
            foreach (var b in playersProjectiles)
                b.Draw(g, p);
            Player.Draw(g, p);
        }

        private void PlayerBulletAdder(IBullet bullet)
        {
            bullet.onDeath += () => toRemovePlayerProjectiles.Add(bullet);
            playersProjectiles.Add(bullet);
        }

        private void EnemyBulletAdder(IProjectile bullet)
        {
            bullet.onDeath += () => toRemoveEnemyProjectiles.Add(bullet);
            enemyProjectiles.Add(bullet);
        }

        private void SetEnemies()
        {
            foreach (var e in Enemies)
            {
                e.shootingRandomizer = new Random(VALUES.random.Next());
                e.Weapon.BulletAdder = EnemyBulletAdder;
                e.onDeath += () => toRemoveEnemies.Add(e);
                e.onDeath += () =>
                {
                    if (VALUES.GOLD_DROP_CHANCE > VALUES.random.Next(100))
                    {
                        var bonus = new GoldCoin(e.Position, VALUES.GOLD_AVERAGE_VALUE);
                        bonus.onDeath += () =>
                        {
                            this.toRemoveEnemyProjectiles.Add(bonus);
                        };
                        enemyProjectiles.Add(bonus);
                    }
                };
            }
        }

        public ClassicLevel(string s)
        {
        }
    }
}