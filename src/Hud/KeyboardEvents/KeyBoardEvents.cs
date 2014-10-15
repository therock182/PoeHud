using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PoeHUD.Hud;

namespace PoeHUD.Hud.KeyboardEvents
{
    public static class KeyBoardEvents
    {

        // switch Minimapsettings
        public static void SwitchDebug()
        {
            Settings.SetBool("debug", !Settings.GetBool("debug"));
        }

        public static void SwitchMenu()
        {
            Settings.SetBool("Window.ShowIngameMenu", !Settings.GetBool("Window.ShowIngameMenu"));
        }
    
    }
}
