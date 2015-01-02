using System;
using System.Windows.Forms;

using PoeHUD.Framework.Helpers;
using PoeHUD.Hud.UI.Renderers;

using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Windows;

namespace PoeHUD.Hud.UI
{
    public sealed class Graphics : IDisposable
    {
        private const CreateFlags CREATE_FLAGS = CreateFlags.Multithreaded | CreateFlags.HardwareVertexProcessing;

        private readonly DeviceEx device;

        private readonly Direct3DEx direct3D;

        private readonly FontRenderer fontRenderer;

        private readonly TextureRenderer textureRenderer;

        private readonly Action reset;

        private PresentParameters presentParameters;

        private bool resized;

        private float fontFactor;

        public Graphics(RenderForm form, int width, int height)
        {
            reset = () => form.Invoke(new Action(() =>
            {
                device.Reset(presentParameters);
                fontRenderer.Flush();
                textureRenderer.Flush();
                ChangeFontFactor();
                resized = false;
            }));
            form.UserResized += (sender, args) => Resize(form.ClientSize.Width, form.ClientSize.Height);
            presentParameters = new PresentParameters
            {
                Windowed = true,
                SwapEffect = SwapEffect.Discard,
                BackBufferFormat = Format.A8R8G8B8,
                BackBufferCount = 1,
                BackBufferWidth = width,
                BackBufferHeight = height,
                PresentationInterval = PresentInterval.One,
                MultiSampleType = MultisampleType.None,
                MultiSampleQuality = 0,
                PresentFlags = PresentFlags.LockableBackBuffer
            };
            direct3D = new Direct3DEx();
            device = new DeviceEx(direct3D, 0, DeviceType.Hardware, form.Handle, CREATE_FLAGS, presentParameters);
            fontRenderer = new FontRenderer(device);
            textureRenderer = new TextureRenderer(device);
            ChangeFontFactor();
        }

        public event Action Render;

        public void RenderLoop()
        {
            while (!device.IsDisposed)
            {
                ActionHelper.TryInvoke(() =>
                {
                    if (resized)
                    {
                        reset();
                    }

                    device.Clear(ClearFlags.Target, Color.Transparent, 0, 0);
                    device.SetRenderState(RenderState.AlphaBlendEnable, true);
                    device.SetRenderState(RenderState.CullMode, Cull.Clockwise);
                    device.BeginScene();

                    fontRenderer.Begin();
                    textureRenderer.Begin();
                    Render.SafeInvoke();
                    textureRenderer.End();
                    fontRenderer.End();

                    device.EndScene();
                    device.Present();
                });
            }
        }

        public void Dispose()
        {
            if (!device.IsDisposed)
            {
                device.Dispose();
                direct3D.Dispose();
                fontRenderer.Dispose();
                textureRenderer.Dispose();
            }
        }

        private void ChangeFontFactor()
        {
            // TODO * presentParameters.BackBufferHeight / 800f
            fontFactor = 1.7f; // TODO 1.7f - size to height
        }

        private void Resize(int width, int height)
        {
            if (width > 0 && height > 0)
            {
                presentParameters.BackBufferWidth = width;
                presentParameters.BackBufferHeight = height;
                resized = true;
            }
        }

        #region FontRenderer Methods

        public Size2 DrawText(string text, float size, Vector2 position, Color color, FontDrawFlags align = FontDrawFlags.Left)
        {
            return fontRenderer.DrawText(text, "Verdana", (int)(size * fontFactor), position, color, align);
        }

        public Size2 DrawText(string text, float size, Vector2 position, FontDrawFlags align = FontDrawFlags.Left)
        {
            return fontRenderer.DrawText(text, "Verdana", (int)(size * fontFactor), position, Color.White, align);
        }

        public Size2 MeasureText(string text, float size, FontDrawFlags align = FontDrawFlags.Left)
        {
            return fontRenderer.MeasureText(text, "Verdana", (int)(size * fontFactor), align);
        }

        #endregion

        #region TextureRenderer Methods

        public void DrawBox(RectangleF rectangle, Color color)
        {
            textureRenderer.DrawBox(rectangle, color);
        }

        public void DrawFrame(RectangleF rectangle, float borderWidth, Color color)
        {
            textureRenderer.DrawFrame(rectangle, borderWidth, color);
        }

        public void DrawImage(string fileName, RectangleF rectangle, float repeatX = 1f)
        {
            DrawImage(fileName, rectangle, Color.White, repeatX);
        }

        public void DrawImage(string fileName, RectangleF rectangle, RectangleF uvCoords)
        {
            DrawImage(fileName, rectangle, uvCoords, Color.White);
        }

        public void DrawImage(string fileName, RectangleF rectangle, RectangleF uvCoords, Color color)
        {
            try
            {
                textureRenderer.DrawImage("textures/" + fileName, rectangle, uvCoords, color);
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("Failed to load texture {0}: {1}", fileName, e.Message));
                Environment.Exit(0);
            }
        }

        public void DrawImage(string fileName, RectangleF rectangle, Color color, float repeatX = 1f)
        {
            try
            {
                textureRenderer.DrawImage("textures/" + fileName, rectangle, color, repeatX);
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("Failed to load texture {0}: {1}", fileName, e.Message));
                Environment.Exit(0);
            }
        }

        #endregion
    }
}