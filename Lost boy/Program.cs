using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Lost_boy
{
    class Program
    {
        private static async Task enumerate(List<int> l, string w)
        {
            await Task.Run(() => l.ForEach(i => Console.WriteLine(i + w)));
            Console.WriteLine("Enumerate "  + w + " finished");
        }
        private static async void function(List<int> l)
        {
            Task perform = enumerate(l, "first enum");
            Task perform2 = enumerate(l, "second enum");
            for (int i = 0; i < l.Count / 2; ++i)
                Console.WriteLine(l[i] + "call");
            Console.WriteLine("Call finished");
            await perform;
            await perform2;
            Console.WriteLine("Synchronized");
        }
        static void Main(string[] args)
        {
            //int i = 0;
            //List<int> nums = Enumerable.Repeat(++i, 100).ToList();
            //function(nums);
            //Console.WriteLine("Finished calling functions");
            Form1 f = new Form1();
            Application.Run(f);
        }
    }
}
