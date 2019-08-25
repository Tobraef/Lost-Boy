using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lost_boy
{
    public class WeaponShopItem : IShopItem
    {
        IWeapon wrappedWeapon;

        public WeaponShopItem(IWeapon weapon, int price)
        {
            this.wrappedWeapon = weapon;
            this.Price = price;
        }

        public int Price
        {
            get;
            set;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public Vector Size { get; set; }

        public bool IsSelected(Vector mouse)
        {
            return
                mouse.X > X && mouse.X < X + Size.X &&
                mouse.Y > Y && mouse.Y < Y + Size.Y;
        }

        public string Description
        {
            get;
            set;
        }
    }
    public class Grocery
    {
        private readonly List<IShopItem> items = new List<IShopItem>();
    }
}
