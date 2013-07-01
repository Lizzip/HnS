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

        //General vars
        int windowHeight = 600, windowWidth = 800;
        int platformHeight = 512;
        List<string> heroAssetList = new List<string>();
        List<string> enemyAssetList = new List<string>();
        List<string> backgroundAssetList = new List<string>();

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
            entityManager = new EntityManager(Content, platformHeight, windowWidth, windowHeight);

            //Push images for hero entity to list
            heroAssetList.Add("man1");
            heroAssetList.Add("man1Walk");
            heroAssetList.Add("man2");
            heroAssetList.Add("man2Walk");
            heroAssetList.Add("middleMan");
            heroAssetList.Add("wideMan");
            heroAssetList.Add("narrowMan");
            heroAssetList.Add("jump1Man");
            heroAssetList.Add("jump2Man");
            heroAssetList.Add("jump3Man");

            //Push images for enemy entity to list
            enemyAssetList.Add("enemy1");
            enemyAssetList.Add("enemy1Walk");
            enemyAssetList.Add("enemy2");
            enemyAssetList.Add("enemy2Walk");

            //Push images for background manager
            backgroundAssetList.Add("background//sky");
            backgroundAssetList.Add("background//mountains");
            backgroundAssetList.Add("background//hills");
            backgroundAssetList.Add("background//grass");
            backgroundAssetList.Add("background//platform");
            backgroundAssetList.Add("background//cloudsFull");

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            entityManager.createBackground(backgroundAssetList, 2.0f);
            entityManager.createHero(new Vector2(100, platformHeight-10/*-10 due to bigger character*/),heroAssetList);
            entityManager.createEnemy(new Vector2(900, platformHeight), enemyAssetList);
            entityManager.createEnemy(new Vector2(-200, platformHeight), enemyAssetList); 
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

            //If the player number of lives 0 then quit the game
            if (entityManager.getHero().getNumLives <= 0)
                this.Exit();

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
            
            //EntityManager - Draw all entities
            entityManager.drawAll(spriteBatch);
            
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
