using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace HnS
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        KeyboardState currentKB, prevKB;
        MouseState currentMouse, prevMouse;
        int windowHeight, windowWidth;
        int platformHeight = 502, manIndex = 0;
        Texture2D platformImage;
        Texture2D[] manImages = new Texture2D[2];
        int facing = 1; //0 = Right, 1 = left

        public Game1()
        {
            windowHeight = 600;
            windowWidth = 800;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
            graphics.PreferredBackBufferHeight = windowHeight;
            graphics.PreferredBackBufferWidth = windowWidth;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            platformImage = Content.Load<Texture2D>("platform");
            manImages[0] = Content.Load<Texture2D>("man1");
            manImages[1] = Content.Load<Texture2D>("man2");

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            currentKB = Keyboard.GetState();
            currentMouse = Mouse.GetState();
            if (currentKB.IsKeyDown(Keys.Escape)) this.Exit();

            if (currentMouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
            {
                if (manIndex == 0)
                {
                    manIndex = 1;
                }
                else
                {
                    manIndex = 0;
                }
            }

            if (currentKB.IsKeyDown(Keys.A) && prevKB.IsKeyDown(Keys.A) == false)
            {
                facing = 1;
            }

            if (currentKB.IsKeyDown(Keys.D) && prevKB.IsKeyDown(Keys.D) == false)
            {
                facing = 0;
            }

            prevKB = currentKB;
            prevMouse = currentMouse;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);
            spriteBatch.Begin();

            spriteBatch.Draw(platformImage, Vector2.Zero, Color.White);

            if (facing == 0)
            {
                spriteBatch.Draw(manImages[manIndex], new Vector2(100,
                    platformHeight - manImages[manIndex].Height), Color.White);
            }
            else
            {
                spriteBatch.Draw(manImages[manIndex], new Vector2(100, 
                    platformHeight - manImages[manIndex].Height), null, 
                    Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.FlipHorizontally, 0);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
