using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Hud;
using PoeHUD.Poe;
using System.IO;

namespace PoeHUD
{
    public class Program
    {

        private static int FindPoeProcess(out Offsets offs)
        {
            var clients = Process.GetProcessesByName(Offsets.Regular.ExeName).Select(p => Tuple.Create(p, Offsets.Regular)).ToList();
            clients.AddRange(Process.GetProcessesByName(Offsets.Steam.ExeName).Select(p => Tuple.Create(p, Offsets.Steam)));
            int ixChosen = clients.Count > 1 ? chooseSingleProcess(clients) : 0;
            if (clients.Count > 0 && ixChosen >= 0)
            {
                offs = clients[ixChosen].Item2;
                return clients[ixChosen].Item1.Id;
            }
            offs = null;
            return 0;
        }

        private static int chooseSingleProcess(List<Tuple<Process, Offsets>> clients)
        {
            String o1 = String.Format("Yes - process #{0}, started at {1}", clients[0].Item1.Id,
                clients[0].Item1.StartTime.ToLongTimeString());
            String o2 = String.Format("No - process #{0}, started at {1}", clients[1].Item1.Id,
                clients[1].Item1.StartTime.ToLongTimeString());
            const string o3 = "Cancel - quit this application";
            var answer = MessageBox.Show(null, String.Join(Environment.NewLine, o1, o2, o3),
                "Choose a PoE instance to attach to", MessageBoxButtons.YesNoCancel);
            return answer == DialogResult.Cancel ? -1 : answer == DialogResult.Yes ? 0 : 1;
        }

        [STAThread]
        public static void Main(string[] args)
        {
#if !DEBUG
            MemoryControl.Start();
#endif
            FileStream fs = new FileStream("csum", FileMode.OpenOrCreate);
            fs.Close();
            string HUDLOC = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            string lastCsums = System.IO.File.ReadAllText("csum");
            int lastCsum = string.IsNullOrEmpty(lastCsums) ? 0 : int.Parse(lastCsums);
            if (System.AppDomain.CurrentDomain.FriendlyName == "PoeHUD.exe" || System.AppDomain.CurrentDomain.FriendlyName == "ExileHUD.exe" || System.AppDomain.CurrentDomain.FriendlyName == "ExileBuddy.exe")
            {
                MessageBox.Show("Please rename the HUD for your safety.");
                return;
            }
            if (HashCheck.GetCSum(HUDLOC) == 0 | HashCheck.GetCSum(HUDLOC) == lastCsum)
            {
                MessageBox.Show("Running the Scrambler for your safety. LastCsum = " + lastCsums);
                System.IO.StreamWriter store = new System.IO.StreamWriter("csum");
                store.WriteLine(HashCheck.GetCSum(HUDLOC));
                store.Close();
                if (!File.Exists("config/scrambler.txt") || (new FileInfo("config/scrambler.txt").Length == 0))
                {
                    FileStream Ccreator = new FileStream("config/scrambler.txt", FileMode.OpenOrCreate);
                    Ccreator.Close();
                }

                System.IO.StreamWriter Cwriter = new System.IO.StreamWriter("config/scrambler.txt");
                Cwriter.WriteLine(System.AppDomain.CurrentDomain.FriendlyName);
                Cwriter.Close();
                System.Diagnostics.Process.Start("Scrambler.exe", "fromHUD");
                return;
            }
            else
            {
                System.IO.StreamWriter store = new System.IO.StreamWriter("csum");
                store.WriteLine(HashCheck.GetCSum(HUDLOC));
                store.Close();
            }

            Offsets offs;
            int pid = FindPoeProcess(out offs);

            if (pid == 0)
            {
                MessageBox.Show("Path of Exile is not running!");
                return;
            }

            Sounds.LoadSounds();

            AppDomain.CurrentDomain.UnhandledException += (sender, exceptionArgs) =>
            {
                MessageBox.Show("Program exited with message:\n " + exceptionArgs.ExceptionObject.ToString());
                Environment.Exit(1);
            };


            using (Memory memory = new Memory(offs, pid))
            {
                offs.DoPatternScans(memory);
                GameController gameController = new GameController(memory);
                gameController.RefreshState();

                Func<bool> gameEnded = () => memory.IsInvalid();
                var overlay = new ExternalOverlay(gameController, gameEnded);
                Application.Run(overlay);
            }
        }
    }
}
