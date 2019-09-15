using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lost_boy.Meteor
{
    public class MeteorDispenser
    {
        private class MeteorFactory
        {
            public IProjectile CreateRandom()
            {
                var r = VALUES.random;
                return new Meteor(
                    new Vector(r.Next(0, VALUES.WIDTH), 0),
                    new Vector(r.Next(-10, 10), r.Next(VALUES.METEOR_AVG_SPEED - 5, VALUES.METEOR_AVG_SPEED + 5)),
                    new Vector(r.Next(VALUES.METEOR_MIN_SIZE, VALUES.METEOR_MAX_SIZE),
                        r.Next(VALUES.METEOR_MIN_SIZE, VALUES.METEOR_MAX_SIZE)),
                        r.Next(VALUES.METEOR_AVG_DMG - 10, VALUES.METEOR_AVG_DMG + 10));
            }

            public IProjectile Create(Vector where, Vector speed, Vector size, int dmg)
            {
                return new Meteor(where, speed, size, dmg);
            }
        }
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

        public void GenerateMeteorWave()
        {
            int break1 = VALUES.random.Next(12);
            int break2 = VALUES.random.Next(12);
            int break3 = VALUES.random.Next(12);
            while (break1 == break2 || break2 == break3 || break3 == break1)
            {
                break2 = VALUES.random.Next(12);
                break3 = VALUES.random.Next(12);
            }
            for (int i = 0; i < 9; i++)
            {
                if (i == break1 || i == break2)
                    continue;
                MeteorAdder(factory.Create(
                    new Vector(i * VALUES.WIDTH / 12, 0),
                    new Vector(0, 20),
                    new Vector(VALUES.WIDTH / 12, VALUES.WIDTH / 12),
                    25));
            }
        }

        public void SetDifficulty(Difficulty dif)
        {
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