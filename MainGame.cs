// #define DRAW_MINIMAP

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TheDeepEnd
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
        Texture2D depth0Layer0Tex, depth0Layer1Tex;
        Texture2D depth1TopTex, depth1Layer0Tex, depth1Layer1Tex, depth1Layer2Tex, depth2TransTex;
        Texture2D perlinNoiseTex;
        SpriteFont font;
        
        public SoundEffect sfx_damage, sfx_jump, sfx_land, sfx_peline, sfx_upgrade;

        public const int canvasWidth = 256;
        public const int canvasHeight = 240;
        public const float startScale = 3.0f;
        public const int tileSize = 16;

        public const int levelWidth = canvasWidth / tileSize;
        public const int levelHeight = canvasHeight / tileSize;
        public const int cameraScrollAheadLimit = (int) (canvasHeight * 0.4f);
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

        bool isGameOverScreenShown = false;
        bool isTitleScreenShown = false;
        float gameOverScreenPos = 0f;
        float gameOverScreenVel = 0f;
        float somethingHue = 0f;
        float noiseOffs = 0f;

        // PLAYER SCORE
        int maxPlayerY = 0;

        // HIGH SCORE
        int highScore = 0;
        bool hasHighScore = false; // does this session have a high score?

        Level level;
        List<Actor> actors = new List<Actor>();

        public List<Actor> Actors { get { return actors; } }

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

            Window.Title = "The Deep End";
        }

        protected override void Initialize()
        {
            base.Initialize();

            IsMouseVisible = true;
            Window.AllowUserResizing = true;

            isTitleScreenShown = true;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            blankTexture = new Texture2D(GraphicsDevice, 1, 1);
            blankTexture.SetData(new Color[] { Color.White });

            tilesTex = Hax.LoadTexture2D(GraphicsDevice, "assets/tiles.png");
            actorsTex = Hax.LoadTexture2D(GraphicsDevice, "assets/actors.png");
            gameOverBgTex = Hax.LoadTexture2D(GraphicsDevice, "assets/gameover.png");
            perlinNoiseTex = Hax.LoadTexture2D(GraphicsDevice, "assets/overlay.png");
            depth0Layer0Tex = Hax.LoadTexture2D(GraphicsDevice, "assets/depth_0_layer_0.png");
            depth0Layer1Tex = Hax.LoadTexture2D(GraphicsDevice, "assets/depth_0_layer_1.png");
            depth1TopTex = Hax.LoadTexture2D(GraphicsDevice, "assets/depth_1_top.png");
            depth1Layer0Tex = Hax.LoadTexture2D(GraphicsDevice, "assets/depth_1_layer_0.png");
            depth1Layer1Tex = Hax.LoadTexture2D(GraphicsDevice, "assets/depth_1_layer_1.png");
            depth1Layer2Tex = Hax.LoadTexture2D(GraphicsDevice, "assets/depth_1_layer_2.png");
            depth2TransTex = Hax.LoadTexture2D(GraphicsDevice, "assets/depth_2_transition.png");

            font = Content.Load<SpriteFont>("font");

            sfx_damage = Hax.LoadSoundEffect("assets/damage.pcm");
            sfx_jump = Hax.LoadSoundEffect("assets/jump.pcm");
            sfx_land = Hax.LoadSoundEffect("assets/land.pcm");
            sfx_peline = Hax.LoadSoundEffect("assets/paline.pcm");
            sfx_upgrade = Hax.LoadSoundEffect("assets/upgrade.pcm");

            ResetEverything();
        }

        int actualRowY = 0;

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
            isGameOverScreenShown = false;
            maxPlayerY = 0;
            isTitleScreenShown = false;
            actualRowY = 0;

            int spawnPointX = -1, spawnPointY = -1;

            for (int y = 0; y < levelHeight * 2; y++)
            {
                int spX = GenerateRow(y, actualRowY++);
                if (spX >= 0 && spawnPointX < 0)
                {
                    spawnPointX = spX;
                    spawnPointY = y;
                }
            }

            if (spawnPointX < 0)
                throw new Exception("Didn't decide on a spawn point! :(");

            actors.Add(new Player(level, new Vector2(spawnPointX * 16, spawnPointY * 16)));
        }

        public void StartDeathSequence()
        {
            deathTimer = deathTime;

            // Check if there was a new high score
            if (highScore < maxPlayerY)
            {
                highScore = maxPlayerY;
                hasHighScore = true;
            }
        }

        int prefabY, prefabIndex;
        bool flipCurrentPrefab;
        Prefab currentPrefab;

        public int CameraY { get => cameraY; }

        int GenerateRow(int rowToGenerate, int actualRowY)
        {
            int spawnPointX = 0;
            if (prefabY == currentPrefab.height)
            {
                // shit, we need to choose.
                currentPrefab = LevelPrefabs.GetRandomPrefab(random, ref prefabIndex);
                prefabY = 0;
                flipCurrentPrefab = random.Next(2) != 0;
            }

            // then advance the current prefab
            level.PlacePrefabSlice(rowToGenerate, prefabY, currentPrefab, ref spawnPointX, flipCurrentPrefab, actualRowY);
            prefabY++;
            return spawnPointX;
        }

        void ScrollDown(int amount)
        {
            int oldCameraY = cameraY;
            cameraY += amount;
            maxPlayerY += amount;

            // if crossed a row boundary, then generate the next row
            if (oldCameraY / tileSize != cameraY / tileSize)
            {
                int rowToRegen = (oldCameraY / tileSize) % (levelHeight * 2);

                foreach (var actor in actors)
                {
                    if ((int)(actor.position.Y + actor.hitBoxSize.Y - cameraY) < 0)
                        actor.deleted = true;
                }

                GenerateRow(rowToRegen, actualRowY++);
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

        void UpdateGame(GameTime gameTime)
        {
            noiseOffs += Hax.Elapsed(gameTime) * 5;

            if (noiseOffs > 512)
                noiseOffs -= 512;

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

            if (Hax.Timer(ref deathTimer, gameTime) == eTimerState.Finished)
            {
                // show the game over screen
                //fadeOutTimer = fadeOutTime;
                isGameOverScreenShown = true;
                gameOverScreenPos = -canvasHeight;
                gameOverScreenVel = 400;
            }

            if (isGameOverScreenShown)
            {
                // make it fall
                gameOverScreenPos += gameOverScreenVel * Hax.Elapsed(gameTime);
                Hax.SetFloatWithTarget(ref gameOverScreenVel, 1000f, Hax.Elapsed(gameTime) * 500);

                if (gameOverScreenPos > 0)
                {
                    if (Math.Abs(gameOverScreenVel) > 50)
                        gameOverScreenVel = -gameOverScreenVel * 0.4f;
                    else
                        gameOverScreenVel = 0;
                    gameOverScreenPos = 0;
                }

                if (keyboardState.IsKeyDown(Keys.Enter) && !prevKeyboardState.IsKeyDown(Keys.Enter) && fadeOutTimer == 0)
                    fadeOutTimer = fadeOutTime;
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
                    ScrollDown((int)(plr.position.Y - cameraY - cameraScrollAheadLimit));
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

            level.UpdateSpecialEffects(gameTime);
        }

        void UpdateTitle(GameTime gameTime)
        {
            if (keyboardState.IsKeyDown(Keys.Enter) && !prevKeyboardState.IsKeyDown(Keys.Enter) && fadeOutTimer == 0)
            {
                fadeOutTimer = fadeOutTime;
            }
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

            if (Hax.Timer(ref fadeOutTimer, gameTime) == eTimerState.Finished)
            {
                ResetEverything();
                return;
            }

            Hax.Timer(ref fadeInTimer, gameTime);

            if (isTitleScreenShown)
                UpdateTitle(gameTime);
            else
                UpdateGame(gameTime);

            somethingHue += Hax.Elapsed(gameTime) * 50;
            while (somethingHue > 360)
                somethingHue -= 360;

            gameScale = (float)GraphicsDevice.Viewport.Height / (float) canvasHeight;
            scaleMatrix = Matrix.CreateScale(gameScale);
            translateMatrix = Matrix.CreateTranslation(new Vector3((GraphicsDevice.Viewport.Width - (float)(canvasWidth * gameScale)) / 2, 0, 0));
        }

        readonly int[] springFrameTable = new int[] { 4, 1, 4, 1, 4, 1, 1, 1, 1, 4, 4, 4, 2, 2, 2, 5, 5, 5, 3, 3, 3, 3, 6, 6, 0 };

        void DrawLevel()
        {
            for (int y = 0; y < levelWidth + 1; y++)
            {
                for (int x = 0; x < levelWidth; x++)
                {
                    eTileType tt = level.GetTile(x, y + cameraY / tileSize);
                    if (tt == eTileType.None)
                        continue;

                    TileInfo ti = TileManager.GetTileInfo(tt);
                    int fineY = cameraY % tileSize;
                    int frameX = ti.texX, frameY = ti.texY;

                    float f;
                    if (ti.collisionType == eCollisionType.Spring &&
                        (f = level.GetSpecialEffectTimer(new Point(x, y + cameraY / tileSize))) > 0)
                    {
                        frameX += springFrameTable[(int)((1.0f - f / Level.springSpecialEffectTime) * springFrameTable.Length)];
                    }

                    spriteBatch.Draw(
                        tilesTex,
                        new Rectangle(x * tileSize, y * tileSize - fineY, tileSize, tileSize),
                        new Rectangle(frameX * tileSize, frameY * tileSize, tileSize, tileSize),
                        Color.White
                    );
                }
            }
        }

        readonly int[] actorTable = new int[] { 3, 3, 2, 3, 4, 3, 3, 2 };
        void DrawActor(Actor actor)
        {
            Point frame = actor.GetFrameInActors();
            Vector2 vec = actor.position - new Vector2((tileSize - actor.hitBoxSize.X) / 2, tileSize - actor.hitBoxSize.Y + cameraY);

            Player player = null;
            if (actor is Player)
                player = actor as Player;

            if (!(player is null) && player.doubleJumpEnabled)
            {
                spriteBatch.Draw(
                    actorsTex,
                    vec + new Vector2(0, actorTable[frame.X]),
                    new Rectangle(1 * tileSize, 2 * tileSize, tileSize, tileSize),
                    Color.White,
                    0.0f,
                    Vector2.Zero,
                    1.0f,
                    actor.facingLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    0.0f
                );
            }

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

            if (!(player is null) && player.health > 1)
            {
                spriteBatch.Draw(
                    actorsTex,
                    vec,
                    new Rectangle(frame.X * tileSize, (frame.Y + 1) * tileSize, tileSize, tileSize),
                    Color.White,
                    0.0f,
                    Vector2.Zero,
                    1.0f,
                    actor.facingLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    0.0f
                );
            }
        }

        void DrawActors()
        {
            foreach (var actor in actors)
                DrawActor(actor);
        }

        void DrawGameOverScreenBG()
        {
            spriteBatch.Draw(gameOverBgTex, new Rectangle(0, (int)gameOverScreenPos, canvasWidth, canvasHeight), Color.White);
        }

        void DrawGameOverScreenText()
        {
            const string gameOver = "Game Over!";
            const string pressEnterStr = "Press Enter to Restart!";

            int w = (int) gameOverScreenPos;

            // Game Over !
            const float titleScale = 0.4f;
            float msr = font.MeasureString(gameOver).X * titleScale / 2;
            spriteBatch.DrawString(font, gameOver, new Vector2(canvasWidth / 2 - msr, 40 + w), Color.Black, 0.0f, Vector2.Zero, titleScale, SpriteEffects.None, 0.0f);

            const float normalScale = 0.2f;
            string scoreStr = maxPlayerY.ToString();
            string highScoreStr = highScore.ToString();
            float scoreStrSize = font.MeasureString(scoreStr).X * normalScale;
            float highScoreStrSize = font.MeasureString(highScoreStr).X * normalScale;
            float pressEnterStrSize = font.MeasureString(pressEnterStr).X * normalScale;

            spriteBatch.DrawString(font, "Score: ",      new Vector2(32, w+96),                                   Color.Black, 0.0f, Vector2.Zero, normalScale, SpriteEffects.None, 0.0f);
            spriteBatch.DrawString(font, "High Score: ", new Vector2(32, w+128),                                  Color.Black, 0.0f, Vector2.Zero, normalScale, SpriteEffects.None, 0.0f);
            spriteBatch.DrawString(font, scoreStr,       new Vector2(canvasWidth - 32 - scoreStrSize,     w+96),  Color.Black, 0.0f, Vector2.Zero, normalScale, SpriteEffects.None, 0.0f);
            spriteBatch.DrawString(font, highScoreStr,   new Vector2(canvasWidth - 32 - highScoreStrSize, w+128), Color.Black, 0.0f, Vector2.Zero, normalScale, SpriteEffects.None, 0.0f);

            Color color = Hax.HSVToRGB(somethingHue, 1.0f, 1.0f);
            spriteBatch.DrawString(font, pressEnterStr, new Vector2(canvasWidth / 2 - pressEnterStrSize / 2, w + 180), color, 0.0f, Vector2.Zero, normalScale, SpriteEffects.None, 0.0f);
        }

        void DrawFades()
        {
            // note: these timers are DECREASING
            float alpha = fadeOutTimer == 0 ? 0.0f : Hax.Lerp(0.0f, 1.0f, 1.0f - (fadeOutTimer / fadeOutTime));
            float alpha2 = fadeInTimer == 0 ? 0.0f : Hax.Lerp(0.0f, 1.0f, fadeInTimer / fadeInTime);
            alpha = Math.Max(alpha, alpha2);

            if (alpha > 0.01f)
                spriteBatch.Draw(blankTexture, new Rectangle(0, 0, canvasWidth, canvasHeight), Color.FromNonPremultiplied(0, 0, 0, (int)(255 * alpha)));
        }

        void DrawTitle()
        {
            const float titleScale = 0.4f;
            const float normalScale = 0.2f;

            const string titleStr = "The Deep End";
            const string pressEnterStr = "Press ENTER to begin!";

            float titleStrSize = font.MeasureString(titleStr).X * titleScale;
            float pressEnterStrSize = font.MeasureString(pressEnterStr).X * normalScale;

            Color color = Hax.HSVToRGB(somethingHue, 1.0f, 1.0f);
            spriteBatch.DrawString(font, titleStr, new Vector2(canvasWidth / 2 - titleStrSize / 2, 32), color, 0.0f, Vector2.Zero, titleScale, SpriteEffects.None, 0.0f);
            spriteBatch.DrawString(font, pressEnterStr, new Vector2(canvasWidth / 2 - pressEnterStrSize / 2, canvasHeight / 2), Color.White, 0.0f, Vector2.Zero, normalScale, SpriteEffects.None, 0.0f);
        }

        Color GetBGColor()
        {
            if (isTitleScreenShown)
                return Color.Black;

            return Color.FromNonPremultiplied(8, 12, 18, 255);
        }

        public const int level0Height = canvasHeight * 4;
        public const int level1Height = 1024;
        public const int level2Height = 576;

        public readonly Color bgColor = Color.FromNonPremultiplied(128, 96, 96, 255);

        void DrawDepth0Background(int yOffset, int lr)
        {
            if (lr == 0)
                if (yOffset * 0.25f + 96 >= -128) 
                    spriteBatch.Draw(depth0Layer0Tex, new Vector2(0, yOffset * 0.25f + 96), bgColor);

            if (lr == 1) spriteBatch.Draw(depth0Layer1Tex, new Vector2(0, yOffset * 0.5f + 256), bgColor);
        }

        void DrawDepth1Background(int yOffset, int lr)
        {
            yOffset += level0Height;
            if (lr == 0) spriteBatch.Draw(depth1Layer0Tex, new Vector2(0, yOffset * 0.125f), bgColor);
            if (lr == 1) spriteBatch.Draw(depth1Layer1Tex, new Vector2(0, yOffset * 0.25f), bgColor);
            if (lr == 2) spriteBatch.Draw(depth1Layer2Tex, new Vector2(0, yOffset * 0.5f), bgColor);
            if (lr == 2) spriteBatch.Draw(depth1TopTex, new Vector2(0, yOffset * 0.5f - 128), bgColor);
        }

        void DrawDepth2Background(int yOffset)
        {
            yOffset += level0Height;
            yOffset += level1Height;
            if (yOffset < 0)
                yOffset %= (192 * 2);
            spriteBatch.Draw(depth2TransTex, new Vector2(0, yOffset * 0.5f), bgColor);
            spriteBatch.Draw(depth2TransTex, new Vector2(0, yOffset * 0.5f + 192 * 1), bgColor);
            spriteBatch.Draw(depth2TransTex, new Vector2(0, yOffset * 0.5f + 192 * 2), bgColor);
        }

        void DrawBackground()
        {
            DrawDepth1Background(-cameraY, 0);
            DrawDepth1Background(-cameraY, 1);
            DrawDepth0Background(-cameraY, 0);
            DrawDepth0Background(-cameraY, 1);
            DrawDepth1Background(-cameraY, 2);
            DrawDepth0Background(-cameraY, 2);
            DrawDepth2Background(-cameraY);
        }

        /// This is called when the game should draw itself.
        /// param gameTime Provides a snapshot of timing values.
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(GetBGColor());

            // POINT CLAMP, SCALED
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, scaleMatrix * translateMatrix);

            // draw letterboxes
            spriteBatch.Draw(blankTexture, new Rectangle(-1000, 0, 1000, canvasHeight), Color.Black);
            spriteBatch.Draw(blankTexture, new Rectangle(canvasWidth, 0, 1000, canvasHeight), Color.Black);

            if (!isTitleScreenShown)
            {
                DrawBackground();
                DrawLevel();
                DrawActors();
            }

            spriteBatch.End();

            // LINEAR WRAP, SCALED, ADDITIVE
            if (!isTitleScreenShown)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, null, null, null, scaleMatrix * translateMatrix);
                spriteBatch.Draw(
                    perlinNoiseTex,
                    new Vector2(-(noiseOffs % 1), -(noiseOffs % 1)),
                    new Rectangle((int)noiseOffs, (int)(noiseOffs + cameraY / 2) % 512, canvasWidth + 1, canvasHeight + 1),
                    Color.FromNonPremultiplied(255, 255, 255, 100)
                );
                spriteBatch.End();
            }

            // POINT CLAMP, SCALED
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, scaleMatrix * translateMatrix);
            if (isGameOverScreenShown) DrawGameOverScreenBG();
            spriteBatch.End();

            // LINEAR CLAMP, SCALED
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, null, scaleMatrix * translateMatrix);

            if (isGameOverScreenShown)
                DrawGameOverScreenText();
            else if (isTitleScreenShown)
                DrawTitle();

            DrawFades();

            spriteBatch.End();

            if (!isTitleScreenShown)
            {
                // POINT CLAMP, NON SCALED
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
}
