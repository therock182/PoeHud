using System.Collections.Generic;
using System.Reflection;
using System.Drawing;
using PoeHUD.Framework;
using PoeHUD.Poe;
using PoeHUD.Game;
using PoeHUD.Poe.EntityComponents;
using PoeHUD.Poe.UI;
using SlimDX.Direct3D9;
using PoeHUD.ExileBot;
using System.Linq;


namespace PoeHUD.Hud.debugwin
{
    class debugWindowRenderer : HUDPlugin
    {
        private int lines;
        private Rect destWin;

        public override void OnEnable()
        {
        }

        public override void OnDisable()
        {
        }

        private void addLine(RenderingContext rc,string t)
        {
            rc.AddTextWithHeight(new Vec2(destWin.X , destWin.Y + (lines * 16)), t, Color.White, 8, DrawTextFormat.Left);
            lines++;
        }

        public override void Render(RenderingContext rc)
        {
            if (Settings.GetBool("debug"))
            {
                lines = 0;

                var mm = this.poe.Internal.game.IngameState.IngameUi.Minimap.SmallMinimap;
                var qt = this.poe.Internal.game.IngameState.IngameUi.QuestTracker;
                var gl = this.poe.Internal.game.IngameState.IngameUi.GemLvlUpPanel;
                Rect miniMapRect = mm.GetClientRect();
                Rect qtRect = qt.GetClientRect();
                Rect glRect = gl.GetClientRect();
                Rect clientRect;
                if (qt.IsVisible && qtRect.X + qt.Width < miniMapRect.X + miniMapRect.X + 50)
                    clientRect = qtRect;
                else
                    clientRect = miniMapRect;
                destWin = new Rect(clientRect.X, clientRect.Y + clientRect.H + 5, clientRect.W, clientRect.H);
                rc.AddBox(destWin, Color.FromArgb(180, 0, 0, 0));
                rc.AddFrame(destWin, Color.Gray, 2);

                ShowAdresses(rc);
                AddPlayerinfo(rc);
                //ShowOpenUiWindows(rc);
//                DrawOpenWindows(rc);
                showInGameUI(rc);
                //ShowAllUiWindows(rc);
            }
        }

        private void showInGameUI(RenderingContext rc)
        {
            for (int i = 0; i <= 220; i++)
            {
                int offs = i * 4;
                Element el = this.poe.Internal.IngameState.IngameUi.ReadObjectAt<Element>(offs);

                bool known = false;
                PropertyInfo[] prop = typeof(IngameUIElements).GetProperties();
                foreach (PropertyInfo p in prop)
                {
                    var m = p.MemberType;
                    if (p.PropertyType == typeof(Element) || p.PropertyType.BaseType == typeof(Element))
                    {
                        if (p.CanRead)
                        {
                            Element b = (Element)p.GetValue(this.poe.Internal.game.IngameState.IngameUi, null);
                            known = known || b.address == el.address;
                        }
                    }
                }


                //if (el.IsVisible)
                //{
                    Rect Re = el.GetClientRect();
                    Color c = Color.Gold;
                    int w = 1;
                    if (known)
                    {
                        c = Color.Red;
                        w = 2;
                    }
                    rc.AddFrame(Re, c,w);
                    rc.AddTextWithHeight(new Vec2(Re.X, Re.Y), offs.ToString("X3"), c, 6, DrawTextFormat.Left);

                //}
            }
        }

        private void ShowAllUiWindows(RenderingContext rc)
        {
            //Most parts taken from ShowUIHierarchy

            Element root = this.poe.Internal.IngameState.UIRoot;
            int[] path = new int[12];
            for (path[0] = 0x80; path[0] <= 0x210; path[0] += 4)
            {

                if (path[0] == 0x120 || path[0] == 0xd8 || path[0] == 0xa0 || path[0] == 0x154 || path[0] == 0x158)
                    continue;

                Element starting_it = this.poe.Internal.IngameState.IngameUi.ReadObjectAt<Element>(path[0]);
                
                DrawWin(rc, starting_it, path,1,2,true);
            }
        }

