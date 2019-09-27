using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lost_boy.Setup;

namespace Lost_boy
{
    public class BossLevelBuilder : ILevelBuilder
    {
        private BossLevel level = new BossLevel();
        private PlayerShip player = Form1.player;

        private void RewardPlayer(Tier tier)
        {
            Getters.GetRandomAmmo(tier).AddToInventory(player);
            player.Gold += 750 * (int)tier;
            player.MaxHealth += 50;
            player.Heal(player.MaxHealth);
            player.Defence += 15;
        }

        private Enemies.EnemyShip GetRandomBoss(Tier tier)
        {
            switch (VALUES.random.Next(1))
            {
                case 0: return new Enemies.Splitter(tier, level.ShipAdder);
            }
            throw new NotImplementedException("No more bosses");
        }

        public IPlayAble Build()
        {
            return level;
        }

        public ILevelBuilder SetContent(LevelInfoHolder info)
        {
            Tier t = info.tier;
            level.AppendEnemy(GetRandomBoss(t));
            level.Finished += b => { if (b) RewardPlayer(t); };
            return this;
        }

        public ILevelBuilder SetDescription(string description)
        {
            return this;
        }

        public ILevelBuilder SetFinishedAction(Action<bool> action)
        {
            level.Finished += action;
            return this;
        }

        public ILevelBuilder SetPlayer(PlayerShip ship)
        {
            level.Player = ship;
            return this;
        }
    }
}
