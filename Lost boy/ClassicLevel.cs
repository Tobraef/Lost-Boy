using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;

namespace Lost_boy
{
    public class ClassicLevel : ILevel
    {
        public event Action<bool> Finished;
        private List<EnemyShip> toRemoveEnemies = new List<EnemyShip>();
        private List<IProjectile> enemyProjectiles = new List<IProjectile>();
        private List<IProjectile> playersProjectiles = new List<IProjectile>();

        private List<IProjectile> toAddPlayerProjectiles = new List<IProjectile>();

        private List<IProjectile> toRemoveProjectiles = new List<IProjectile>();

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
                            var newBonus = bonus.Key.Clone(enemy.Position
                            + new Vector(VALUES.random.Next(-10, 10),
                                        VALUES.random.Next(-10, 10)));
                            EnemyBulletAdder(newBonus);
                        };
                    }
                }
            }
        }

        private void HandleEnemies_elapse()
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
        }

        private void HandlePlayerProjectiles_elapse()
        {
            playersProjectiles.AddRange(toAddPlayerProjectiles);
            toAddPlayerProjectiles.Clear();
            foreach (var bullet in playersProjectiles)
            {
                bullet.Move();
                if (bullet.Position.Y + bullet.Size.Y < 0)
                {
                    bullet.Recycle();
                }
            }
        }

        private void HandlePlayer_elapse()
        {
            Player.Move();
            try
            {
                foreach (var bullet in enemyProjectiles)
                {
                    bullet.Move();
                    if (bullet.Position.Y > VALUES.HEIGHT)
                    {
                        bullet.Recycle();
                    }
                    else if (Player.IsHit(bullet))
                    {
                        bullet.AffectShip(Player);
                    }
                }
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("Enemy bullets exception {0}", e.StackTrace);
            }
        }

        private void Clear_elapse()
        {
            foreach (var enemy in toRemoveEnemies)
            {
                Enemies.Remove(enemy);
            }
            toRemoveEnemies.Clear();

            foreach (var bullet in toRemoveProjectiles)
            {
                enemyProjectiles.Remove(bullet);
                playersProjectiles.Remove(bullet);
            }
            toRemoveProjectiles.Clear();
        }

        private void CheckConditions_elapse()
        {
            if (Player.Health < 1)
                Finished(false);
            if (Enemies.Count + enemyProjectiles.Count == 0)
                Finished(true);
        }

        public void Elapse()
        {
            HandleEnemies_elapse();
            HandlePlayerProjectiles_elapse();
            HandlePlayer_elapse();
            Clear_elapse();
            CheckConditions_elapse();
        }

        private void RandomEnemyFalldown()
        {
            Enemies
                .Where(s => !(s.MovementStrategy is LevelInitialStrategy))
                .ToList()
                .ForEach(s =>
                {
                    if (VALUES.random.Next(100) < 30)
                        s.MovementStrategy = new FallDownStrategy();
                });
        }

        public void Begin()
        {
            SetEnemies();
            Player.Weapon.BulletAdder = PlayerBulletAdder;
            Player.Weapon.RecycledBulletAdder = PlayerRecycledBulletAdder;
            if (Player.Weapon.Ammo is ExplosiveBulletFactory)
            {
                (Player.Weapon.Ammo as ExplosiveBulletFactory).BulletAdder = PlayerBulletAdder;
            }
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

        private void PlayerBulletAdder(IProjectile bullet)
        {
            bullet.OnRecycle += toRemoveProjectiles.Add;
            toAddPlayerProjectiles.Add(bullet);
        }

        private void PlayerRecycledBulletAdder(IProjectile bullet)
        {
            toAddPlayerProjectiles.Add(bullet);
        }

        private void EnemyBulletAdder(IProjectile bullet)
        {
            bullet.OnRecycle += toRemoveProjectiles.Add;
            enemyProjectiles.Add(bullet);
        }

        private void EnemyRecycledBulletAdder(IProjectile bullet)
        {
            enemyProjectiles.Add(bullet);
        }

        private void SetEnemies()
        {
            foreach (var e in Enemies)
            {
                e.Weapon.BulletAdder = EnemyBulletAdder;
                e.Weapon.RecycledBulletAdder = EnemyRecycledBulletAdder;
                e.onDeath += () => toRemoveEnemies.Add(e);
                e.onDeath += () =>
                {
                    if (VALUES.GOLD_DROP_CHANCE > VALUES.random.Next(100))
                    {
                        var bonus = new GoldCoin(e.Position, VALUES.GOLD_AVERAGE_VALUE);
                        EnemyBulletAdder(bonus);
                    }
                };
            }
        }

        private void Cleanup()
        {
            enemyProjectiles.Clear();
            playersProjectiles.Clear();
            toAddPlayerProjectiles.Clear();
            toRemoveProjectiles.Clear();
            Player.CleanupAfterLvl();
        }

        public ClassicLevel(string s)
        {
            Finished += b => Cleanup();
        }
    }
}