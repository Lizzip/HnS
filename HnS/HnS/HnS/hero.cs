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
    class Hero : Entity
    {
        //Loaders and Managers
        ContentManager contentManager;
        EntityManager entityManager;

        //Input states
        KeyboardState currentKB, prevKB;
        MouseState currentMouse, prevMouse;

        //Textures and Fonts
        List<Texture2D> images = new List<Texture2D>();
        Texture2D healthBarOutline;
        SpriteFont smallFont;
        SpriteFont deathFont;

        //Timers
        List<float> countDownTimers = new List<float>();
        int walkingTimer = 0, deathTimer = 1;


        //Combat
        Vector2 attackOrigin, healthTextPos, numLivesPos, deathTextPos;
        float attackDamage, health;
        int numLives;
                
        //General Vars
        //facing 0 = right, 1 = left
        int activeImage = 0, facing = 0;
        Vector2 position;
        float speed, scale, velocityY;
        bool isJumping;

        ///////////////////////////////////////////////////
        // CONSTRUCTORS AND LOADING ///////////////////////
        ///////////////////////////////////////////////////

        public Hero() { }

        public Hero(EntityManager eManager, Vector2 pos, ContentManager content, List<string> assets)
        {
            entityManager = eManager;
            isJumping = false;
            velocityY = 0;
            position = pos;
            health = 100.0f;
            speed = 0.15f;
            scale = 0.8f;
            attackDamage = 10.0f;
            numLives = 3;
            contentManager = content;
            loadContent(assets);
        }

        void loadContent(List<string> assets)
        {
            //Load all animation images for hero
            Texture2D temp;
            for (int i = 0, len = assets.Count; i < len; i++)
            {
                temp = contentManager.Load<Texture2D>(assets.ElementAt(i));
                images.Add(temp);
            }

            //Load other images and fonts
            healthBarOutline = contentManager.Load<Texture2D>("healthBarOutline");
            smallFont = contentManager.Load<SpriteFont>("smallFont");
            deathFont = contentManager.Load<SpriteFont>("deathFont");

            //Set other variables (adjust default draw height for image height - to draw hero standing on platform)
            position.Y -= images.ElementAt(0).Height * scale;
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
                if (activeImage == 0) activeImage = 2;
                else activeImage = 0;

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
                    switch (activeImage)
                    {
                        case 0:
                            activeImage = 1;
                            break;
                        case 1:
                            activeImage = 0;
                            break;
                        case 2:
                            activeImage = 3;
                            break;
                        case 3:
                            activeImage = 2;
                            break;
                    }

                    countDownTimers[walkingTimer] = 100.0f;
                }
                
                if(position.X < entityManager.getScreenWidth() * 0.8)
                    position.X += speed * theGameTime.ElapsedGameTime.Milliseconds;
            }

            if (currentKB.IsKeyDown(Keys.A))
            {
                if (countDownTimers[walkingTimer] < 0.0f)
                {
                    switch (activeImage)
                    {
                        case 0:
                            activeImage = 1;
                            break;
                        case 1:
                            activeImage = 0;
                            break;
                        case 2:
                            activeImage = 3;
                            break;
                        case 3:
                            activeImage = 2;
                            break;
                    }

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
            if (facing == 0) theSpriteBatch.Draw(images.ElementAt(activeImage), position, null,
                    Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            else theSpriteBatch.Draw(images.ElementAt(activeImage), position, null,
                    Color.White, 0, Vector2.Zero, scale, SpriteEffects.FlipHorizontally, 0);

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
                velocityY = -5.0f;
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
            if (position.Y + (images.ElementAt(activeImage).Height * scale) >= entityManager.getPlatformHeight() && isJumping)
            {
                isJumping = false;

                //Ensure player is set to exact same height after every jump (was varying slightly before due to decrementing by float)
                position.Y = entityManager.getPlatformHeight() -5 - (images.ElementAt(activeImage).Height * scale);
                
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
