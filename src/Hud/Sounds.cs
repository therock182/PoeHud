using System;
using System.Media;
using System.Windows.Forms;

namespace PoeHUD.Hud
{
    public  static class Sounds
    {
        public static SoundPlayer AlertSound;
        public static SoundPlayer DangerSound;

        public static void LoadSounds()
        {
            try
            {
                AlertSound = new SoundPlayer("sounds/alert.wav");
                AlertSound.Load();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error when loading alert.wav: " + ex.Message);
                Environment.Exit(0);
            }
            try
            {
                DangerSound = new SoundPlayer("sounds/danger.wav");
                DangerSound.Load();
            }
            catch (Exception ex2)
            {
                MessageBox.Show("Error when loading danger.wav: " + ex2.Message);
                Environment.Exit(0);
            }
        }
    }
}