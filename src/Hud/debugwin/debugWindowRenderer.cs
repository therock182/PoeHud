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

                AddPlayerinfo(rc);
                ShowOpenUiWindows(rc);
                DrawOpenWindows(rc);
            }
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



