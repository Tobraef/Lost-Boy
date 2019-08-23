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
        ILevel lvl; 
        private List<EnemyShip> enemies = new List<EnemyShip>();

        public ILevelBuilder SetDroppable(DroppableSet set)
        {
            this.lvl.Droppables = set;
            return this;
        }

        public ILevelBuilder AppendEnemy(EnemyShip ship)
        {
            enemies.Add(ship);
            return this;
        }

        public ILevelBuilder SetDescription(string description)
        {
            this.lvl.Description = description;
            return this;
        }

        public ILevelBuilder SetDifficulty(Difficulty difficulty)
        {
            this.lvl.Difficulty = difficulty;
            return this;
        }

        public ILevelBuilder SetInitialMovementStrategy(IMovementStrategy ms)
        {
            this.lvl.InitialMovementStrategy = ms;
            return this;
        }

        public ILevelBuilder SetPlayer(PlayerShip player)
        {
            this.lvl.Player = player;
            return this;
        }

        public LevelBuilder(LevelType type)
        {
            switch(type)
            {
                case LevelType.Classic:
                    lvl = new ClassicLevel("So classic");
                    break;
            }
        }

        public ILevel Build()
        {
            lvl.Enemies = enemies;
            return lvl;
        }
    }

    public class ClassicLevel : ILevel
    {
        public event Action<bool> Finished;
        private List<EnemyShip> toRemoveEnemies = new List<EnemyShip>();
        private List<IProjectile> enemyProjectiles = new List<IProjectile>();
        private List<IProjectile> playersProjectiles = new List<IProjectile>();
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

        public DroppableSet Droppables
        {
            set;
            private get;
        }

        public string Description
        {
            get;
            set;
        }

        public Difficulty Difficulty
        {
            private get;
            set;
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

        public void Elapse()
        {
            foreach (var enemy in Enemies)
            {
                enemy.Move();
                if (enemy.Position.Y > VALUES.HEIGHT)
                    enemy.Teleport(enemy.Position.X, -enemy.Size.Y);

                if (enemy.Position.X < -enemy.Size.X - 10)
                    enemy.Teleport(VALUES.WIDTH, enemy.Position.Y);
                else if (enemy.Position.X > VALUES.WIDTH + enemy.Size.X + 10)
                    enemy.Teleport(0, enemy.Position.Y);

                enemy.Shoot();
                foreach (var bullet in playersProjectiles)
                {
                    bullet.Move();
                    if (bullet.Position.Y + bullet.Size.Y < 0)
                    {
                        toRemoveProjectiles.Add(bullet);
                    }
                    else if (enemy.IsHit(bullet))
                    {
                        bullet.AffectShip(enemy);
                    }
                }
            }

            Player.Move();
            foreach (var bullet in enemyProjectiles)
            {
                bullet.Move();
                if (bullet.Position.Y > VALUES.HEIGHT)
                {
                    toRemoveProjectiles.Add(bullet);
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
            foreach (var bullet in toRemoveProjectiles)
            {
                playersProjectiles.Remove(bullet);
                enemyProjectiles.Remove(bullet);
            }
            toRemoveProjectiles.Clear();
        }

        public void Begin()
        {
            SetEnemies();
            Player.bulletAdder += PlayerBulletAdder;
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
            bullet.onDeath += () => toRemoveProjectiles.Add(bullet);
            playersProjectiles.Add(bullet);
        }

        private void EnemyBulletAdder(IProjectile bullet)
        {
            bullet.onDeath += () => toRemoveProjectiles.Add(bullet);
            enemyProjectiles.Add(bullet);
        }

        private void SetEnemies()
        {
            foreach (var e in Enemies)
            {
                e.bulletAdder += EnemyBulletAdder;
                e.onDeath += () => toRemoveEnemies.Add(e);
                e.onDeath += () =>
                {
                    var bonus = GetRandomBonus(e.Position);
                    if (bonus != null)
                    {
                        bonus.onDeath += () => this.toRemoveProjectiles.Add(bonus);
                        enemyProjectiles.Add(bonus);
                    }
                };
                e.onDeath += () =>
                {
                    if (VALUES.GOLD_DROP_CHANCE > VALUES.random.Next(100))
                    {
                        var bonus = new GoldCoin(e.Position, VALUES.GOLD_AVERAGE_VALUE);
                        bonus.onDeath += () =>
                        {
                            this.toRemoveProjectiles.Add(bonus);
                        };
                        enemyProjectiles.Add(bonus);
                    }
                };
                e.MovementStrategy = InitialMovementStrategy;
            }
            Thread th = new Thread(() =>
            {
                Thread.Sleep(5000);
                List<EnemyShip> ships = Enemies;
                foreach (var ship in ships)
                {
                    ship.SetDefaultMoveStrategy();
                }
            });
            th.Start();
        }

        private Bonus GetRandomBonus(Vector position)
        {
            if (VALUES.BONUS_DROP_CHANCE * (int)Difficulty > VALUES.random.Next(100))
            switch (Droppables)
            {
                case DroppableSet.Low:
                case DroppableSet.High:
                        switch(VALUES.random.Next(1,6))
                        {
                            case 1:
                                return new BulletSizeChangeBonus(position);
                            case 2:
                                return new BulletSpeedBonus(position);
                            case 3:
                                return new BurnBonus(position);
                            case 4:
                                return new HealthBonus(position);
                            case 5:
                                return new LaserDamageBonus(position);
                            case 6:
                                return new WeaponReloadTimeBonus(position);
                        }
                        break;
            }
            return null;
        }

        public ClassicLevel(string s)
        {
        }
    }

    //TODO IMPLEMENT public class DroppableSetOfItems 
}
