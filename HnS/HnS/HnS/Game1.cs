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
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        //Loaders and Managers
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        EntityManager entityManager;

        //Input states
        KeyboardState currentKB, prevKB;
        MouseState currentMouse, prevMouse;

        //Textures and Fonts
        Texture2D platformImage, background;

        //General vars
        int windowHeight = 600, windowWidth = 800;
        int platformHeight = 502;
        List<string> heroAssetList = new List<string>();
        List<string> enemyAssetList = new List<string>();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //Set window dimensions and make cursor visible
            this.IsMouseVisible = true;
            graphics.PreferredBackBufferHeight = windowHeight;
            graphics.PreferredBackBufferWidth = windowWidth;
        }

        protected override void Initialize()
        {
            //Create entity manager
            entityManager = new EntityManager(Content);

            //Push images for hero entity to list
            heroAssetList.Add("man1");
            heroAssetList.Add("man1Walk");
            heroAssetList.Add("man2");
            heroAssetList.Add("man2Walk");

            //Push images for enemy entity to list
            enemyAssetList.Add("enemy1");
            enemyAssetList.Add("enemy1Walk");
            enemyAssetList.Add("enemy2");
            enemyAssetList.Add("enemy2Walk");

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            platformImage = Content.Load<Texture2D>("platform");
            background = Content.Load<Texture2D>("bg");
            entityManager.createHero(new Vector2(100, platformHeight),heroAssetList);
            entityManager.createEnemy(new Vector2(550, platformHeight), enemyAssetList);
        }

        protected override void UnloadContent(){}

        protected override void Update(GameTime gameTime)
        {
            //Get new mouse and keyboard states
            currentKB = Keyboard.GetState();
            currentMouse = Mouse.GetState();

            //Quit game on Esc key press
            if (currentKB.IsKeyDown(Keys.Escape)) this.Exit();

            //EntityManager - Update all entities
            entityManager.updateAll(gameTime);

            //Set previous mouse and keyboard states
            prevKB = currentKB;
            prevMouse = currentMouse;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //Set background colour as gray
            GraphicsDevice.Clear(Color.Gray);
            spriteBatch.Begin();

            //Draw Background
            spriteBatch.Draw(background, Vector2.Zero, Color.White * 0.3f);

            //Draw platform
            spriteBatch.Draw(platformImage, Vector2.Zero, Color.White);

            //EntityManager - Draw all entities
            entityManager.drawAll(spriteBatch);
            
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
