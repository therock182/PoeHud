using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KeyboardAndMouse;
using PoeHUD.ExileBot;

namespace PoeHUD.Hud
{
    public delegate void KbEvent();

    class keyboard
    {
        private PathOfExile poe;


        private Dictionary <Keys,Delegate> KeyBoardEvents = new Dictionary<Keys,Delegate>();

        public keyboard(PathOfExile PoE)
        {
            poe = PoE;
            HookManager.KeyDown += HookManager_KeyDown;
        }


        bool IsCtrlDown()
        {
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        bool IsShiftDown()
        {
            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        bool IsAltDown()
        {
            if ((Control.ModifierKeys & Keys.Alt) == Keys.Alt)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public void AddKeyEvent(Keys KeyCode, Delegate evt)
        {
            if (!KeyBoardEvents.ContainsKey(KeyCode)) // Event does not yet exist ?
                KeyBoardEvents.Add(KeyCode, evt); // Add to event Dictionary
            else // Event already Exist =
                KeyBoardEvents[KeyCode] = evt; // update event Dictionary
        }

        /// <summary>
        /// Global Keyboard -Event handler
        /// Will only trigger Keyboard Events when Poe is in foreground to make sure it doesnt bother other Programs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void HookManager_KeyDown(object sender, KeyEventArgs e)
        {
            if (poe.Window.IsForeground())
            {
                if (KeyBoardEvents.ContainsKey(e.KeyCode))  // Event for Key exist ? 
                {
                    KeyBoardEvents[e.KeyCode].DynamicInvoke(null);
                }
            }
        }

    }
}
