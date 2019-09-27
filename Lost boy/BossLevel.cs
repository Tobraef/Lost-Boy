using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lost_boy
{
    using Enemies;

    public class BossLevel : ClassicLevel
    {
        private List<EnemyShip> toAddEnemies = new List<EnemyShip>();

        public void ShipAdder(EnemyShip e)
        {
            toAddEnemies.Add(e);
        }

        public override void Elapse()
        {
            toAddEnemies.ForEach(s => AppendEnemy(s));
            toAddEnemies.Clear();
            base.Elapse();
        }

        public BossLevel() :
            base("Boss")
        {
        }
    }
}
