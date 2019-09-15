using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lost_boy
{
    public enum ScrapType
    {
        Steel,
        Carbon,
        Uranium,
        Plutonium
    }

    public abstract class Scrap : IItem
    {
        public abstract ScrapType Type
        {
            get;
        }

        public abstract int Price
        {
            get;
        }

        public void AddToInventory(PlayerShip player)
        {
            player.Backpack.Add(this);
        }

        public void Equip(PlayerShip player)
        {
            throw new Exception("Cannot equip scraps");
        }
    }

    public class SteelScrap : Scrap
    {
        public override ScrapType Type
        {
            get { return ScrapType.Steel; }
        }

        public override int Price
        {
            get { return 5; }
        }
    }
}
