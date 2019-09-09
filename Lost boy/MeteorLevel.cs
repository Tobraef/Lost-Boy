using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lost_boy.Meteor
{
    public class MeteorLevel : ILevel
    {
        private MeteorDispenser dispenser = new MeteorDispenser(0);
        private Dictionary<Bonus, int> droppables = new Dictionary<Bonus,int>();
        private List<IProjectile> projectiles = new List<IProjectile>();
        private List<IProjectile> toRemoveProjectiles = new List<IProjectile>();

        public event Action<bool> Finished;

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

        public void AdjustToDifficulty(Difficulty diff, int id)
        {
            dispenser.SetDifficulty(diff, id);
        }

        public void SetDroppables(Dictionary<Bonus, int> set, Difficulty diff)
        {
            foreach (var pair in set)
            {
                droppables.Add(pair.Key, pair.Value * (int)diff);
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
                    dispenser.RampUpSpeed();
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

        private void MeteorAdder(IProjectile meteor)
        {
            meteor.onDeath += () => toRemoveProjectiles.Add(meteor);
            projectiles.Add(meteor);
        }

        public void Begin()
        {
            dispenser.MeteorAdder = MeteorAdder;
        }

        private void DropRandomBonuses()
        {
            projectiles.AddRange(
            droppables
                .Where(pair => VALUES.random.Next(100) < pair.Value)
                .Select(pair => pair.Key.Clone(new Vector(VALUES.random.Next(VALUES.WIDTH), 0)))
                );
        }

        public void Elapse()
        {
            if (VALUES.random.Next(100) < 30)
                dispenser.ShootMeteor();
            if (VALUES.random.Next(100) < 10)
                DropRandomBonuses();
            Player.Move();
            foreach (var meteor in projectiles)
            {
                meteor.Move();
                if (meteor.Position.Y > VALUES.HEIGHT)
                {
                    toRemoveProjectiles.Add(meteor);
                }
                if (Player.IsHit(meteor))
                {
                    meteor.AffectShip(Player);
                }
            }
            foreach (var meteor in toRemoveProjectiles)
            {
                projectiles.Remove(meteor);
            }

            if (dispenser.LeftMeteors < 1)
                Finished(true);
            if (Player.Health < 0)
                Finished(false);
        }

        public void Draw(System.Drawing.Graphics g, System.Drawing.Pen p)
        {
            foreach (var meteor in projectiles)
            {
                meteor.Draw(g, p);
            }
            Player.Draw(g, p);
        }
    }
}
