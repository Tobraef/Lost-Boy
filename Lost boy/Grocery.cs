using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Lost_boy
{
    public class Grocery : IPlayAble
    {
        private EquipmentView playerItemsView;
        private EquipmentView shopItemsView;
        private Event.TextBox exit = new Event.TextBox(new Vector(VALUES.WIDTH - 100, 0), "X");
        private Event.TextBox playerGold;
        private IPlayAble equipment;
        private Action<MouseEventArgs> currentMouseHandler;
        private Action<Graphics, Pen> currentDraw;

        public int Gold
        {
            get;
            set;
        }

        public Grocery(EquipmentView playerView, EquipmentView shopView)
        {
            playerItemsView = playerView;
            shopItemsView = shopView;
            playerGold = new Event.TextBox(new Vector(50, 0), "Gold: " + Form1.player.Gold);
        }

        public event Action<bool> Finished;

        public void HandlePlayer(char key)
        {
            if (key == 'c')
            {
                equipment = new ChangingRoom();
                PrepareSecondView();
            }
        }

        public void HandlePlayer_KeyUp(char key)
        { }

        private void DefaultMouseHandler(MouseEventArgs m)
        {
            Vector click = new Vector(m.X, m.Y);
            var item = playerItemsView.GetItem(click);
            if (item != null)
            {
                item.SellFrom(Form1.player);
                playerItemsView.RemoveItem(item);
            }
            else
            {
                item = shopItemsView.GetItem(click);
                if (item != null)
                {
                    if (Form1.player.Gold > item.Price)
                    {
                        Form1.player.Gold -= item.Price;
                        item.AddToInventory(Form1.player);
                        shopItemsView.RemoveItem(item);
                    }
                }
            }
            playerGold.Text = "Gold: " + Form1.player.Gold.ToString();
            if (exit.IsPressed(click))
                Finished(true);
        }

        private void DelegateMouseHandler(MouseEventArgs m)
        {
            equipment.HandlePlayer_Mouse(m);
        }

        public void HandlePlayer_Mouse(MouseEventArgs m)
        {
            currentMouseHandler(m);
        }

        private void GrantControlToSecondView()
        {
            currentMouseHandler = DelegateMouseHandler;
            currentDraw = DelegateDraw;
        }

        private void RetrieveControl()
        {
            currentMouseHandler = DefaultMouseHandler;
            currentDraw = DefaultDraw;
        }

        private void PrepareSecondView()
        {
            equipment.Finished += b => { RetrieveControl(); equipment = null; };
            GrantControlToSecondView();
            equipment.Begin();
        }

        public void Begin()
        {
            equipment = null;
            currentDraw = DefaultDraw;
            currentMouseHandler = DefaultMouseHandler;
        }

        public void Elapse()
        { }

        private void DefaultDraw(Graphics g, Pen p)
        {
            playerItemsView.Draw(g, p);
            shopItemsView.Draw(g, p);
            exit.Draw(g, p);
            playerGold.Draw(g, p);
        }

        private void DelegateDraw(Graphics g, Pen p)
        {
            equipment.Draw(g, p);
        }

        public void Draw(Graphics g, Pen p)
        {
            currentDraw(g, p);
        }
    }
}
