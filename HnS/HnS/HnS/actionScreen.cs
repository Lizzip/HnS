using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace HnS
{
    //This is the "main game" screen, where the action in the game takes place. *All
    //code from the Game1 class can be placed here. *
    class actionScreen : gameScreen
    {
        EntityManager entityManager;
        //Game game;

        //Input states
        KeyboardState currentKB, prevKB;
        MouseState currentMouse, prevMouse;

        //General vars
        int windowHeight = 600, windowWidth = 800;
        int platformHeight = 512;
        List<string> heroAssetList = new List<string>();
        List<string> enemyAssetList = new List<string>();
        List<string> backgroundAssetList = new List<string>();

        public actionScreen(Game theGame, ContentManager theContent, SpriteBatch theSpriteBatch)
            : base(theGame, theSpriteBatch)
        {
            game = theGame;

            //Create the entity manager
            entityManager = new EntityManager(theContent, platformHeight, windowWidth,
                windowHeight);

            //Push images for hero entity to the list
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
            backgroundAssetList.Add("background//trees");
            backgroundAssetList.Add("background//platform");
            backgroundAssetList.Add("background//cloudsFull");

            base.Initialize();
        }

        public void loadContent(ContentManager theContent)
        {
            base.LoadContent();

            entityManager.createBackground(backgroundAssetList, 2.0f);
            entityManager.createHero(new Vector2(100, platformHeight-10/*-10 due to bigger char*/), heroAssetList);
            entityManager.createEnemy(new Vector2(900, platformHeight), enemyAssetList);
            entityManager.createEnemy(new Vector2(-200, platformHeight), enemyAssetList);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Get new mouse and keyboard states
            currentKB = Keyboard.GetState();
            currentMouse = Mouse.GetState();

            //Quit if escape key is pressed
            if (currentKB.IsKeyDown(Keys.Escape)) game.Exit();

            //Entity manager - update all entities
            entityManager.updateAll(gameTime);

            //If the player number of lives is 0. then quit
            if (entityManager.getHero().getNumLives <= 0)
                game.Exit();

            //Set previous mouse and keyboard states
            prevKB = currentKB;
            prevMouse = currentMouse;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            //Set background colour to grey
            GraphicsDevice.Clear(Color.Gray);
            spriteBatch.Begin();

            //EntityManager - draw all entities
            entityManager.drawAll(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

    }
}
