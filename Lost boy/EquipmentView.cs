using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Lost_boy
{
    public class EquipmentView
    {
        private Dictionary<Event.TextBox, IItem> representations;

        public void Draw(Graphics g, Pen p)
        {
            p.Color = Color.White;
            foreach (var pair in representations)
                pair.Key.Draw(g, p);
        }

        public IItem GetItem(Vector which)
        {
            var itemPair = representations.FirstOrDefault(pair => pair.Key.IsPressed(which));
            if (itemPair.Key == null || itemPair.Key.Text[0] == '0')
                return null;
            return itemPair.Value;
        }

        public void RemoveItem(IItem item)
        {
            Event.TextBox box = null;
            foreach (var pair in representations)
            {
                if (pair.Value == item)
                {
                    int count = 0;
                    if (int.TryParse(pair.Key.Text.Split(' ').First(), out count))
                    {
                        int newCount = count - 1;
                        pair.Key.Text = pair.Key.Text.Remove(0, pair.Key.Text.IndexOf(' '));
                        pair.Key.Text = pair.Key.Text.Insert(0, newCount.ToString());
                    }
                    else
                    {
                        box = pair.Key;
                    }
                }
            }
            if (box != null)
            {
                representations.Remove(box);
            }
        }

        public void SetForScrap(IHolder player, int xPosition)
        {
            int i = 0;
            foreach (var s in player.Scraps)
            {
                representations.Add(new Event.TextBox(
                    new Vector(xPosition, 50 + i * 50), s.Value + " " + s.Key.ToString() + " " + s.Key.Price),
                s.Key);
                ++i;
            }
        }

        public void SetForEquipables(IHolder player, int xPosition)
        {
            int i = 0;
            foreach (var e in player.Backpack)
            {
                representations.Add(new Event.TextBox(
                    new Vector(xPosition, 50 + i * 50), e.ToString() + " " + e.Price),
                    e);
                ++i;
            }
        }

        public void SetForAllItems(IHolder player, int xPosition)
        {
            int i = 0;
            foreach (var e in player.Backpack)
            {
                representations.Add(new Event.TextBox(
                    new Vector(xPosition, 50 + i * 50), e.ToString() + " " + e.Price),
                    e);
                ++i;
            }
            foreach (var s in player.Scraps)
            {
                representations.Add(new Event.TextBox(
                    new Vector(xPosition, 50 + i * 50), s.Value + " " + s.Key.ToString() + " " + s.Key.Price),
                s.Key);
                ++i;
            }
        }

        public EquipmentView()
        { 
            representations = new Dictionary<Event.TextBox, IItem>();
        }
    }
}
