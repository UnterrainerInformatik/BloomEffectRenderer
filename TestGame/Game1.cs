// *************************************************************************** 
// This is free and unencumbered software released into the public domain.
// 
// Anyone is free to copy, modify, publish, use, compile, sell, or
// distribute this software, either in source code form or as a compiled
// binary, for any purpose, commercial or non-commercial, and by any
// means.
// 
// In jurisdictions that recognize copyright laws, the author or authors
// of this software dedicate any and all copyright interest in the
// software to the public domain. We make this dedication for the benefit
// of the public at large and to the detriment of our heirs and
// successors. We intend this dedication to be an overt act of
// relinquishment in perpetuity of all present and future rights to this
// software under copyright law.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// 
// For more information, please refer to <http://unlicense.org>
// ***************************************************************************

using System;
using System.Text;
using BloomEffectRenderer;
using BloomEffectRenderer.Utils;
using InputStateManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TestGame
{
    /// <summary>
    ///     This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private const string IMAGE_COPYRIGHT =
            "Image by Michael Beckwith\nAuthor: https://www.flickr.com/people/78207463@N04 \n" +
            "Source: https://www.flickr.com/photos/78207463@N04/8226826999/ \n'Inside St Kentigerns RC Church.'\n" +
            "Located in Blackpool, Lancashire, England, UK.";

        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private InputManager Input { get; } = new InputManager();

        private SpriteFont Font { get; set; }
        private Texture2D Image { get; set; }
        private Point Resolution { get; } = new Point(1280, 720);
        private Renderer Renderer { get; } = new Renderer();

        private bool IsBloom { get; set; } = true;
        private bool IsDebug { get; set; } = true;
        private int SettingIndex { get; set; }
        private RenderTarget2D DebugTarget { get; set; }
        private GameTime GameTime { get; set; }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferMultiSampling = true;
            graphics.PreferredBackBufferWidth = Resolution.X;
            graphics.PreferredBackBufferHeight = Resolution.Y;
            graphics.IsFullScreen = false;
            graphics.PreparingDeviceSettings += PrepareDeviceSettings;
            graphics.SynchronizeWithVerticalRetrace = true;

            IsMouseVisible = true;
            IsFixedTimeStep = true;

            Content.RootDirectory = "Content";
        }

        void PrepareDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            e.GraphicsDeviceInformation.GraphicsProfile = GraphicsProfile.HiDef;
        }

        protected override void Initialize()
        {
            base.Initialize();
            Renderer.Initialize(graphics.GraphicsDevice, Resolution);
            DebugTarget = new RenderTarget2D(graphicsDevice: graphics.GraphicsDevice,
                width: Resolution.X,
                height: Resolution.Y,
                mipMap: false,
                preferredFormat: SurfaceFormat.Color,
                preferredDepthFormat: DepthFormat.None,
                preferredMultiSampleCount: 1,
                usage: RenderTargetUsage.PreserveContents);
        }

        /// <summary>
        ///     LoadContent will be called once per game and is the place to load
        ///     all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Font = Content.Load<SpriteFont>("AnonymousPro8");
            Image = Content.Load<Texture2D>("image");
            Renderer.LoadContent(GraphicsDevice);
        }

        /// <summary>
        ///     UnloadContent will be called once per game and is the place to unload
        ///     game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            Renderer.UnloadContent();
        }

        /// <summary>
        ///     Allows the game to run logic such as updating the world,
        ///     checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            Input.Update();
            if (Input.Pad.Is.Press(Buttons.Back) || Input.Key.Is.Press(Keys.Escape))
                Exit();

            HandleInput();
            base.Update(gameTime);
        }

        private void HandleInput()
        {
            if (Input.Key.Is.Press(Keys.Space))
                IsBloom = !IsBloom;
            if (Input.Key.Is.Press(Keys.Tab))
                IsDebug = !IsDebug;

            if (Input.Key.Is.Press(Keys.OemPlus) || Input.Key.Is.Press(Keys.Add))
            {
                SettingIndex++;
                if (SettingIndex >= Setting.PRESET_SETTING.Length)
                    SettingIndex = 0;
            }
            if (Input.Key.Is.Press(Keys.OemMinus) || Input.Key.Is.Press(Keys.Subtract))
            {
                SettingIndex--;
                if (SettingIndex < 0)
                    SettingIndex = Setting.PRESET_SETTING.Length - 1;
            }

            Setting s = Setting.PRESET_SETTING[SettingIndex];
            bool m = false;
            HandleFloatInput(Keys.Q, Keys.W, .01f, s.BloomThreshold, ref m, true);
            HandleFloatInput(Keys.A, Keys.S, .1f, s.BlurAmount, ref m, true);
            HandleFloatInput(Keys.Y, Keys.X, .1f, s.BloomIntensity, ref m, true);
            HandleFloatInput(Keys.E, Keys.R, .1f, s.BloomSaturation, ref m, true);
            HandleFloatInput(Keys.D, Keys.F, .1f, s.BaseIntensity, ref m, true);
            HandleFloatInput(Keys.C, Keys.V, .1f, s.BaseSaturation, ref m, true);
        }

        private void HandleFloatInput(Keys down, Keys up, float step, Fader f, ref bool isModified, bool repeat = false)
        {
            f.Value = HandleFloatInput(down, up, step, (float) f.Value, ref isModified, repeat);
        }

        private float HandleFloatInput(Keys down, Keys up, float step, float value, ref bool isModified,
            bool repeat = false)
        {
            float v = 0;
            if (!repeat && Input.Key.Is.Press(up) || repeat && Input.Key.Is.Down(up))
            {
                v = step;
                isModified = true;
            }
            if (!repeat && Input.Key.Is.Press(down) || repeat && Input.Key.Is.Down(down))
            {
                v = -step;
                isModified = true;
            }
            return value + v;
        }

        /// <summary>
        ///     This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(DebugTarget);
            GraphicsDevice.Clear(Color.TransparentBlack);
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            // Persist gameTime to pass it to the debug-renderer for font-lerping.
            GameTime = gameTime;
            DrawImage();
            DrawDebugTarget();
            DrawText(gameTime);
            base.Draw(gameTime);
        }

        private void DrawImage()
        {
            if (IsBloom)
            {
                Renderer.Render(graphics.GraphicsDevice,
                    spriteBatch,
                    "image",
                    Image,
                    null,
                    Setting.PRESET_SETTING[SettingIndex],
                    DebugDel);
            }
            else
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                spriteBatch.Draw(Image, new Rectangle(0, 0, Resolution.X, Resolution.Y), Color.White);
                spriteBatch.End();
            }
        }

        private void DebugDel(string name, RenderTarget2D t, RenderPhase phase)
        {
            if (t != null)
            {
                // The constantly switching to and from the debug-rendertarget only works if it's set to 'preserveContents' which is something
                // I wouldn't recommend doing in a real game-environment.
                GraphicsDevice.SetRenderTarget(DebugTarget);
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                float f = 7;
                spriteBatch.Draw(t,
                    new Rectangle(((int) phase + 1) * Resolution.X / (int) f,
                        5 * (Resolution.Y / (int) f),
                        (int) (Resolution.X / f),
                        (int) (Resolution.Y / f)),
                    Color.White);
                spriteBatch.DrawString(Font,
                    phase + ":",
                    new Vector2(((int) phase + 1f) * Resolution.X / (int) f, 5 * (Resolution.Y / (int) f) - 10),
                    GetLerpColor(GameTime));
                spriteBatch.End();
            }
        }

        private void DrawDebugTarget()
        {
            if (IsDebug)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                spriteBatch.Draw(DebugTarget, new Rectangle(0, 0, Resolution.X, Resolution.Y), Color.White);
                spriteBatch.End();
            }
        }

        private Color GetLerpColor(GameTime gameTime)
        {
            var t = .5f + .5f * (float) Math.Sin(5 * gameTime.TotalGameTime.TotalSeconds);
            return Color.Lerp(Color.White, Color.Gray, t);
        }

        private void DrawText(GameTime gameTime)
        {
            Color c = GetLerpColor(gameTime);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            spriteBatch.DrawString(Font, BuildText(), new Vector2(10, 10), c);
            Vector2 s = Font.MeasureString(IMAGE_COPYRIGHT);
            spriteBatch.DrawString(Font, IMAGE_COPYRIGHT, new Vector2(30f, Resolution.Y - s.Y - 30f), c);
            spriteBatch.End();
        }

        private string BuildText()
        {
            Setting s = Setting.PRESET_SETTING[SettingIndex];
            StringBuilder sb = new StringBuilder();
            string bloom = IsBloom ? "ON" : "OFF";
            string debug = IsDebug ? "ON" : "OFF";
            sb.Append($"Blur Effect: {bloom} (SPACE)\n");
            sb.Append($"Debug View: {debug} (TAB)\n\n");
            sb.Append($"Setting: [{SettingIndex + 1}/{Setting.PRESET_SETTING.Length}] {s.Name} >(+), <(-)\n");
            sb.Append($"  BloomThreshold : {s.BloomThreshold.Value:0.###} >(q), <(w)\n");
            sb.Append($"  BlurAmount     : {s.BlurAmount.Value:0.###} >(a), <(s)\n");
            sb.Append($"  BloomIntensity : {s.BloomIntensity.Value:0.###} >(y), <(x)\n");
            sb.Append($"  BloomSaturation: {s.BloomSaturation.Value:0.###} >(e), <(r)\n");
            sb.Append($"  BaseIntensity  : {s.BaseIntensity.Value:0.###} >(d), <(f)\n");
            sb.Append($"  BaseSaturation : {s.BaseSaturation.Value:0.###} >(c), <(v)\n");
            sb.Append("\nPress <ESC> to exit!");
            return sb.ToString();
        }
    }
}