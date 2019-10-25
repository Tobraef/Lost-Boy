using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;

namespace Lost_boy
{
    using Enemies;
    public class ClassicLevel : ILevel
    {
        public event Action<bool> Finished;
        private List<EnemyShip> enemyShips = new List<EnemyShip>();
        private List<EnemyShip> toRemoveEnemies = new List<EnemyShip>();
        private List<IProjectile> enemyProjectiles = new List<IProjectile>();
        private List<IProjectile> playersProjectiles = new List<IProjectile>();

        private List<IProjectile> toAddPlayerProjectiles = new List<IProjectile>();

        private List<IProjectile> toRemovePlayerProjectiles = new List<IProjectile>();
        private List<IProjectile> toRemoveEnemyProjectiles = new List<IProjectile>();

        public PlayerShip Player
        {
            set;
            private get;
        }

        public string Description
        {
            get;
            set;
        }

        public void AdjustToDifficulty(Difficulty diff)
        {
            switch (diff)
            {
                case Difficulty.None:
                case Difficulty.Normal:
                    break;
                case Difficulty.Easy:
                    foreach (var enemy in enemyShips)
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
                    foreach (var enemy in enemyShips)
                    {
                        enemy.MaxHealth *= 3;
                        enemy.MaxHealth /= 2;
                        enemy.Weapon.Ammo.AppendDmgModifier((ref int damage) => damage *= 2);
                    }
                    break;
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

        public void SetDroppables(Dictionary<IBonus, int> set)
        {
            foreach (var enemy in enemyShips)
            {
                foreach (var bonus in set)
                {
                    if (bonus.Value > VALUES.random.Next(100))
                    {
                        enemy.onDeath += () =>
                        {
                            var newBonus = bonus.Key.Clone(enemy.Position
                            + new Vector(VALUES.random.Next(-10, 10),
                                        VALUES.random.Next(-10, 10)));
                            newBonus.onDeath += newBonus.Recycle;
                            EnemyBulletAdder(newBonus);
                        };
                    }
                }
            }
        }

        private void HandleEnemies_elapse()
        {
            foreach (var enemy in enemyShips)
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
                if (bullet.Position.Y < -50)
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
                enemyShips.Remove(enemy);
            }
            toRemoveEnemies.Clear();

            foreach (var bullet in toRemovePlayerProjectiles)
            {
                playersProjectiles.Remove(bullet);
            }
            toRemovePlayerProjectiles.Clear();
            foreach (var bullet in toRemoveEnemyProjectiles)
            {
                enemyProjectiles.Remove(bullet);
            }
        }

        private void CheckConditions_elapse()
        {
            if (Player.Health < 1)
                Finished(false);
            if (enemyShips.Count + enemyProjectiles.Count == 0)
                Finished(true);
        }

        public virtual void Elapse()
        {
            HandleEnemies_elapse();
            HandlePlayerProjectiles_elapse();
            HandlePlayer_elapse();
            Clear_elapse();
            CheckConditions_elapse();
        }

        private void RandomEnemyFalldown()
        {
            enemyShips
                .ForEach(s =>
                {
                    if (VALUES.random.Next(100) < 50)
                        s.MovementStrategy = new FallDownStrategy(s.MaxSpeed);
                });
        }

        public void Begin()
        {
            Player.Weapon.BulletAdder = PlayerBulletAdder;
            Player.Weapon.RecycledBulletAdder = PlayerRecycledBulletAdder;
            new Thread(() =>
            {
                Thread.Sleep(60000);
                RandomEnemyFalldown();
            }).Start();
        }

        public void Draw(Graphics g, Pen p)
        {
            foreach (var e in enemyShips)
                e.Draw(g, p);
            foreach (var b in enemyProjectiles)
                b.Draw(g, p);
            foreach (var b in playersProjectiles)
                b.Draw(g, p);
            Player.Draw(g, p);
        }

        private void PlayerBulletAdder(IProjectile bullet)
        {
            bullet.OnRecycle += toRemovePlayerProjectiles.Add;
            toAddPlayerProjectiles.Add(bullet);
        }

        private void PlayerRecycledBulletAdder(IProjectile bullet)
        {
            toAddPlayerProjectiles.Add(bullet);
        }

        private void EnemyBulletAdder(IProjectile bullet)
        {
            bullet.OnRecycle += toRemoveEnemyProjectiles.Add;
            enemyProjectiles.Add(bullet);
        }

        private void EnemyRecycledBulletAdder(IProjectile bullet)
        {
            enemyProjectiles.Add(bullet);
        }

        public void HandlePlayer_Mouse(System.Windows.Forms.MouseEventArgs where)
        { }

        public void AppendEnemy(EnemyShip e)
        {
            e.Weapon.BulletAdder = EnemyBulletAdder;
            e.Weapon.RecycledBulletAdder = EnemyRecycledBulletAdder;
            e.onDeath += () => toRemoveEnemies.Add(e);
            enemyShips.Add(e);
        }

        private void Cleanup()
        {
            enemyProjectiles.Clear();
            playersProjectiles.Clear();
            toAddPlayerProjectiles.Clear();
            toRemovePlayerProjectiles.Clear();
            toRemoveEnemyProjectiles.Clear();
            Player.CleanupAfterLvl();
        }

        public ClassicLevel(string s)
        {
            Finished += b => Cleanup();
        }
    }
}