using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lost_boy
{
    public class ChangingRoom : IPlayAble
    {
        private EquipmentView playerEquipables;
        private Event.TextBox playerWeapon;
        private Event.TextBox playerAmmo;
        private Event.TextBox exit = new Event.TextBox(new Vector(VALUES.WIDTH - 100, 0), "X");
        private PlayerShip player = Form1.player;
        public event Action<bool> Finished;

        private void RefreshView()
        {
            if (playerWeapon == null ||
                !playerWeapon.Text.Equals(player.Weapon.ToString()))
                playerWeapon = new Event.TextBox(new Vector(200, 100), player.Weapon.ToString());
            if (playerAmmo == null ||
                !playerAmmo.Text.Equals(player.Weapon.Ammo.ToString()))
                playerAmmo = new Event.TextBox(new Vector(200, 150), player.Weapon.Ammo.ToString());
            playerEquipables = new EquipmentView();
            playerEquipables.SetForEquipables(player, 400);
        }


        public void HandlePlayer(char key)
        { }

        public void HandlePlayer_KeyUp(char key)
        { }

        public void HandlePlayer_Mouse(System.Windows.Forms.MouseEventArgs m)
        {
            var item = playerEquipables.GetItem(new Vector(m.X, m.Y));
            if (item != null)
            {
                ((IEquipable)item).EquipOn(player);
                RefreshView();
            }
            else if (exit.IsPressed(new Vector(m.X, m.Y)))
                Finished(true);
        }

        public void Begin()
        {
            RefreshView();
        }

        public void Elapse()
        { }

        public void Draw(System.Drawing.Graphics g, System.Drawing.Pen p)
        {
            playerEquipables.Draw(g, p);
            playerWeapon.Draw(g, p);
            playerAmmo.Draw(g, p);
            exit.Draw(g, p);
        }

        public ChangingRoom()
        { }
    }
}
