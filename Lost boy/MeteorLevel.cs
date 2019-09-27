using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Lost_boy.Meteor
{
    public class MeteorLevel : IPlayAble
    {
        private MeteorDispenser dispenser = new MeteorDispenser(0);
        private bool waveTime = true;
        private Dictionary<IBonus, int> droppables = new Dictionary<IBonus, int>();
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

        public void AdjustToDifficulty(Difficulty diff)
        {
            dispenser.SetDifficulty(diff);
        }

        public void SetDroppables(Dictionary<IBonus, int> set)
        {
            foreach (var pair in set)
            {
                droppables.Add(pair.Key, pair.Value);
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
            try
            {
                var drop = droppables
                    .First(pair => VALUES.random.Next(100) < pair.Value).Key;
                MeteorAdder(drop.Clone(new Vector(VALUES.random.Next(VALUES.WIDTH), 0)));
            }
            catch (InvalidOperationException e) { return; }
        }

        private void HandleLogic_elapse()
        {
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
        }

        private void AddStuff_elapse()
        {
            if (waveTime)
            {
                dispenser.GenerateMeteorWave();
                waveTime = false;
                new Thread(() =>
                {
                    Thread.Sleep(5000);
                    waveTime = true;
                }).Start();
            }
            if (VALUES.random.Next(100) < 30)
                dispenser.ShootMeteor();
            if (VALUES.random.Next(100) < 30)
                DropRandomBonuses();
        }

        private void CheckConditions_elapse()
        {
            if (dispenser.LeftMeteors < 1)
                Finished(true);
            if (Player.Health < 0)
                Finished(false);
        }

        public void HandlePlayer_Mouse(System.Windows.Forms.MouseEventArgs where)
        { }

        private void Clean_elapse()
        {
            foreach (var meteor in toRemoveProjectiles)
            {
                projectiles.Remove(meteor);
            }
        }

        public void Elapse()
        {
            HandleLogic_elapse();
            Clean_elapse();
            AddStuff_elapse();
            CheckConditions_elapse();
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