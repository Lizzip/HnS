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

        //FPS counter
        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;

        //Input states
        KeyboardState currentKB, prevKB;
        MouseState currentMouse, prevMouse;

        //Fonts
        SpriteFont smallFont;
        SpriteFont deathFont;

        //Textures
        List<Texture2D> legImages = new List<Texture2D>();
        List<Texture2D> topImages = new List<Texture2D>();
        Texture2D healthBarOutline, temp, bloodSplat;

        ////////////////////////////////////////////////
        //Animation
        int activeTopImage = 0, stationaryTopImage = 0,
            walkingIndex = 0, jumpingImage = 4, stationaryLegImage = 3;
        //bool isJumping, isAttacking;
        int[] walkingPattern, attackPattern;
        float speed, scale, velocityY;

        //Animation
        animation bodyAnimation, armAnimation;
        public animation GetBodyAnimation
        {
            get { return bodyAnimation; }
        }

        public animation GetArmAnimation
        {
            get { return armAnimation; }
        }

        Vector2 bodyTempCurrentFrame, armTempCurrentFrame;
        bool isJumping, isAttacking, flip;
        //////////////////////////////////////////////
        

        //Timers
        List<float> countDownTimers = new List<float>();
        int walkingTimer = 0, deathTimer = 1, attackTimer = 2, bloodTimer = 3;

        //Combat
        Vector2 healthTextPos, numLivesPos, deathTextPos, bloodPos;
        float attackDamage, health;
        int numLives, attackIndex = 4, armourLevel = 1;
        
                
        //General Vars
        //facing 0 = right, 1 = left
        int facing = 0, charHeightOffset = 2, UID;
        Vector2 position;
        SpriteEffects spriteEffects;
        
        ///////////////////////////////////////////////////
        // CONSTRUCTORS AND LOADING ///////////////////////
        ///////////////////////////////////////////////////

        public Hero() { }

        public Hero(EntityManager eManager, int uid, Vector2 pos, ContentManager content, List<string> legAssets, List<string> topAssets)
        {
            entityManager = eManager;
            UID = uid;

            //Movement and animation
            isJumping = false;
            isAttacking = false;
            flip = false;
            velocityY = 0;
            position = pos;
            speed = 0.15f;

            bodyTempCurrentFrame = Vector2.Zero;
            armTempCurrentFrame = Vector2.Zero;
            bodyAnimation = new animation(position, new Vector2(4, 2), 90);
            armAnimation = new animation(position, new Vector2(6, 1), 40);
            


            scale = 0.7f;

            health = 100.0f;
            attackDamage = 10.0f;
            numLives = 3;
            contentManager = content;
            walkingPattern = new int[4] { 1, 2, 1, 0 };
            attackPattern = new int[3] { 1, 2, 3 };
            loadContent(legAssets, topAssets);
        }

        void loadContent(List<string> legAssets, List<string> topAssets)
        {
            //Animation stuff
            bodyAnimation.AnimationImage = contentManager.Load<Texture2D>("hero\\herospritesheet");
            armAnimation.AnimationImage = contentManager.Load <Texture2D>("hero\\heroarmspritesheet");



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
            bloodSplat = contentManager.Load<Texture2D>("bloodSplat");

            //Set other variables (adjust default draw height for image height - to draw hero standing on platform)
            position.Y -= topImages.ElementAt(0).Height *scale;
            healthTextPos = new Vector2(25, 17);
            numLivesPos = new Vector2(25, 35);
            deathTextPos = new Vector2((float)entityManager.getScreenWidth() / 3, (float)entityManager.getScreenHeight() / 2);

            //Create countdown timers
            countDownTimers.Add(250.0f);//walking timer
            countDownTimers.Add(0.0f);//death timer
            countDownTimers.Add(0.0f);//Attack timer
            countDownTimers.Add(0.0f);//blood timer
        }

        ///////////////////////////////////////////////////
        // ENTITY OVERRIDES ///////////////////////////////
        ///////////////////////////////////////////////////

        public override void update(GameTime theGameTime)
        {
            //Animation
            //bodyAnimation.Active = true;
            //armAnimation.Active = true;

            bodyTempCurrentFrame.X = bodyAnimation.CurrentFrame.X;
            bodyAnimation.Position = position;
            bodyAnimation.CurrentFrame = bodyTempCurrentFrame;
            bodyAnimation.Update(theGameTime);

            armTempCurrentFrame.X = armAnimation.CurrentFrame.X;
            armAnimation.Position = position;
            armAnimation.CurrentFrame = armTempCurrentFrame;
            armAnimation.Update(theGameTime);

            //Update FPS
            elapsedTime += theGameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }

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
            attack(theGameTime, currentMouse, prevMouse);

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


            //Movement and animation
            if (currentKB.IsKeyDown(Keys.D))
            {
                facing = 0;
                MoveRight(theGameTime);
            }

            else if (currentKB.IsKeyDown(Keys.A))
            {
                facing = 1;
                MoveLeft(theGameTime);
            }
            else
            {
                bodyTempCurrentFrame.X = 0;
                bodyTempCurrentFrame.Y = 0;
                bodyAnimation.Active = false;
                armAnimation.Active = false;
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

            bodyAnimation.Draw(theSpriteBatch, scale, flip);
            armAnimation.Draw(theSpriteBatch, scale, flip);

            ////Draw leg image
            //if (isJumping)
            //{
            //    //Draw jumping leg image
            //    theSpriteBatch.Draw(legImages.ElementAt(jumpingImage), position, null,
            //        Color.White, 0, Vector2.Zero, scale, spriteEffects, 0);
            //}
            //else
            //{
            //    if (currentKB.IsKeyDown(Keys.A) == false && currentKB.IsKeyDown(Keys.D) == false)
            //    {
            //        //Draw stationary leg image
            //        theSpriteBatch.Draw(legImages.ElementAt(stationaryLegImage), position, null,
            //        Color.White, 0, Vector2.Zero, scale, spriteEffects, 0);
            //    }
            //    else
            //    {
            //        //Draw active walking leg image
            //        theSpriteBatch.Draw(legImages.ElementAt(walkingPattern[walkingIndex]), position, null,
            //            Color.White, 0, Vector2.Zero, scale, spriteEffects, 0);
            //    }
            //}

            ////Draw top image
            //if (attackIndex < 3)
            //{
            //    //Draw current active attack image
            //    theSpriteBatch.Draw(topImages.ElementAt(attackPattern[attackIndex]), position, null,
            //            Color.White, 0, Vector2.Zero, scale, spriteEffects, 0);
            //}
            //else
            //{
            //    //Draw default waist high sword image
            //    theSpriteBatch.Draw(topImages.ElementAt(stationaryTopImage), position, null,
            //            Color.White, 0, Vector2.Zero, scale, spriteEffects, 0);
            //}

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

            //Draw FPS
            frameCounter++;
            string fps = string.Format("fps: {0}", frameRate);
            theSpriteBatch.DrawString(smallFont, fps, new Vector2(713, 13), Color.Black);
            theSpriteBatch.DrawString(smallFont, fps, new Vector2(712, 12), Color.White);

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
                //Set the Y frame to the first line of the spritesheet
                bodyTempCurrentFrame.Y = 0;
            }
            else
            {
                velocityY = 0;
                //Set the Y frame to the second line of the spritesheet
                //bodyTempCurrentFrame.Y = 1;
            }

            //Once the player Y position reaches the platform
            //the isJumping bool is set to false (can't fall below platform)
            if (position.Y + (legImages.ElementAt(jumpingImage).Height * scale) >= entityManager.getPlatformHeight() - charHeightOffset && isJumping)
            {
                isJumping = false;

                //Ensure player is set to exact same height after every jump (was varying slightly before due to decrementing by float)
                position.Y = entityManager.getPlatformHeight() - charHeightOffset - (legImages.ElementAt(jumpingImage).Height * scale);
                
            }
        }

        public bool IsMovingLeft()
        {
            if (currentKB.IsKeyDown(Keys.A))
                return true;
            else return false;
        }

        public void MoveLeft(GameTime theGameTime)
        {
            //Set the Y frame to the second line of the body spritesheet if not jumping
            //and set the animation to active.
            if(!isJumping)
                bodyTempCurrentFrame.Y = 1;
            bodyAnimation.Active = true;
            //Flip the animation if moving left. This is passed into the animation draw
            //method.
            flip = true;

            if (countDownTimers[walkingTimer] < 0.0f)
            {
                if (walkingIndex < 3) walkingIndex++;
                else walkingIndex = 0;

                countDownTimers[walkingTimer] = 100.0f;
            }

            if (position.X > entityManager.getScreenWidth() * 0.15)
                position.X -= speed * theGameTime.ElapsedGameTime.Milliseconds;
        }

        public bool IsMovingRight()
        {
            if (currentKB.IsKeyDown(Keys.D))
                return true;
            else return false;
        }

        public void MoveRight(GameTime theGameTime)
        {
            //Set the Y frame to the second line of the body spritesheet if not jumping
            //and set the animation to active.
            if (!isJumping)
                bodyTempCurrentFrame.Y = 1;
            bodyAnimation.Active = true;
            //Don't flip the animation if moving right. This is passed into the animation draw
            //method.
            flip = false;

            if (countDownTimers[walkingTimer] < 0.0f)
            {
                if (walkingIndex < 3) walkingIndex++;
                else walkingIndex = 0;
                countDownTimers[walkingTimer] = 100.0f;
            }

            if (position.X < entityManager.getScreenWidth() * 0.8)
                position.X += speed * theGameTime.ElapsedGameTime.Milliseconds;
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
            entityManager.broadcastAttackHero(attackDamage, position);
        }

        public override void beHit(float damage, Vector2 pointOfImpact)
        {
            health -= damage * armourLevel;
            splurgeBlood(pointOfImpact);
        }

        void splurgeBlood(Vector2 pointOfImpact)
        {
            countDownTimers[bloodTimer] = 1000.0f;
            bloodPos = pointOfImpact;
        }

        private void attack(GameTime theGameTime, MouseState currentMS, MouseState prevMS)
        {
            //Check for left mouse click - Attack if not currently attacking
            if (currentMouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released && !isAttacking)
            {
                isAttacking = true;
                attackIndex = 0;
                broadcastAttack();
                countDownTimers[attackTimer] = 35.0f;
                entityManager.getDebugger().Out("Attack", theGameTime.TotalGameTime); //Debugger test for attacking
            }

            if (isAttacking)
            {
                //Enable the arm/sword animation
                armAnimation.Active = true;

                if (countDownTimers[attackTimer] < 0.0f)
                {
                    if (attackIndex < 4)
                    {
                        attackIndex++;
                        countDownTimers[attackTimer] = 30.0f;
                    }
                    else if (attackIndex == 4)
                    {
                        isAttacking = false;
                    }
                }
            }
            else
                armAnimation.Active = false;
            
        }
    }
}
