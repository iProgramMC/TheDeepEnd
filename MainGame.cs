// #define DRAW_MINIMAP

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
        Random random;
        Texture2D blankTexture;
        Texture2D tilesTex, actorsTex, gameOverBgTex;
        SpriteFont font;

        public const int canvasWidth = 256;
        public const int canvasHeight = 240;
        public const float startScale = 3.0f;
        public const int tileSize = 16;

        public const int levelWidth = canvasWidth / tileSize;
        public const int levelHeight = canvasHeight / tileSize;
        public const int cameraScrollAheadLimit = canvasHeight / 2;
        public const float fadeOutTime = 0.5f;
        public const float fadeInTime = 0.5f;
        public const float deathTime = 1.0f;

        public const Keys jumpKey = Keys.Space;
        public const Keys leftKey = Keys.Left;
        public const Keys rightKey = Keys.Right;

        float gameScale = 1f;
        float spacePressedTimer = 0f;
        float deathTimer = 0f; // time until the game over screen shows up
        float fadeOutTimer = 0f; // fade out
        float fadeInTimer = 0f; // fade in
        int cameraY = 0;

        Level level;
        List<Actor> actors = new List<Actor>();

        KeyboardState keyboardState, prevKeyboardState;

        public MainGame()
        {
            level = new Level(this);
            random = new Random();

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
            gameOverBgTex = Hax.LoadTexture2D(GraphicsDevice, "assets/gameover.png");

            font = Content.Load<SpriteFont>("font");

            ResetEverything();
        }

        void ResetEverything()
        {
            actors.Clear();
            cameraY = 0;
            prefabY = 0;
            prefabIndex = -1;
            currentPrefab = default;
            fadeInTimer = fadeInTime;
            fadeOutTimer = 0;
            deathTimer = 0;

            for (int y = 0; y < levelHeight * 2; y++)
                GenerateRow(y);

            actors.Add(new Player(level, new Vector2(16, 16)));
        }

        public void StartDeathSequence()
        {
            deathTimer = deathTime;
        }

        int prefabY, prefabIndex;
        Prefab currentPrefab;

        public int CameraY { get => cameraY; }

        void GenerateRow(int rowToGenerate)
        {
            if (prefabY == currentPrefab.height)
            {
                // shit, we need to choose.
                currentPrefab = LevelPrefabs.GetRandomPrefab(random, ref prefabIndex);
                prefabY = 0;
            }

            // then advance the current prefab
            level.PlacePrefabSlice(rowToGenerate, prefabY, currentPrefab);
            prefabY++;
        }

        void ScrollDown(int amount)
        {
            int oldCameraY = cameraY;
            cameraY += amount;

            // if crossed a row boundary, then generate the next row
            if (oldCameraY / tileSize != cameraY / tileSize)
            {
                int rowToRegen = (oldCameraY / tileSize) % (levelHeight * 2);

                GenerateRow(rowToRegen);
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
            return (keyboardState.IsKeyDown(jumpKey) && prevKeyboardState.IsKeyUp(jumpKey)) || spacePressedTimer > 0;
        }
        public bool JumpHeld()
        {
            return keyboardState.IsKeyDown(jumpKey);
        }

        public void ConsumeJumpBuffer() { spacePressedTimer = 0f; }

        public void AddActor(Actor actor)
        {
            actors.Add(actor);
        }

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

            //if (keyboardState.IsKeyDown(Keys.Down))
            //    ScrollDown(4);

            if (deathTimer > 0)
            {
                deathTimer -= Hax.Elapsed(gameTime);

                if (deathTimer <= 0)
                {
                    deathTimer = 0;
                    // show the game over screen
                    fadeOutTimer = fadeOutTime;
                }
            }
            
            if (fadeOutTimer > 0)
            {
                fadeOutTimer -= Hax.Elapsed(gameTime);

                if (fadeOutTimer <= 0)
                {
                    fadeOutTimer = 0;
                    ResetEverything();
                    return;
                }
            }
            
            if (fadeInTimer > 0)
            {
                fadeInTimer -= Hax.Elapsed(gameTime);

                if (fadeInTimer <= 0)
                    fadeInTimer = 0;
            }

            Player plr = null;
            for (int i = 0; i < actors.Count; i++)
            {
                Actor actor = actors[i];
                actor.Update(this, gameTime);
                if (actor is Player)
                    plr = actor as Player;
            }

            if (!(plr is null))
            {
                if (plr.position.Y - cameraY > cameraScrollAheadLimit)
                {
                    ScrollDown((int)(plr.position.Y - cameraY - cameraScrollAheadLimit));
                }
            }

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
            Vector2 vec = actor.position - new Vector2((tileSize - actor.hitBoxSize.X) / 2, tileSize - actor.hitBoxSize.Y + cameraY);
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

            // note: these timers are DECREASING
            float alpha  = fadeOutTimer == 0 ? 0.0f : Hax.Lerp(0.0f, 1.0f, 1.0f - (fadeOutTimer / fadeOutTime));
            float alpha2 = fadeInTimer  == 0 ? 0.0f : Hax.Lerp(0.0f, 1.0f, fadeInTimer / fadeInTime);
            alpha = Math.Max(alpha, alpha2);

            if (alpha > 0.01f)
                spriteBatch.Draw(blankTexture, new Rectangle(0, 0, canvasWidth, canvasHeight), Color.FromNonPremultiplied(0, 0, 0, (int)(255 * alpha)));

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

            //if (!(plr is null))
            //    spriteBatch.DrawString(font, $"yo waddup {plr.position.Y}", new Vector2(0, 0), Color.White, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.0f);

#if DRAW_MINIMAP
            const int tw = 8;
            spriteBatch.Draw(blankTexture, new Rectangle(0, 0, levelWidth * tw, levelHeight * tw * 2), Color.FromNonPremultiplied(10, 10, 10, 200));
            for (int y = 0; y < levelHeight * 2; y++)
            {
                for (int x = 0; x < levelWidth; x++)
                {
                    if (level.GetTile(x, y) != eTileType.None)
                        spriteBatch.Draw(blankTexture, new Rectangle(x * tw, y * tw, tw, tw), Color.White);
                }
            }

            if (!(plr is null))
            {
                spriteBatch.Draw(blankTexture, new Rectangle((int)(plr.position.X * tw / tileSize), (int)((plr.position.Y % (tileSize * levelHeight * 2)) * tw / tileSize), tw, tw), Color.Red);
            }
#endif

            spriteBatch.End();
        }
    }
}
