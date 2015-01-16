using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Scrambler
{
    class Program
    {
        static void Main(string[] args)
        {
            ushort Csum = Scrambler.GetCSum("PoeHUD.exe");
            string hash = Csum.ToString();
            if (hash == "0") {
                Console.WriteLine("This is your first time running the program, This will randomise the hash of the program by editing the PE header so that even if the executable is scanned, it will never have the same hash twice. \n");
            }
            Console.WriteLine("Curent hash " + hash + "\n Press Any Key to continue. \n");
            Random rnd = new Random();
            ushort randu = (ushort)rnd.Next(0, 65535);
            Scrambler.ScrambleCsum("PoeHUD.exe", "PoeHUD.exe", randu);
            Csum = Scrambler.GetCSum("PoeHUD.exe");
            hash = Csum.ToString();
            Console.ReadKey();
            Console.WriteLine("New hash " + hash);
            Console.ReadKey();
        }
    }
}
