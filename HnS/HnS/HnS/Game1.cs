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
        public static Debugger debugger;
        
        //Input states
        KeyboardState currentKB, prevKB;
        MouseState currentMouse, prevMouse;

        //General vars
        int windowHeight = 600, windowWidth = 800;
        int platformHeight = 512;
        List<string> enemyAssetList = new List<string>();
        List<string> backgroundAssetList = new List<string>();

          
        
        //////////////////////////////////////////////
        //GAME SCREEN STUFF - IGNORE FOR NOW ! :P
         
        //Game screens
        gameScreen activeScreen;
        startScreen startScreen;
        actionScreen actionScreen;
        popupScreen quitScreen;
        popupScreen pauseScreen;

        //Pause screen states
        bool isPaused = false;

        //Screen text string lists
        string[] startItems = { "Start Game", "End Game" };
        string[] quitItems = { "Yes", "No" };
        string[] pauseItems = { "PAUSED" };

        // END GAME SCREEN STUFF
        ////////////////////////


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
            //Create the debugger
            debugger = new Debugger(Content);

            //Create entity manager
            entityManager = new EntityManager(Content, debugger, platformHeight, windowWidth, windowHeight);
            /*
            //Push images for enemy entity to list
            enemyAssetList.Add("enemy1");
            enemyAssetList.Add("enemy1Walk");
            enemyAssetList.Add("enemy2");
            enemyAssetList.Add("enemy2Walk");
            */
            //Push images for background manager
            backgroundAssetList.Add("background//sky");
            backgroundAssetList.Add("background//mountains");
            backgroundAssetList.Add("background//hills");
            backgroundAssetList.Add("background//trees");
            backgroundAssetList.Add("background//platform");
            backgroundAssetList.Add("background//cloudsFull");

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            entityManager.createBackground(backgroundAssetList, 2.0f);
            entityManager.createHero(new Vector2(100, platformHeight));
            entityManager.createEnemy(new Vector2(900, platformHeight));
            entityManager.createEnemy(new Vector2(-200, platformHeight));
         //   entityManager.createPotion(Color.Yellow, new Vector2(250, platformHeight));
         //   entityManager.createPotion(Color.Red, new Vector2(590, platformHeight));

            /////////////////////////////////////////////
            // GAME SCREEN INITIALISATION - IGNORE FOR NOW//
            ////////////////////////////////////////////////
            //startScreen = new startScreen(this, spriteBatch, Content.Load<SpriteFont>("menufont"),
            //    Content.Load<Texture2D>("screens\\startscreen"), startItems);
            //Components.Add(startScreen);
            //startScreen.Hide();

            //actionScreen = new actionScreen(this, Content, spriteBatch);
            //actionScreen.loadContent(Content);
            //Components.Add(actionScreen);
            //actionScreen.Hide();

            //quitScreen = new popupScreen(this, spriteBatch, Content.Load<SpriteFont>("menufont"),
            //    Content.Load<Texture2D>("screens\\quitscreen"), quitItems);
            //Components.Add(quitScreen);
            //quitScreen.Hide();

            //pauseScreen = new popupScreen(this, spriteBatch, Content.Load<SpriteFont>("pausefont"),
            //    Content.Load<Texture2D>("screens\\flame2"), pauseItems);
            //Components.Add(pauseScreen);
            //pauseScreen.Hide();

            //activeScreen = startScreen;
            //activeScreen.Show();

            // END GAME SCREEN 
            //////////////////////////////////////
        }

        protected override void UnloadContent(){}

        protected override void Update(GameTime gameTime)
        {
            //Get new mouse and keyboard states
            currentKB = Keyboard.GetState();
            currentMouse = Mouse.GetState();

            //Quit game on Esc key press
            if (CheckKey(Keys.Escape)) this.Exit();

            //EntityManager - Update all entities
            entityManager.updateAll(gameTime);

            //If the player number of lives 0 then quit the game
            if (entityManager.getHero().getNumLives <= 0)
                this.Exit();


            ///////////////////////////////////////////////
            // GAME SCREEN STUFF - IGNORE FOR NOW ///////
            //////////////////////////////////////////////
            //if (activeScreen == startScreen)
            //    HandleStartScreen();
            //else if (activeScreen == actionScreen)
            //    HandleActionScreen(gameTime);
            //else if (activeScreen == quitScreen)
            //    HandleQuitScreen();
            //else if (activeScreen == pauseScreen)
            //    HandlePauseScreen();
            // END GAME SCREEN //
            /////////////////////////////////


            base.Update(gameTime);

            //Set previous mouse and keyboard states
            prevKB = currentKB;
            prevMouse = currentMouse;
            
            
        }

        private void HandleStartScreen()
        {
            if (CheckKey(Keys.Enter))
            {
                if (startScreen.SelectedIndex == 0)
                {
                    activeScreen.Hide();
                    activeScreen = actionScreen;
                    activeScreen.Show();
                }
                if (startScreen.SelectedIndex == 1)
                {
                    this.Exit();
                }
            }
        }

        private void HandleActionScreen(GameTime gameTime)
        {
            actionScreen.Update(gameTime);

            if (CheckKey(Keys.Escape))
            {
                activeScreen.Enabled = false;
                activeScreen = quitScreen;
                activeScreen.Show();
            }

            if (CheckKey(Keys.P) && !isPaused)
            {
                isPaused = true;
                activeScreen.Enabled = false;
                activeScreen = pauseScreen;
                activeScreen.Show();
            }


        }

        private void HandleQuitScreen()
        {
            if (CheckKey(Keys.Enter))
            {
                if (quitScreen.SelectedIndex == 0)
                {
                    activeScreen.Hide();
                    actionScreen.Hide();
                    activeScreen = startScreen;
                    activeScreen.Show();
                    
                }

                if (quitScreen.SelectedIndex == 1)
                {
                    activeScreen.Hide();
                    activeScreen = actionScreen;
                    activeScreen.Show();
                }
            }
        }

        private void HandlePauseScreen()
        {
            if (isPaused && CheckKey(Keys.P))
            {
                isPaused = false;
                activeScreen.Hide();
                activeScreen = actionScreen;
                activeScreen.Show();
            }
        }

        private bool CheckKey(Keys key)
        {
            return currentKB.IsKeyUp(key) && prevKB.IsKeyDown(key);
        }
        

        protected override void Draw(GameTime gameTime)
        {
            //Set background colour as gray
            GraphicsDevice.Clear(Color.Gray);
            spriteBatch.Begin();
            
            /////////////////////////////////////
            // COMMENT THIS SECTION OUT TO TEST GAME SCREENS

            //EntityManager - Draw all entities
            entityManager.drawAll(spriteBatch);
            debugger.Output(spriteBatch);

            // END OF COMMENT OUT SECTION
            /////////////////////////////////

            base.Draw(gameTime);
            spriteBatch.End();
            
        }
    }
}
