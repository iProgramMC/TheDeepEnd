using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace JeffJamGame
{
    /*
     * Description
     * 
     * Tiles.png
     * 
     * BLOCK, Spike Up, Spike Right, Spike Left, Spike Down
     * 
     * 
     * 
     * 
     * Player.png
     * Idle, Jump, Fall, Walk1, Walk2+4, Walk3, Dive
     * HAT variations of the same moves
     * 
     */

    public class MainGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Matrix scaleMatrix;
        Matrix translateMatrix;

        Texture2D blankTexture;
        Texture2D tilesTex, actorsTex;
        SpriteFont font;

        public const int canvasWidth = 256;
        public const int canvasHeight = 240;
        public const float startScale = 3.0f;
        public const int tileSize = 16;

        public const int levelWidth = canvasWidth / tileSize;
        public const int levelHeight = canvasHeight / tileSize;

        public const Keys jumpKey = Keys.Space;
        public const Keys leftKey = Keys.Left;
        public const Keys rightKey = Keys.Right;

        float gameScale = 1f;
        float spacePressedTimer = 0f;
        int cameraY = 0;

        Level level = new Level();
        List<Actor> actors = new List<Actor>();

        KeyboardState keyboardState, prevKeyboardState;
        
        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "assets";
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

            tilesTex = Hax.LoadTexture2D(GraphicsDevice, "assets/tiles.png");
            actorsTex = Hax.LoadTexture2D(GraphicsDevice, "assets/actors.png");

            font = Content.Load<SpriteFont>("font");

            RegenerateScreen(0);
            RegenerateScreen(1);

            // add a player. for fun
            actors.Add(new Player(level, new Vector2(16, 16)));
        }

        int crap = 0;

        void RegenerateScreen(int screenToRegen)
        {
            eTileType tileType = (eTileType)(1 + crap % 6);
            level.DrawBorder(0, screenToRegen * levelHeight, levelWidth, levelHeight, tileType);
            crap++;
        }

        void ScrollDown(int amount)
        {
            int oldCameraY = cameraY;
            cameraY += amount;

            // if crossed a screen boundary, then regenerate the last screen
            if (oldCameraY / canvasHeight != cameraY / canvasHeight)
            {
                int screenToRegen = (oldCameraY / canvasHeight) % 2;

                RegenerateScreen(screenToRegen);
            }
        }

        public float GetMoveX()
        {
            float x = 0;
            if (keyboardState.IsKeyDown(leftKey))
                x -= 1.0f;
            if (keyboardState.IsKeyDown(rightKey))
                x += 1.0f;
            return x;
        }

        public bool JumpPressed()
        {
            return keyboardState.IsKeyDown(jumpKey) || spacePressedTimer > 0;
        }

        public void ConsumeJumpBuffer() { spacePressedTimer = 0f; }

        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// param "gameTime" Provides a snapshot of timing values.
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            prevKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();

            //if (keyboardState.IsKeyDown(Keys.Up))
            //    cameraY--; // camera won't be up-scrollable soon!
            //
            //if (keyboardState.IsKeyDown(Keys.Down))
            //    ScrollDown(4);

            foreach (var actor in actors)
                actor.Update(this, gameTime);

            for (int i = 0; i < actors.Count; i++)
            {
                if (actors[i].deleted)
                {
                    actors.RemoveAt(i);
                    i--;
                    continue;
                }
            }

            if (keyboardState.IsKeyDown(jumpKey) && !prevKeyboardState.IsKeyDown(jumpKey))
            {
                spacePressedTimer = 0.1f;
            }
            else
            {
                spacePressedTimer -= Hax.Elapsed(gameTime);
                if (spacePressedTimer < 0)
                    spacePressedTimer = 0;
            }

            gameScale = (float)GraphicsDevice.Viewport.Height / (float) canvasHeight;
            scaleMatrix = Matrix.CreateScale(gameScale);
            translateMatrix = Matrix.CreateTranslation(new Vector3((GraphicsDevice.Viewport.Width - (float)(canvasWidth * gameScale)) / 2, 0, 0));
        }

        void DrawLevel()
        {
            for (int y = 0; y < levelWidth + 1; y++)
            {
                for (int x = 0; x < levelWidth; x++)
                {
                    eTileType tt = level.GetTile(x, y + cameraY / tileSize);
                    if (tt == eTileType.None)
                        continue;

                    int fineY = cameraY % tileSize;

                    TileInfo ti = TileManager.GetTileInfo(tt);
                    spriteBatch.Draw(
                        tilesTex,
                        new Rectangle(x * tileSize, y * tileSize - fineY, tileSize, tileSize),
                        new Rectangle(ti.texX * tileSize, ti.texY * tileSize, tileSize, tileSize),
                        Color.White
                    );
                }
            }
        }

        void DrawActor(Actor actor)
        {
            Point frame = actor.GetFrameInActors();
            Vector2 vec = actor.position - new Vector2((tileSize - actor.hitBoxSize.X) / 2, tileSize - actor.hitBoxSize.Y);
            spriteBatch.Draw(
                actorsTex,
                vec,
                new Rectangle(frame.X * tileSize, frame.Y * tileSize, tileSize, tileSize),
                Color.White,
                0.0f,
                Vector2.Zero,
                1.0f,
                actor.facingLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                0.0f
            );
        }

        void DrawActors()
        {
            foreach (var actor in actors)
                DrawActor(actor);
        }

        /// This is called when the game should draw itself.
        /// param gameTime Provides a snapshot of timing values.
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);// FromNonPremultiplied(10, 10, 10, 255));

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, scaleMatrix * translateMatrix);

            // draw letterboxes
            spriteBatch.Draw(blankTexture, new Rectangle(-1000, 0, 1000, canvasHeight), Color.Black);
            spriteBatch.Draw(blankTexture, new Rectangle(canvasWidth, 0, 1000, canvasHeight), Color.Black);

            DrawLevel();
            DrawActors();

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, translateMatrix);

            Player plr = null;
            foreach (var actor in actors)
            {
                if (actor is Player)
                {
                    plr = actor as Player;
                    break;
                }
            }

            if (!(plr is null))
                spriteBatch.DrawString(font, $"yo waddup {plr.velocity.Y}", new Vector2(0, 0), Color.White, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.0f);

            spriteBatch.End();
        }
    }
}
