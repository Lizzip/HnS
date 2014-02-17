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
    public class Hero : Entity
    {
        #region Variables
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
        Texture2D healthBarOutline, temp, bloodSplat, heroPanel, player2Indicator,
            heartOutlines, heart1Fill, heart2Fill, heart3Fill, staminaBarOutline;

        ////////////////////////////////////////////////
        //Animation
        float speed, scale, velocityY;
        int armAnimWidth = 12, armAnimSpeed = 28;

        //Animation
        animation bodyAnimation, armAnimation, bloodAnimation;
        public animation GetBodyAnimation
        {
            get { return bodyAnimation; }
        }

        public animation GetArmAnimation
        {
            get { return armAnimation; }
        }

        public animation GetBloodAnimation
        {
            get { return bloodAnimation; }
        }

        Vector2 bodyTempCurrentFrame, armTempCurrentFrame, bloodTempCurrentFrame;
        bool isJumping, isAttacking, flip;
        //////////////////////////////////////////////
        

        //Timers
        List<float> countDownTimers = new List<float>();
        int deathTimer = 0, attackTimer = 1, bloodTimer = 2;

        //Combat
        Vector2 healthTextPos, numLivesPos, deathTextPos, bloodPos;
        float attackDamage, health, stamina;
        int numLives, attackIndex = 4, armourLevel = 1;
        
                
        //General Vars
        //facing 0 = right, 1 = left
        int charHeightOffset = 2, UID;
        bool local, exists; //False by default so we don't draw player 2 unless theyre connected
        Vector2 position, currentPos, prevPos, posDiff;
        #endregion

        #region Constructors and Loading
        public Hero() { }

        public Hero(int uid, Vector2 pos, ContentManager content, bool localPlayer)
        {
            entityManager = Game1.entityManager;
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
            bloodTempCurrentFrame = Vector2.Zero;
            bodyAnimation = new animation(position, new Vector2(4, 2), 90);
            armAnimation = new animation(position, new Vector2(armAnimWidth, 1), armAnimSpeed);
            bloodAnimation = new animation(position, new Vector2(6, 1), 60);

            scale = 0.7f;

            health = 100.0f;
            stamina = 100.0f;
            attackDamage = 10.0f;
            numLives = 3;
            contentManager = content;
            loadContent();
        }

        void loadContent()
        {
            //Animation stuff
            bodyAnimation.AnimationImage = contentManager.Load<Texture2D>("hero\\herospritesheet");
            armAnimation.AnimationImage = contentManager.Load <Texture2D>("hero\\heroarmspritesheet2");
            bloodAnimation.AnimationImage = contentManager.Load<Texture2D>("bloodanim");

            //Load other images and fonts
            healthBarOutline = contentManager.Load<Texture2D>("healthBarOutline");
            staminaBarOutline = contentManager.Load<Texture2D>("staminaBarOutline");
            smallFont = contentManager.Load<SpriteFont>("smallFont");
            deathFont = contentManager.Load<SpriteFont>("deathFont");
            bloodSplat = contentManager.Load<Texture2D>("bloodSplat");
            heroPanel = contentManager.Load<Texture2D>("panel\\heroPanel");
            heartOutlines = contentManager.Load<Texture2D>("panel\\heroPanelHeartOutlines");
            heart1Fill = contentManager.Load<Texture2D>("panel\\heroPanelHeart1");
            heart2Fill = contentManager.Load<Texture2D>("panel\\heroPanelHeart2");
            heart3Fill = contentManager.Load<Texture2D>("panel\\heroPanelHeart3");

            //Only load texture if this is player 2
            if (!local)
                player2Indicator = contentManager.Load<Texture2D>("hero\\secondPlayerIndicator");


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
        #endregion

        #region Entity Overrides (Update/Draw)
        public override void update(GameTime theGameTime)
        {
            if (exists)
            {
                //store current position
                prevPos = position;

                //Update the hero animations
                AnimateHero(theGameTime);

                //Make the hero charge (currently makes him ice skate :P)
                if(IsCharging()) Charge(theGameTime);

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
                        countDownTimers[i] -= (float)theGameTime.ElapsedGameTime.Milliseconds;   
                }

                //Get new mouse, gamepad and keyboard states
                currentKB = Keyboard.GetState();
                currentMouse = Mouse.GetState();
                currentGamePad = GamePad.GetState(PlayerIndex.One);

                position.Y += velocityY;

                //Make the character jump based on spacebar press
                Jump(theGameTime);

                //Attack with left mouseclick
                attack(theGameTime);

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
                        MoveRight(theGameTime);
                    else if (currentKB.IsKeyDown(Keys.A))
                        MoveLeft(theGameTime);
                    else bodyAnimation.Active = false;
                }
                else
                {
                    if (currentGamePad.IsConnected)
                    {
                        if (currentGamePad.ThumbSticks.Left.X > 0.0f)
                            MoveRight(theGameTime);
                        else if (currentGamePad.ThumbSticks.Left.X < 0.0f)
                            MoveLeft(theGameTime);
                        else bodyAnimation.Active = false;
                    }

                    if (currentKB.IsKeyDown(Keys.Right))
                        MoveRight(theGameTime);
                    else if (currentKB.IsKeyDown(Keys.Left))
                        MoveLeft(theGameTime);
                    else bodyAnimation.Active = false;
                }

                //Set previous mouse, gamepad and keyboard states
                prevKB = currentKB;
                prevMouse = currentMouse;
                prevGamePad = currentGamePad;
                currentPos = position;

                posDiff = Vector2.Subtract(prevPos, currentPos);
                
                //If pos has changed, alert server
                if (posDiff != Vector2.Zero && entityManager.getNetworkingEnabled() == true)
                    sendPosition(posDiff);

                //Send animation update for player 2
                sendAnimationState();
            }

            

            base.update(theGameTime);
        }

        public override void draw(Microsoft.Xna.Framework.Graphics.SpriteBatch theSpriteBatch)
        {
            if (exists)
            {
                //Draw blood if the hero has been recently hit
                if (bloodAnimation.Active)
                    bloodAnimation.Draw(theSpriteBatch, scale, flip);

                //Draw the body and arm animations for the player character
                armAnimation.Draw(theSpriteBatch, scale, flip);
                if (!isJumping)
                {
                    if (IsMovingLeft() || IsMovingRight() || IsCharging())
                        bodyAnimation.Draw(theSpriteBatch, scale, flip);
                    else
                        bodyAnimation.forceDraw(theSpriteBatch, scale, flip, 0, 0);
                }
                else
                    bodyAnimation.forceDraw(theSpriteBatch, scale, flip, 3, 0);

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
                            theSpriteBatch.Draw(heart3Fill, Vector2.Zero, Color.White);
                    }
                }

                //Draw heart outlines on hero panel
                theSpriteBatch.Draw(heartOutlines, Vector2.Zero, Color.White);

                //Draw yellow stamina bar area for current stamina
                theSpriteBatch.Draw(staminaBarOutline, new Rectangle(180, 56, (int)(staminaBarOutline.Width * ((double)stamina / 100) - 2), 18),
                    new Rectangle(0, 44, staminaBarOutline.Width, 44), Color.Yellow);

                //Display death text after dying
                if (countDownTimers[deathTimer] > 0.0f)
                    theSpriteBatch.DrawString(deathFont, "YOU LOSE A LIFE", deathTextPos, Color.Red);

                //Draw FPS
                frameCounter++;
                string fps = string.Format("fps: {0}", frameRate);
                theSpriteBatch.DrawString(smallFont, fps, new Vector2(713, 13), Color.Black);
                theSpriteBatch.DrawString(smallFont, fps, new Vector2(712, 12), Color.White);

                //If player 2, show indicator
                if (!local)
                    theSpriteBatch.Draw(player2Indicator, new Vector2(position.X, position.Y - player2Indicator.Height), null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            }

            base.draw(theSpriteBatch);
        }
        #endregion

        #region Additional Movement
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
            if (currentKB.IsKeyDown(Keys.A) && local) return true;
            if (currentGamePad.IsConnected && currentGamePad.ThumbSticks.Left.X < 0.0f && !local) return true;
            if (currentKB.IsKeyDown(Keys.Left) && !local) return true;

            return false;
        }

        public void MoveLeft(GameTime theGameTime)
        {
            speed = Math.Abs(speed) * -1;
            //Set the Y frame to the second line of the body spritesheet if not jumping
            //and set the animation to active.
            if(!isJumping) bodyTempCurrentFrame.Y = 1;
            bodyAnimation.Active = true;
            //Flip the animation if moving left. This is passed into the animation draw
            //method.
            flip = true;

            if (position.X > entityManager.getScreenWidth() * 0.15)
                position.X += speed * theGameTime.ElapsedGameTime.Milliseconds;
        }

        public bool IsMovingRight()
        {
            if (currentKB.IsKeyDown(Keys.D) && local) return true;
            if (currentGamePad.IsConnected && currentGamePad.ThumbSticks.Left.X > 0.0f && !local) return true;
            if (currentKB.IsKeyDown(Keys.Right) && !local) return true;

            return false;
        }

        public void MoveRight(GameTime theGameTime)
        {
            speed = Math.Abs(speed);
            //Set the Y frame to the second line of the body spritesheet if not jumping
            //and set the animation to active.
            if (!isJumping) bodyTempCurrentFrame.Y = 1;
            bodyAnimation.Active = true;
            //Don't flip the animation if moving right. This is passed into the animation draw
            //method.
            flip = false;

            if (position.X < entityManager.getScreenWidth() * 0.8)
                position.X += speed * theGameTime.ElapsedGameTime.Milliseconds;
        }

        private void AnimateHero(GameTime theGameTime)
        {
            //Update the body position and animation frames
            bodyTempCurrentFrame.X = bodyAnimation.CurrentFrame.X;
            bodyAnimation.Position = position;
            bodyAnimation.CurrentFrame = bodyTempCurrentFrame;
            bodyAnimation.Update(theGameTime);

            //Update the arm position and animation frames
            armTempCurrentFrame.X = armAnimation.CurrentFrame.X;
            armAnimation.Position = position;
            armAnimation.CurrentFrame = armTempCurrentFrame;
            armAnimation.Update(theGameTime);

            //Only show and update the blood animation when the blood timer is above 0
            // (when the character has been hit)
            if (countDownTimers[bloodTimer] > 0.0f)
            {
                bloodAnimation.Active = true;
                bloodTempCurrentFrame.X = bloodAnimation.CurrentFrame.X;
                bloodAnimation.Position = new Vector2(position.X - 5, position.Y - 5);
                bloodAnimation.CurrentFrame = bloodTempCurrentFrame;
                bloodAnimation.Update(theGameTime);
            }
            else
                bloodAnimation.Active = false;
        }

        #endregion

        #region Death
        public void RemoveLife()
        {
            numLives--;
            health = 100.0f;
            countDownTimers[deathTimer] = 1500.0f;
        }
        #endregion

        #region Getters and Setters
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
        #endregion

        #region Network

        public void sendPosition(Vector2 posDiff)
        {
            Game1.network.writeStream.Position = 0;
            Game1.network.writer.Write((byte)Protocol.PlayerMoved);
            Game1.network.writer.Write(posDiff.X);
            Game1.network.writer.Write(posDiff.Y);
            Game1.network.SendData(Game1.network.GetDataFromMemoryStream(Game1.network.writeStream));

            //Send animation update
            //sendAnimationState();
        }

        public void sendAnimationState()
        {
            Game1.network.writeStream.Position = 1;
            Game1.network.writer.Write((byte)Protocol.PlayerAnimationState);

            //Send the state of the following:
            // body animation current frame
            // position
            // arm animation current frame
            // if blood countdown timer > 0 - make active, current frame, position
            // if blood countdown timer < 0 - make inactive

            Game1.network.writer.Write(position.X);
            Game1.network.writer.Write(position.Y);
            Game1.network.writer.Write(bodyTempCurrentFrame.X);
            Game1.network.writer.Write(bodyTempCurrentFrame.Y);
            Game1.network.writer.Write(armTempCurrentFrame.X);
            Game1.network.writer.Write(armTempCurrentFrame.Y);
            
            if (countDownTimers[bloodTimer] > 0.0f)
            {
                Game1.network.writer.Write(true);
                Game1.network.writer.Write(bloodTempCurrentFrame.X);
                Game1.network.writer.Write(bloodTempCurrentFrame.Y);
            }
            else Game1.network.writer.Write(false);

            Game1.network.SendData(Game1.network.GetDataFromMemoryStream(Game1.network.writeStream));
        }

        public void getAnimationState(List<float> values)
        {
            //Set positions
            position = bodyAnimation.Position = new Vector2(values[0], values[1]);
            armAnimation.Position = new Vector2(values[0], values[1]);

            bodyTempCurrentFrame = bodyAnimation.CurrentFrame = new Vector2(values[2], values[3]);
            armTempCurrentFrame = armAnimation.CurrentFrame = new Vector2(values[4], values[5]);
            
            if (values[6] > 0.0f)
            {
                bloodAnimation.Active = true;
                bloodTempCurrentFrame = bloodAnimation.CurrentFrame = new Vector2(values[7], values[8]);
                bloodAnimation.Position = new Vector2(values[0] - 5, values[1] - 5);
            }
            else bloodAnimation.Active = false;
        }

        #endregion

        #region Combat

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

        private void attack(GameTime theGameTime)
        {
            //Check for left mouse click - Attack if not currently attacking

            if (!isAttacking && (currentKB.IsKeyDown(Keys.NumPad0) && !local) ||
                (currentMouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released && local) ||
                (currentGamePad.IsConnected && !local && currentGamePad.Buttons.X == ButtonState.Pressed && prevGamePad.Buttons.X == ButtonState.Released))
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
                if (countDownTimers[attackTimer] < 0.0f) isAttacking = false;

            }
            else
                armAnimation.Active = false;
            
        }


        public bool IsCharging()
        {
            if (currentKB.IsKeyDown(Keys.LeftShift))
                return true;
            else return false;
        }

        //Need to attach a timer with this like with the attack and such
        private void Charge(GameTime theGameTime)
        {
            float chargeSpeed;

            chargeSpeed = speed * 2.5f;
            position.X += chargeSpeed * (float)theGameTime.ElapsedGameTime.TotalMilliseconds;

            stamina -= 0.5f;
        }

        public void heal(int amount)
        {
            //Heal by given amount, cap at 100
            health += amount;
            if (health > 100) health = 100;
        }

        public void gainStamina(int amount)
        {
            //Increase stamina, cap at 100
            stamina += amount;
            if (stamina > 100) stamina = 100;
        }

        #endregion
    }
}