        private void DrawWin(RenderingContext rc, Element start, int[] path,int depth,int maxdeep,bool onlyUnknown)
        {
            bool known = false;
            PropertyInfo[] prop = typeof(IngameUIElements).GetProperties();
            foreach (PropertyInfo p in prop)
            {
                var m = p.MemberType;
                if (p.PropertyType == typeof(Element) || p.PropertyType.BaseType == typeof(Element))
                {
                    if (p.CanRead)
                    {
                        Element b = (Element)p.GetValue(this.poe.Internal.game.IngameState.IngameUi, null);
                        known = known || b.address == start.address;
                    }
                }
            }

            //if ((onlyUnknown && !known) || (!onlyUnknown))
            //{
            if (known) {
                //Most parts taken from ShowUIHierarchy
                Rect Re = start.GetClientRect();

                //string sPath = path[0].ToString("X3") + "-" + string.Join("-", path.Skip(1).Take(depth - 1));
                //int ix = depth > 0 ? path[depth - 1] : 0;
                //var c = Color.FromArgb(255, 255 - 25 * (ix % 10), 255 - 25 * ((ix % 100) / 10), 255);
                Color c = Color.Gray; 
                if (depth==1) c = Color.Red;
                //string msg = string.Format("[{2}] {1:X8} : {0}", Re, start.address, sPath);
                //rc.AddTextWithHeight(new Vec2(Re.X, Re.Y + depth * 10 - 10), sPath, c, 8, DrawTextFormat.Left);
                rc.AddFrame(Re, c);
                for (int i = 0; i < start.Children.Count; i++)
                {
                    Element child = start.Children[i];
                    path[depth] = i;
                    if (depth <= maxdeep)
                        DrawWin(rc, child, path, depth + 1, maxdeep, onlyUnknown);
                }
            }
        }


        private void ShowAdresses(RenderingContext rc)
        {
            addLine(rc, "inggameUI :" + this.poe.Internal.game.IngameState.IngameUi.address.ToString("X8"));
            addLine(rc, "ServerData:" + this.poe.Internal.IngameState.ServerData.address.ToString("X8"));
            addLine(rc, "Playerinv:" + this.poe.Internal.IngameState.ServerData.PlayerInventories.address.ToString("X8"));
        }

        private void AddPlayerinfo(RenderingContext rc)
        {
            Life l = this.poe.Player.GetComponent<Life>();
            addLine(rc, "Health =" + l.CurHP + "/" + l.MaxHP);
            foreach (Poe.Buff b in l.Buffs)
            {
                addLine(rc, b.Name + " (" + b.Timer.ToString() + ")");
            }
            addLine(rc, "----------------------------------");
        }

        private void ShowOpenUiWindows(RenderingContext rc)
        {
            PropertyInfo[] prop = typeof(IngameUIElements).GetProperties();
            foreach (PropertyInfo p in prop)
            {
                var m = p.MemberType;
                if (p.PropertyType == typeof(Element) || p.PropertyType.BaseType == typeof(Element))
                {
                    string output = p.Name;
                    if (p.CanRead)
                    {
                        Element b = (Element)p.GetValue(this.poe.Internal.game.IngameState.IngameUi, null);
                        if (b.IsVisible && b.Width > 0 && b.Height > 0)
                        {
                                
                            output += string.Format(": {0} (x={1} y={2} w={3} h={4} )", b.IsVisible ? "o" : "c", b.X, b.Y, b.Width, b.Height);
                            addLine(rc, output);
                        }
                    }
                    
                }
            }
        }

        private void DrawOpenWindows(RenderingContext rc)
        {
            PropertyInfo[] prop = typeof(IngameUIElements).GetProperties();
            foreach (PropertyInfo p in prop)
            {
                var m = p.MemberType;
                if (p.PropertyType == typeof(Element) || p.PropertyType.BaseType == typeof(Element))
                {
                    string output = p.Name;
                    if (p.CanRead)
                    {
                        Element b = (Element)p.GetValue(this.poe.Internal.game.IngameState.IngameUi, null);
                        if (b.IsVisible && b.Width > 0 && b.Height > 0)
                        {
                            Rect Re = b.GetClientRect();
                            Re = new Rect(Re.X, Re.Y, Re.W, Re.H);
                            rc.AddFrame(Re, Color.Gold, 1);
                            rc.AddTextWithHeight(new Vec2(Re.X, Re.Y), p.Name, Color.White, 6, DrawTextFormat.Left);
                        }
                    }

                }

            }
        }
    }
}



