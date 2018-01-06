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
using BloomEffectRenderer;
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

        private SpriteFont Font { get; set; }
        private Texture2D Image { get; set; }
        private Point Resolution { get; } = new Point(1280, 720);
        private Renderer Renderer { get; } = new Renderer();
        private bool IsBoom { get; set; } = true;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferMultiSampling = true;
            graphics.PreferredBackBufferWidth = Resolution.X;
            graphics.PreferredBackBufferHeight = Resolution.Y;
            graphics.IsFullScreen = false;
            graphics.PreparingDeviceSettings += PrepareDeviceSettings;
            graphics.SynchronizeWithVerticalRetrace = true;

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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        ///     This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            DrawImage();
            DrawText(gameTime);

            base.Draw(gameTime);
        }

        private void DrawImage()
        {
            if (IsBoom)
            {
                Renderer.Render(graphics.GraphicsDevice, spriteBatch, "image", Image, null, Settings.PRESET_SETTINGS[1]);
            }
            else
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                spriteBatch.Draw(Image, new Rectangle(0, 0, Resolution.X, Resolution.Y), Color.White);
                spriteBatch.End();
            }
        }

        private void DrawText(GameTime gameTime)
        {
            var t = .5f + .5f * (float)Math.Sin(5 * gameTime.TotalGameTime.TotalSeconds);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            Color c = Color.Lerp(Color.White, Color.Gray, t);

            spriteBatch.DrawString(Font, "More to come... Press <ESC> to exit!", new Vector2(10, 10), c);

            Vector2 s = Font.MeasureString(IMAGE_COPYRIGHT);
            spriteBatch.DrawString(Font,
                IMAGE_COPYRIGHT,
                new Vector2(30f, Resolution.Y - s.Y - 30f),
                c);

            spriteBatch.End();
        }
    }
}