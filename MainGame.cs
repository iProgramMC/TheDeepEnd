using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace JeffJamGame
{
    public class MainGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Matrix scaleMatrix;
        Matrix translateMatrix;

        Texture2D blankTexture;
        Texture2D stuff;

        public const int canvasWidth = 256;
        public const int canvasHeight = 240;
        public const float startScale = 3.0f;

        float gameScale = 1f;
        
		public MainGame()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			Window.AllowUserResizing = true;
			IsMouseVisible = true;

			graphics.PreferredBackBufferWidth = (int)(canvasWidth * startScale);
			graphics.PreferredBackBufferHeight = (int)(canvasHeight * startScale);
		}

        protected override void Initialize()
        {
            base.Initialize();

            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            blankTexture = new Texture2D(GraphicsDevice, 1, 1);
            blankTexture.SetData(new Color[] { Color.White });

            stuff = Hax.LoadTexture2D(GraphicsDevice, "assets/stuff.png");
        }

        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// param "gameTime" Provides a snapshot of timing values.
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            gameScale = (float)GraphicsDevice.Viewport.Height / (float) canvasHeight;
            scaleMatrix = Matrix.CreateScale(gameScale);
            translateMatrix = Matrix.CreateTranslation(new Vector3((GraphicsDevice.Viewport.Width - (float)(canvasWidth * gameScale)) / 2, 0, 0));
        }

        /// This is called when the game should draw itself.
        /// param gameTime Provides a snapshot of timing values.
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, scaleMatrix * translateMatrix);

            // draw letterboxes
            spriteBatch.Draw(blankTexture, new Rectangle(-1000, 0, 1000, canvasHeight), Color.Black);
            spriteBatch.Draw(blankTexture, new Rectangle(canvasWidth, 0, 1000, canvasHeight), Color.Black);

            spriteBatch.Draw(stuff, new Vector2(0, 0), Color.White);

            spriteBatch.End();
        }
    }
}
