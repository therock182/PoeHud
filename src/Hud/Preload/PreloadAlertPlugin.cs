using System;
using System.Collections.Generic;
using System.Linq;

using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Hud.UI;

using SharpDX;
using SharpDX.Direct3D9;

namespace PoeHUD.Hud.Preload
{
    public class PreloadAlertPlugin : SizedPlugin<PreloadAlertSettings>
    {
        private readonly HashSet<string> alerts;

        private readonly Dictionary<string, string> alertStrings;

        private bool areaChanged = true;

        private DateTime maxParseTime = DateTime.Now;

        private int lastCount;

        public PreloadAlertPlugin(GameController gameController, Graphics graphics, PreloadAlertSettings settings)
            : base(gameController, graphics, settings)
        {
            alerts = new HashSet<string>();
            alertStrings = LoadConfig("config/preload_alerts.txt");
            GameController.Area.OnAreaChange += OnAreaChange;
        }

        public override void Render()
        {
            base.Render();
            if (!Settings.Enable)
            {
                return;
            }

            if (areaChanged)
            {
                Parse();
                lastCount = GetNumberOfObjects();
            }
            else if (DateTime.Now <= maxParseTime)
            {
                int count = GetNumberOfObjects();
                if (lastCount != count)
                {
                    areaChanged = true;
                }
            }

            if (alerts.Count > 0)
            {
                Vector2 startPosition = StartDrawPointFunc();
                Vector2 position = startPosition;
                int maxWidth = 0;
                foreach (string alert in alerts)
                {
                    Size2 size = Graphics.DrawText(alert, Settings.TextSize, position, FontDrawFlags.Right);
                    maxWidth = Math.Max(size.Width, maxWidth);
                    position.Y += size.Height;
                }
                if (maxWidth > 0)
                {
                    var bounds = new RectangleF(startPosition.X - maxWidth - 5, startPosition.Y - 5,
                        maxWidth + 10, position.Y - startPosition.Y + 10);
                    Graphics.DrawBox(bounds, Settings.BackgroundColor);
                    Size = bounds.Size;
                    Margin = new Vector2(0, 5);
                }
            }
        }

        private int GetNumberOfObjects()
        {
            Memory memory = GameController.Memory;
            return memory.ReadInt(memory.AddressOfProcess + memory.offsets.FileRoot, 12);
        }

        private void OnAreaChange(AreaController area)
        {
            maxParseTime = area.CurrentArea.TimeEntered.AddSeconds(10);
            areaChanged = true;
        }

        private void Parse()
        {
            areaChanged = false;
            alerts.Clear();
            Memory memory = GameController.Memory;
            int pFileRoot = memory.ReadInt(memory.AddressOfProcess + memory.offsets.FileRoot);
            int count = memory.ReadInt(pFileRoot + 12);
            int listIterator = memory.ReadInt(pFileRoot + 20);
            int areaChangeCount = GameController.Game.AreaChangeCount;
            for (int i = 0; i < count; i++)
            {
                listIterator = memory.ReadInt(listIterator);
                if (memory.ReadInt(listIterator + 8) != 0 && memory.ReadInt(listIterator + 12, 36) == areaChangeCount)
                {
                    string text = memory.ReadStringU(memory.ReadInt(listIterator + 8));
                    if (text.Contains('@'))
                    {
                        text = text.Split(new[] { '@' })[0];
                    }
                    if (text.StartsWith("Metadata/Particles/human_heart"))
                    {
                        alerts.Add("Area contains Corrupted Area");
                    }
                    else if (text.StartsWith("Metadata/Monsters/Missions/MasterStrDex"))
                    {
                        alerts.Add("Area contains Vagan, Weaponmaster");
                    }
                    else if (alertStrings.ContainsKey(text))
                    {
                        Console.WriteLine("Alert because of " + text);
                        alerts.Add(alertStrings[text]);
                    }
                    else if (text.EndsWith("BossInvasion"))
                    {
                        alerts.Add("Area contains Invasion Boss");
                    }
                }
            }
        }
    }
}
