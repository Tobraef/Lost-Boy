using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lost_boy.Meteor
{
    public class MeteorDispenser
    {
        private int speed;
        private MeteorFactory factory = new MeteorFactory();
        private int leftMeteorCount = 100;

        public int LeftMeteors
        {
            get { return leftMeteorCount; }
        }

        public Action<IProjectile> MeteorAdder
        {
            set;
            private get;
        }

        public void ShootMeteor()
        {
            MeteorAdder(factory.CreateRandom());
            leftMeteorCount--;
        }

        public void SetDifficulty(Difficulty dif, int id)
        {
            speed += id * 2;
            speed *= 3 * (int)dif;
            speed /= 4;
        }

        public void RampUpSpeed()
        {
            speed++;
        }

        public void SlowDown()
        {
            speed--;
        }

        public override string ToString()
        {
            return "MeteorDispenser";
        }

        public MeteorDispenser(int speed)
        {
            this.speed = speed;
        }
    }   
}
