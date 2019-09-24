using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lost_boy
{
    public class AssemblyRoom : IPlayAble
    {
        EquipmentView playerView;
        private PlayerShip player = Form1.player;
        private Dictionary<Event.TextBox, ItemAssembly> assemblies =
            new Dictionary<Event.TextBox, ItemAssembly>();
        private Event.TextBox exit = new Event.TextBox(new Vector(VALUES.WIDTH - 100, 0), "X");
        public event Action<bool> Finished;

        public void HandlePlayer(char key)
        {}

        public void HandlePlayer_KeyUp(char key)
        {}

        public void HandlePlayer_Mouse(System.Windows.Forms.MouseEventArgs m)
        {
            Vector where = new Vector(m.X, m.Y);
            var assembly = assemblies
                .Where(kvp => kvp.Key.IsPressed(where))
                .Where(kvp => kvp.Value.CanAssembly(player))
                .FirstOrDefault();
            if (assembly.Value != null)
            { 
                assembly.Value.Assembly(player);
                playerView = new EquipmentView();
                playerView.SetForScrap(player, 400);
            }

            if (exit.IsPressed(where))
                Finished(true);
        }

        private void FillWeapons(ref int i)
        {
            assemblies.Add(new Event.TextBox(new Vector(50, 50 + i*30), "Spray Weapon"),
                new ItemAssembly(() => new Weapon.T1.SprayWeapon(player.Weapon.Ammo), new int[] { 50, 30, 10, 0 })); ++i;
            assemblies.Add(new Event.TextBox(new Vector(50, 50 + i * 30), "Double Weapon"),
                new ItemAssembly(() => new Weapon.T2.DoubleWeapon(player.Weapon.Ammo), new int[] { 50, 70, 30, 5 })); ++i;
            assemblies.Add(new Event.TextBox(new Vector(50, 50 + i * 30), "Triple Weapon"),
                new ItemAssembly(() => new Weapon.T3.TripleWeapon(player.Weapon.Ammo), new int[] { 50, 50, 40, 30 })); ++i;
        }

        private void FillBFs(ref int i)
        {

        }

        public void Begin()
        {
            int i = 0;
            playerView = new EquipmentView();
            playerView.SetForScrap(player, 400);
            FillWeapons(ref i);
            FillBFs(ref i);
        }

        public void Elapse()
        {}

        public void Draw(System.Drawing.Graphics g, System.Drawing.Pen p)
        {
            p.Color = System.Drawing.Color.White;
            foreach (var box in assemblies.Keys)
            {
                box.Draw(g, p);
            }
            playerView.Draw(g, p);
            exit.Draw(g, p);
        }
    }

    class ItemAssembly
    {
        private Func<IEquipable> assemblyInstructions;

        public int CarbonScraps { get; private set; }
        public int MetalScraps { get; private set; }
        public int UraniumScraps { get; private set; }
        public int PlutoniumScraps { get; private set; }

        public void Assembly(IHolder player)
        {
            int newC = player.Scraps.First(kvp => 
                ((Scrap)kvp.Key).Type == ScrapType.Carbon).Value - CarbonScraps;
            int newM = player.Scraps.First(kvp =>
                ((Scrap)kvp.Key).Type == ScrapType.Steel).Value - MetalScraps;
            int newU = player.Scraps.First(kvp =>
                ((Scrap)kvp.Key).Type == ScrapType.Uranium).Value - UraniumScraps;
            int newP = player.Scraps.First(kvp =>
                ((Scrap)kvp.Key).Type == ScrapType.Plutonium).Value - PlutoniumScraps;
            player.Scraps.Clear();
            player.Scraps.Add(new Scrap(ScrapType.Carbon), newC);
            player.Scraps.Add(new Scrap(ScrapType.Steel), newM);
            player.Scraps.Add(new Scrap(ScrapType.Uranium), newU);
            player.Scraps.Add(new Scrap(ScrapType.Plutonium), newP);
            player.Backpack.Add(assemblyInstructions());
        }

        public bool CanAssembly(IHolder player)
        {
            return player.Scraps.First(kvp =>
                ((Scrap)kvp.Key).Type == ScrapType.Carbon).Value >= CarbonScraps &&
                player.Scraps.First(kvp =>
                ((Scrap)kvp.Key).Type == ScrapType.Steel).Value >= MetalScraps &&
                player.Scraps.First(kvp =>
                ((Scrap)kvp.Key).Type == ScrapType.Uranium).Value >= UraniumScraps &&
                player.Scraps.First(kvp =>
                ((Scrap)kvp.Key).Type == ScrapType.Plutonium).Value >= PlutoniumScraps;
        }

        public ItemAssembly(Func<IEquipable> instructions, int[] costs)
        {
            this.assemblyInstructions = instructions;
            this.CarbonScraps = costs[0];
            this.MetalScraps = costs[1];
            this.UraniumScraps = costs[2];
            this.PlutoniumScraps = costs[3];
        }
    }
}
