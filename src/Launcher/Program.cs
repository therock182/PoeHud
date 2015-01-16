using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace Scrambler
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string HUDLOC = "PoeHud.exe";
            if (!File.Exists(HUDLOC))
            {
                OpenFileDialog finder = new OpenFileDialog();
                finder.InitialDirectory = Application.StartupPath;
                finder.Filter = "executable files (*.exe)|*.exe";
                finder.RestoreDirectory = true;
                MessageBox.Show("Couldn't find PoeHUD.exe, please choose the HUD executable"); 

                if (finder.ShowDialog() == DialogResult.OK)
                {
                    HUDLOC = finder.SafeFileName;
                }


            }
            ushort Csum = Scrambler.GetCSum(HUDLOC);
            string hash = Csum.ToString();
            if (hash == "0") {
                Console.WriteLine("This is your first time running the program, This will randomise the hash of the program by editing the PE header so that even if the executable is scanned, it will never have the same hash twice. \n");
            }
            Console.WriteLine("Curent hash " + hash + "\n Press Any Key to continue. \n");
            Random rnd = new Random();
            ushort randu = (ushort)rnd.Next(0, 65535);
            Scrambler.ScrambleCsum(HUDLOC, HUDLOC, randu);
            Csum = Scrambler.GetCSum(HUDLOC);
            hash = Csum.ToString();
            Console.ReadKey();
            Console.WriteLine("New hash " + hash);
            Console.ReadKey();
        }
    }
}
