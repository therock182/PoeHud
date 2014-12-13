using System;
using System.Collections.Generic;
using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Hud.Interfaces;
using SlimDX.XAudio2;

namespace PoeHUD.Hud
{
    public class ClientHacks : HudPluginBase
    {
        private bool fullbrightEnabled;
        private bool hasSetWriteAccess;
        private Memory m;
        private bool maphackEnabled;
        private bool particlesEnabled;
        private bool zoomhackEnabled;



        public ClientHacks(GameController gameController):base(gameController)
        {

            m = GameController.Memory;
            if (Settings.GetBool("ClientHacks"))
            {
                maphackEnabled = Settings.GetBool("ClientHacks.Maphack");
                if (maphackEnabled)
                {
                    EnableMaphack();
                }
                zoomhackEnabled = Settings.GetBool("ClientHacks.Zoomhack");
                if (zoomhackEnabled)
                {
                    EnableZoomhack();
                }
                fullbrightEnabled = Settings.GetBool("ClientHacks.Fullbright");
                if (fullbrightEnabled)
                {
                    EnableFullbright();
                }
            }
        }
      
        public override void Render(RenderingContext rc, Dictionary<UiMountPoint, Vec2> mountPoints)
        {
            bool flag = Settings.GetBool("ClientHacks") && Settings.GetBool("ClientHacks.Maphack");
            if (flag != maphackEnabled)
            {
                maphackEnabled = !maphackEnabled;
                if (maphackEnabled)
                    EnableMaphack();
                else
                    DisableMaphack();
            }
            if (zoomhackEnabled && GameController.InGame)
            {
                float zFar = GameController.Game.IngameState.Camera.ZFar;
                if (zFar != 10000f)
                {
                    GameController.Game.IngameState.Camera.ZFar = 10000f;
                }
            }
            bool flag2 = Settings.GetBool("ClientHacks") && Settings.GetBool("ClientHacks.Zoomhack");
            if (flag2 != zoomhackEnabled)
            {
                zoomhackEnabled = !zoomhackEnabled;
                if (zoomhackEnabled)
                    EnableZoomhack();
                else
                    DisableZoomhack();
            }
            bool flag3 = Settings.GetBool("ClientHacks") && Settings.GetBool("ClientHacks.Fullbright");
            if (flag3 != fullbrightEnabled)
            {
                fullbrightEnabled = !fullbrightEnabled;
                if (fullbrightEnabled)
                    EnableFullbright();
                else
                    DisableFullbright();
            }
            bool flag4 = Settings.GetBool("ClientHacks") && Settings.GetBool("ClientHacks.Particles");
            if (flag4 != particlesEnabled)
            {
                particlesEnabled = !particlesEnabled;
                if (particlesEnabled)
                    EnableParticles();
                else
                    DisableParticles();
            }
        }

        public override void Dispose()
        {
            if (!m.IsInvalid())
            {
                DisableMaphack();
                DisableZoomhack();
                DisableFullbright();
                DisableParticles();
            }
        }
        
    
        private void EnableFullbright()
        {
            if (!hasSetWriteAccess)
            {
                hasSetWriteAccess = true;
                m.MakeMemoryWriteable(m.AddressOfProcess + m.offsets.Fullbright1, 4);
                m.MakeMemoryWriteable(m.AddressOfProcess + m.offsets.Fullbright2, 4);
            }
            m.WriteFloat(m.AddressOfProcess + m.offsets.Fullbright1, 15000f);
            m.WriteFloat(m.AddressOfProcess + m.offsets.Fullbright2, 5000f);
        }

        private void DisableFullbright()
        {
            m.WriteFloat(m.AddressOfProcess + m.offsets.Fullbright1, 1300f);
            m.WriteFloat(m.AddressOfProcess + m.offsets.Fullbright2, 350f);
        }

        private void EnableParticles()
        {
            m.WriteBytes(m.offsets.ParticlesCode, new byte[] {0x90, 0xE9});
        }

        private void DisableParticles()
        {
            m.WriteBytes(m.offsets.ParticlesCode, new byte[] {0x0F, 0x85});
        }


        private void EnableZoomhack()
        {
            m.WriteBytes(m.AddressOfProcess + m.offsets.ZoomHackFunc, 16);

        }

        private void DisableZoomhack()
        {
            m.WriteBytes(m.AddressOfProcess + m.offsets.ZoomHackFunc, 20);
        }

        private void EnableMaphack()
        {
            int addr = m.AddressOfProcess + m.offsets.MaphackFunc;
            if (m.ReadByte(addr) != 0x51)
            {
                Console.WriteLine("Something is wrong with maphackfunc");
            }
            else
            {
                for (; (int) m.ReadByte(addr) != 0xC3; ++addr)
                {
                    if (m.ReadByte(addr) == 0xD9 && m.ReadByte(addr + 1) == 0x00)
                        m.WriteBytes(addr + 1, 0xE8);
                }
                Console.WriteLine("Maphack applied");
            }
        }

        private void DisableMaphack()
        {
            int addr = m.AddressOfProcess + m.offsets.MaphackFunc;
            if (m.ReadByte(addr) != 0x51)
            {
                Console.WriteLine("Something is wrong with maphackfunc");
            }
            else
            {
                for (; (int)m.ReadByte(addr) != 0xC3; ++addr)
                {
                    if (m.ReadByte(addr) == 0xD9 && m.ReadByte(addr + 1) == 0xE8)
                        m.WriteBytes(addr + 1, 0x00);
                }
                Console.WriteLine("Maphack removed");
            }
        }

      
    }
}