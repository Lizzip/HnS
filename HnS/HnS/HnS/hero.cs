﻿using System;
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
    class Hero : Entity
    {
        //Loaders and Managers
        ContentManager contentManager;
        EntityManager entityManager;

        //Input states
        KeyboardState currentKB, prevKB;
        MouseState currentMouse, prevMouse;

        //Fonts
        SpriteFont smallFont;
        SpriteFont deathFont;

        //Textures
        List<Texture2D> legImages = new List<Texture2D>();
        List<Texture2D> topImages = new List<Texture2D>();
        Texture2D healthBarOutline, temp;

        //Animation
        int activeLegImage = 0, activeTopImage = 0, 
            walkingIndex = 0, jumpingImage = 4, stationaryLegImage = 3;
        bool isJumping;
        int[] walkingPattern;
        float speed, scale, velocityY;

        //Timers
        List<float> countDownTimers = new List<float>();
        int walkingTimer = 0, deathTimer = 1;

        //Combat
        Vector2 healthTextPos, numLivesPos, deathTextPos;
        float attackDamage, health;
        int numLives;
                
        //General Vars
        //facing 0 = right, 1 = left
        int facing = 0, charHeightOffset = 2;
        Vector2 position;
        SpriteEffects spriteEffects;
        

        ///////////////////////////////////////////////////
        // CONSTRUCTORS AND LOADING ///////////////////////
        ///////////////////////////////////////////////////

        public Hero() { }

        public Hero(EntityManager eManager, Vector2 pos, ContentManager content, List<string> legAssets, List<string> topAssets)
        {
            entityManager = eManager;
            isJumping = false;
            velocityY = 0;
            position = pos;
            health = 100.0f;
            speed = 0.15f;
            scale = 0.7f;
            attackDamage = 10.0f;
            numLives = 3;
            contentManager = content;
            walkingPattern = new int[4] { 1, 2, 1, 0 };
            loadContent(legAssets, topAssets);
        }

        void loadContent(List<string> legAssets, List<string> topAssets)
        {
            //Load all leg images
            for (int i = 0, len = legAssets.Count; i < len; i++)
            {
                temp = contentManager.Load<Texture2D>(legAssets.ElementAt(i));
                legImages.Add(temp);
            }

            //Load all top images
            for (int i = 0, len = topAssets.Count; i < len; i++)
            {
                temp = contentManager.Load<Texture2D>(topAssets.ElementAt(i));
                topImages.Add(temp);
            }

            //Load other images and fonts
            healthBarOutline = contentManager.Load<Texture2D>("healthBarOutline");
            smallFont = contentManager.Load<SpriteFont>("smallFont");
            deathFont = contentManager.Load<SpriteFont>("deathFont");

            //Set other variables (adjust default draw height for image height - to draw hero standing on platform)
            position.Y -= topImages.ElementAt(0).Height *scale;
            healthTextPos = new Vector2(25, 17);
            numLivesPos = new Vector2(25, 35);
            deathTextPos = new Vector2((float)entityManager.getScreenWidth() / 3, (float)entityManager.getScreenHeight() / 2);

            //Create countdown timers
            countDownTimers.Add(250.0f);//walking timer
            countDownTimers.Add(0.0f);//death timer
        }


        ///////////////////////////////////////////////////
        // ENTITY OVERRIDES ///////////////////////////////
        ///////////////////////////////////////////////////

        public override void update(Microsoft.Xna.Framework.GameTime theGameTime)
        {
            //Count down all count down timers in progress
            for (int i = 0, len = countDownTimers.Count; i < len; i++)
            {
                if (countDownTimers[i] > 0.0f)
                {
                    countDownTimers[i] -= (float)theGameTime.ElapsedGameTime.Milliseconds;
                }
            }
            
            //Get new mouse and keyboard states
            currentKB = Keyboard.GetState();
            currentMouse = Mouse.GetState();

            position.Y += velocityY;

            //Make the character jump based on spacebar press
            Jump(theGameTime);

            //Attack with left mouseclick
            if (currentMouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
            {
                if (activeLegImage == 0) activeLegImage = 2;
                else activeLegImage = 0;

                broadcastAttack();
            }

            ////////////////////////////////
            // Death testing ///////////////
            ////////////////////////////////
            if (Keyboard.GetState().IsKeyDown(Keys.J))
                health--;

            // Death
            if (health <= 0.0)
            {
                position.X = 25;
                RemoveLife();
            }


            //Switch directions
            if (MoveLeft()) facing = 1;
            if (MoveRight()) facing = 0;

            //Walking movement
            if (currentKB.IsKeyDown(Keys.D))
            {
                if (countDownTimers[walkingTimer] < 0.0f)
                {
                    if (walkingIndex < 3) walkingIndex++;
                    else walkingIndex = 0;
                    countDownTimers[walkingTimer] = 100.0f;
                }
                
                if(position.X < entityManager.getScreenWidth() * 0.8)
                    position.X += speed * theGameTime.ElapsedGameTime.Milliseconds;
            }
            else if (currentKB.IsKeyDown(Keys.A))
            {
                if (countDownTimers[walkingTimer] < 0.0f)
                {
                    if (walkingIndex < 3) walkingIndex++;
                    else walkingIndex = 0;

                    countDownTimers[walkingTimer] = 100.0f;
                }
                
                if(position.X > entityManager.getScreenWidth() * 0.15)
                    position.X -= speed * theGameTime.ElapsedGameTime.Milliseconds;
            }

            //Set previous mouse and keyboard states
            prevKB = currentKB;
            prevMouse = currentMouse;
            base.update(theGameTime);
        }

        public override void draw(Microsoft.Xna.Framework.Graphics.SpriteBatch theSpriteBatch)
        {
            //If facing right (0) draw normally, if facing left (1) flip sprite horizontally
            if(facing == 0)spriteEffects = SpriteEffects.None;
            else spriteEffects = SpriteEffects.FlipHorizontally;

            //Draw leg image
            if (isJumping)
            {
                //Draw jumping leg image
                theSpriteBatch.Draw(legImages.ElementAt(jumpingImage), position, null,
                    Color.White, 0, Vector2.Zero, scale, spriteEffects, 0);
            }
            else
            {
                if (currentKB.IsKeyDown(Keys.A) == false && currentKB.IsKeyDown(Keys.D) == false)
                {
                    //Draw stationary leg image
                    theSpriteBatch.Draw(legImages.ElementAt(stationaryLegImage), position, null,
                    Color.White, 0, Vector2.Zero, scale, spriteEffects, 0);
                }
                else
                {
                    //Draw active walking leg image
                    theSpriteBatch.Draw(legImages.ElementAt(walkingPattern[walkingIndex]), position, null,
                        Color.White, 0, Vector2.Zero, scale, spriteEffects, 0);
                }
            }

            //Draw top image
            theSpriteBatch.Draw(topImages.ElementAt(activeTopImage), position, null,
                    Color.White, 0, Vector2.Zero, scale, spriteEffects, 0);

            //Draw white health bar outline
            theSpriteBatch.Draw(healthBarOutline, new Vector2(20, 19), Color.White);

            //Draw grey health bar area for health lost
            theSpriteBatch.Draw(healthBarOutline, new Rectangle(21, 20, healthBarOutline.Width - 2, 18),
                new Rectangle(0, 45, healthBarOutline.Width, 44), Color.Gray);

            //Draw red health bar area for current health
            theSpriteBatch.Draw(healthBarOutline, new Rectangle(21, 20, (int)(healthBarOutline.Width * ((double)health / 100) - 2), 18),
                 new Rectangle(0, 45, healthBarOutline.Width, 44), Color.Red);

            //Write health text
            theSpriteBatch.DrawString(smallFont, "Health: " + health + "%", healthTextPos, Color.White);

            //Write number of lives
            theSpriteBatch.DrawString(smallFont, "Lives: " + numLives, numLivesPos, Color.Black);
            
            //Display death text after dying
            if (countDownTimers[deathTimer] > 0.0f)
                theSpriteBatch.DrawString(deathFont, "YOU LOSE A LIFE", deathTextPos, Color.Red);

            base.draw(theSpriteBatch);
        }

        ///////////////////////////////////////////////////
        // ADDITIONAL MOVEMENT ////////////////////////////
        ///////////////////////////////////////////////////

        private void Jump(GameTime theGameTime)
        {
            //Check if the space is pressed and character is not already jumping. Then move the character
            //in negative Y position (upwards) and set to fall back down based on the Y velocity.
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && !isJumping)
            {
                position.Y -= 10.0f;
                velocityY = -3.0f;
                isJumping = true;
            }

            //Make the character fall based on in increasing Y velocity
            if (isJumping)
            {
                float i = 0.15f;
                velocityY += i;
            }
            else velocityY = 0;

            //Once the player Y position reaches the platform
            //the isJumping bool is set to false (can't fall below platform)
            if (position.Y + (legImages.ElementAt(activeLegImage).Height * scale) >= entityManager.getPlatformHeight()-charHeightOffset && isJumping)
            {
                isJumping = false;

                //Ensure player is set to exact same height after every jump (was varying slightly before due to decrementing by float)
                position.Y = entityManager.getPlatformHeight() - charHeightOffset - (legImages.ElementAt(activeLegImage).Height * scale);
                
            }
        }

        public bool MoveLeft()
        {
            if (currentKB.IsKeyDown(Keys.A)) return true;
            else return false;
        }

        public bool MoveRight()
        {
            if (currentKB.IsKeyDown(Keys.D)) return true;
            else return false;
        }

        ///////////////////////////////////////////////////
        // DEATH //////////////////////////////////////////
        ///////////////////////////////////////////////////
        public void RemoveLife()
        {
            numLives--;
            health = 100.0f;
            countDownTimers[deathTimer] = 1500.0f;
        }

        ///////////////////////////////////////////////////
        // GETTERS AND SETTERS ////////////////////////////
        ///////////////////////////////////////////////////

        public override Vector2 getPos()
        {
            return position;
        }

        public int getNumLives
        {
            get { return numLives; }
        }

        ///////////////////////////////////////////////////
        // COMBAT /////////////////////////////////////////
        ///////////////////////////////////////////////////

        void broadcastAttack()
        {
            entityManager.broadcastAttack(attackDamage, position);
        }
    }
}
