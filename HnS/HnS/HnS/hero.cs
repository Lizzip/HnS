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
        int frameRate = 0, frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;

        //Input states
        KeyboardState currentKB, prevKB;
        MouseState currentMouse, prevMouse;
        GamePadState currentGamePad, prevGamePad; //Controls player 2

        //Fonts
        SpriteFont smallFont;
        SpriteFont deathFont;

        //Textures
        Texture2D healthBarOutline, temp, bloodSplat, heroPanel,
            heartOutlines, heart1Fill, heart2Fill, heart3Fill;

        ////////////////////////////////////////////////
        //Animation
        float speed, scale, velocityY;
        int armAnimWidth = 6, armAnimSpeed = 10;

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
        int deathTimer = 0, attackTimer = 1, bloodTimer = 2;

        //Combat
        Vector2 healthTextPos, numLivesPos, deathTextPos, bloodPos;
        float attackDamage, health;
        int numLives, attackIndex = 4, armourLevel = 1;
        
                
        //General Vars
        //facing 0 = right, 1 = left
        int charHeightOffset = 2, UID;
        bool local, exists; //False by default so we don't draw player 2 unless theyre connected
        Vector2 position, currentPos, prevPos, posDiff;
        
        ///////////////////////////////////////////////////
        // CONSTRUCTORS AND LOADING ///////////////////////
        ///////////////////////////////////////////////////

        public Hero() { }

        public Hero(EntityManager eManager, int uid, Vector2 pos, ContentManager content, bool localPlayer)
        {
            entityManager = eManager;
            UID = uid;

            //Movement and animation
            isJumping = false;
            isAttacking = false;
            flip = false;
            exists = false;
            local = localPlayer;
            velocityY = 0;
            position = pos;
            speed = 0.15f;

            bodyTempCurrentFrame = Vector2.Zero;
            armTempCurrentFrame = Vector2.Zero;
            bodyAnimation = new animation(position, new Vector2(4, 2), 90);
            armAnimation = new animation(position, new Vector2(armAnimWidth, 1), armAnimSpeed);
            
            scale = 0.7f;

            health = 100.0f;
            attackDamage = 10.0f;
            numLives = 3;
            contentManager = content;
            loadContent();
        }

        void loadContent()
        {
            //Animation stuff
            bodyAnimation.AnimationImage = contentManager.Load<Texture2D>("hero\\herospritesheet");
            armAnimation.AnimationImage = contentManager.Load <Texture2D>("hero\\heroarmspritesheet");


            //Load other images and fonts
            healthBarOutline = contentManager.Load<Texture2D>("healthBarOutline");
            smallFont = contentManager.Load<SpriteFont>("smallFont");
            deathFont = contentManager.Load<SpriteFont>("deathFont");
            bloodSplat = contentManager.Load<Texture2D>("bloodSplat");
            heroPanel = contentManager.Load<Texture2D>("panel\\heroPanel");
            heartOutlines = contentManager.Load<Texture2D>("panel\\heroPanelHeartOutlines");
            heart1Fill = contentManager.Load<Texture2D>("panel\\heroPanelHeart1");
            heart2Fill = contentManager.Load<Texture2D>("panel\\heroPanelHeart2");
            heart3Fill = contentManager.Load<Texture2D>("panel\\heroPanelHeart3");


            //Set other variables (adjust default draw height for image height - to draw hero standing on platform)
            position.Y -=bodyAnimation.FrameHeight *scale;
            healthTextPos = new Vector2(25, 17);
            numLivesPos = new Vector2(25, 35);
            deathTextPos = new Vector2((float)entityManager.getScreenWidth() / 3, (float)entityManager.getScreenHeight() / 2);

            //Create countdown timers
            countDownTimers.Add(0.0f);//death timer
            countDownTimers.Add(0.0f);//Attack timer
            countDownTimers.Add(0.0f);//blood timer
        }

        ///////////////////////////////////////////////////
        // ENTITY OVERRIDES ///////////////////////////////
        ///////////////////////////////////////////////////

        public override void update(GameTime theGameTime)
        {
            if (exists)
            {
                //store current position
                prevPos = position;

                //Animation
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

                //Get new mouse, gamepad and keyboard states
                currentKB = Keyboard.GetState();
                currentMouse = Mouse.GetState();
                currentGamePad = GamePad.GetState(PlayerIndex.One);

                position.Y += velocityY;

                //Make the character jump based on spacebar press
                Jump(theGameTime);

                //Attack with left mouseclick
                attack(theGameTime, currentMouse, prevMouse);

                // Death
                if (health <= 0.0)
                {
                    position.X = 25;
                    RemoveLife();
                }


                //Movement and animation (do not move if this is player 2)
                if (local)
                {
                    if (currentKB.IsKeyDown(Keys.D))
                    {
                        MoveRight(theGameTime);
                    }
                    else if (currentKB.IsKeyDown(Keys.A))
                    {
                        MoveLeft(theGameTime);
                    }
                    else
                    {
                        bodyAnimation.Active = false;
                    }
                }
                else
                {
                    if (currentGamePad.IsConnected)
                    {
                        if (currentGamePad.ThumbSticks.Left.X > 0.0f)
                        {
                            MoveRight(theGameTime);
                        }
                        else if (currentGamePad.ThumbSticks.Left.X < 0.0f)
                        {
                            MoveLeft(theGameTime);
                        }
                    }
                }

                //Set previous mouse, gamepad and keyboard states
                prevKB = currentKB;
                prevMouse = currentMouse;
                prevGamePad = currentGamePad;
                currentPos = position;

                posDiff = Vector2.Subtract(prevPos, currentPos);
                
                //If pos has changed, alert server
                if (posDiff != Vector2.Zero && entityManager.getNetworkingEnabled() == true)
                {
                    entityManager.getNetwork().writeStream.Position = 0;
                    entityManager.getNetwork().writer.Write((byte)Protocol.PlayerMoved);
                    entityManager.getNetwork().writer.Write(posDiff.X);
                    entityManager.getNetwork().writer.Write(posDiff.Y);
                    entityManager.getNetwork().SendData(entityManager.getNetwork().GetDataFromMemoryStream(entityManager.getNetwork().writeStream));
                }
            }

            base.update(theGameTime);
        }

        public override void draw(Microsoft.Xna.Framework.Graphics.SpriteBatch theSpriteBatch)
        {
            if (exists)
            {
                //Draw the body and arm animations for the player character
                armAnimation.Draw(theSpriteBatch, scale, flip);
                if (!isJumping)
                {
                    if (IsMovingLeft() || IsMovingRight())
                    {
                        bodyAnimation.Draw(theSpriteBatch, scale, flip);
                    }
                    else
                    {
                        bodyAnimation.forceDraw(theSpriteBatch, scale, flip, 0, 0);
                    }
                }
                else
                {
                    bodyAnimation.forceDraw(theSpriteBatch, scale, flip, 3, 0);
                }

                //Draw hero panel
                theSpriteBatch.Draw(heroPanel, Vector2.Zero, Color.White);

                //Draw red health bar area for current health
                theSpriteBatch.Draw(healthBarOutline, new Rectangle(21, 20, (int)(healthBarOutline.Width * ((double)health / 100) - 2), 18),
                     new Rectangle(0, 45, healthBarOutline.Width, 44), Color.Red);

                //Write health text
                theSpriteBatch.DrawString(smallFont, "Health: " + health + "%", healthTextPos, Color.White);

                //Draw/Fill in each heart for lives
                if (numLives > 0)
                {
                    theSpriteBatch.Draw(heart1Fill, Vector2.Zero, Color.White);

                    if (numLives > 1)
                    {
                        theSpriteBatch.Draw(heart2Fill, Vector2.Zero, Color.White);

                        if (numLives > 2)
                        {
                            theSpriteBatch.Draw(heart3Fill, Vector2.Zero, Color.White);
                        }
                    }
                }

                //Draw heart outlines on hero panel
                theSpriteBatch.Draw(heartOutlines, Vector2.Zero, Color.White);

                //Display death text after dying
                if (countDownTimers[deathTimer] > 0.0f)
                    theSpriteBatch.DrawString(deathFont, "YOU LOSE A LIFE", deathTextPos, Color.Red);

                //Draw FPS
                frameCounter++;
                string fps = string.Format("fps: {0}", frameRate);
                theSpriteBatch.DrawString(smallFont, fps, new Vector2(713, 13), Color.Black);
                theSpriteBatch.DrawString(smallFont, fps, new Vector2(712, 12), Color.White);
            }

            base.draw(theSpriteBatch);
        }

        ///////////////////////////////////////////////////
        // ADDITIONAL MOVEMENT ////////////////////////////
        ///////////////////////////////////////////////////

        private void Jump(GameTime theGameTime)
        {
            //Check if the space is pressed and character is not already jumping. Then move the character
            //in negative Y position (upwards) and set to fall back down based on the Y velocity.
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && !isJumping && local)
            {
                position.Y -= 10.0f;
                velocityY = -3.0f;
                isJumping = true;
            }

            if (!local && !isJumping && currentGamePad.IsConnected)
            {
                if (currentGamePad.Buttons.A == ButtonState.Pressed)
                {
                    position.Y -= 10.0f;
                    velocityY = -3.0f;
                    isJumping = true;
                }
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
                bodyTempCurrentFrame.Y = 1;
            }

            //Once the player Y position reaches the platform
            //the isJumping bool is set to false (can't fall below platform)
            if (position.Y + (bodyAnimation.FrameHeight * scale) >= entityManager.getPlatformHeight() - charHeightOffset && isJumping)
            {
                isJumping = false;

                //Ensure player is set to exact same height after every jump (was varying slightly before due to decrementing by float)
                position.Y = entityManager.getPlatformHeight() - charHeightOffset - (bodyAnimation.FrameHeight * scale);
                
            }
        }

        public bool IsMovingLeft()
        {
            if ((currentKB.IsKeyDown(Keys.A) && local) ||
                (currentGamePad.IsConnected && currentGamePad.ThumbSticks.Left.X < 0.0f && !local))
            {
                return true;
            }
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

            if (position.X > entityManager.getScreenWidth() * 0.15)
                position.X -= speed * theGameTime.ElapsedGameTime.Milliseconds;
        }

        public bool IsMovingRight()
        {
            if ((currentKB.IsKeyDown(Keys.D) && local) ||
                (currentGamePad.IsConnected && currentGamePad.ThumbSticks.Left.X > 0.0f && !local))
            {
                return true;
            }
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
        
        public void setExists(bool ex)
        {
            exists = ex;
        }

        public bool getExists()
        {
            return exists;
        }

        public void setRecievedInfo(Vector2 newPos)
        {
            position = Vector2.Subtract(position, newPos);
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
            if (currentMouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released && !isAttacking && local)
            {
                isAttacking = true;
                attackIndex = 0;
                broadcastAttack();
                countDownTimers[attackTimer] = armAnimSpeed * (armAnimWidth + 1); //There's some jim pokery going here, I dont know why it needs the +1
            }

            if (currentGamePad.IsConnected && !isAttacking && !local && currentGamePad.Buttons.X == ButtonState.Pressed && prevGamePad.Buttons.X == ButtonState.Released)
            {
                isAttacking = true;
                attackIndex = 0;
                broadcastAttack();
                countDownTimers[attackTimer] = armAnimSpeed * (armAnimWidth + 1); //There's some jim pokery going here, I dont know why it needs the +1
            }

            if (isAttacking)
            {
                //Enable the arm/sword animation
                armAnimation.Active = true;

                //Disable attacking when animation has finished playing
                if (countDownTimers[attackTimer] < 0.0f)
                {
                    isAttacking = false;
                }

            }
            else
                armAnimation.Active = false;
            
        }

        public void heal(int amount)
        {
            //Heal by given amount, cap at 100
            health += amount;
            if (health > 100) health = 100;
        }
    }
}
