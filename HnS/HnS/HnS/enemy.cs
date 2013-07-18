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
    class Enemy : Entity
    {
        //Loaders and Managers
        ContentManager contentManager;
        EntityManager entityManager;

        //Textures and Fonts
        List<Texture2D> images = new List<Texture2D>();
        Texture2D healthBarOutline;
        Texture2D bloodSplat;
        SpriteFont font;

        //Timers
        List<float> countDownTimers = new List<float>();
        int walkingTimer = 0, bloodTimer = 1, attackTimer = 2, attackBufferTimer = 3;
        
        //Combat 
        Vector2 healthTextPos;
        int playerStrikingDistance = 20;

        //General Vars
        //facing 0 = right, 1 = left
        int facing, UID;
        Vector2 position, bloodPos, originalPos;
        float speed, scale, health, armourLevel;
        bool walking = true;

        ////////////////////////////////////////////////
        //Animation
        int armAnimWidth = 6, armAnimSpeed = 30;

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
        bool isAttacking, flip;
        //////////////////////////////////////////////


        ///////////////////////////////////////////////////
        // CONSTRUCTORS AND LOADING ///////////////////////
        ///////////////////////////////////////////////////

        public Enemy() { }

        public Enemy(EntityManager EM, int uid, Vector2 pos, ContentManager content)
        {
            entityManager = EM;
            UID = uid;
            isAttacking = false;
            flip = false;
            position = pos;
            contentManager = content;
            health = 100.0f;
            armourLevel = 1.0f;
            speed = 0.05f;
            scale = 0.7f;

            bodyTempCurrentFrame = Vector2.Zero;
            armTempCurrentFrame = Vector2.Zero;
            bloodTempCurrentFrame = Vector2.Zero;
            bodyAnimation = new animation(position, new Vector2(4, 2), 180);
            armAnimation = new animation(position, new Vector2(armAnimWidth, 1), armAnimSpeed);
            bodyTempCurrentFrame.Y = 1;
            bloodAnimation = new animation(position, new Vector2(6, 1), 60);


            facing = 1;
            loadContent();
        }

        void loadContent()
        {
            //Animation stuff
            bodyAnimation.AnimationImage = contentManager.Load<Texture2D>("regularEnemy\\enemyspritesheet");
            armAnimation.AnimationImage = contentManager.Load<Texture2D>("regularEnemy\\enemyarmspritesheet");
            bloodAnimation.AnimationImage = contentManager.Load<Texture2D>("bloodanim");

            //Load other images and fonts
            healthBarOutline = contentManager.Load<Texture2D>("enemyHealthBarOutline");
            font = contentManager.Load<SpriteFont>("smallfont");

            //offset to draw ontop of the platform
            position.Y -= bodyAnimation.FrameHeight * scale;
            originalPos = position;

            //Set health text position to just above enemy position
            healthTextPos = new Vector2(position.X, position.Y - 10);

            //Create countdown timers
            countDownTimers.Add(250.0f);//walking timer
            countDownTimers.Add(0.0f);//blood splat timer
            countDownTimers.Add(0.0f);//Attack timer
            countDownTimers.Add(0.0f);//Attack Buffer timer

            //Load bloodSplat image
            //bloodSplat = contentManager.Load<Texture2D>("bloodSplat");
        }

        void resetSelf()
        {
            position = originalPos;
            health = 100.0f;
            facing = 1;
            countDownTimers[walkingTimer] = 250.0f;
            countDownTimers[bloodTimer] = 0.0f;
        }


        ///////////////////////////////////////////////////
        // ENTITY OVERRIDES ///////////////////////////////
        ///////////////////////////////////////////////////

        public override void update(Microsoft.Xna.Framework.GameTime theGameTime)
        {
            //Die if health is 0
            if (health < 1)
            {
                die();
            }
            else
            {
                //Animation
                bodyTempCurrentFrame.X = bodyAnimation.CurrentFrame.X;
                bodyAnimation.Position = position;
                bodyAnimation.CurrentFrame = bodyTempCurrentFrame;
                bodyAnimation.Update(theGameTime);

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
                    bloodAnimation.Position = new Vector2(position.X - 24, position.Y - 22);
                    bloodAnimation.CurrentFrame = bloodTempCurrentFrame;
                    bloodAnimation.Update(theGameTime);
                }
                else
                    bloodAnimation.Active = false;

                //Count down all count down timers in progress
                for (int i = 0, len = countDownTimers.Count; i < len; i++)
                {
                    if (countDownTimers[i] > -1.0f)
                    {
                        countDownTimers[i] -= (float)theGameTime.ElapsedGameTime.Milliseconds;
                    }
                }

                if (Vector2.Distance(position, entityManager.getHero().getPos()) > 2.0f) walking = true;
                else walking = false;

                //Walk towards player
                if (walking)
                {
                    bodyAnimation.Active = true;

                    //face player
                    if (entityManager.getHero().getPos().X > position.X)
                    {
                        facing = 0;
                        flip = false;
                    }
                    else
                    {
                        facing = 1;
                        flip = true;
                    }

                    if (entityManager.getHero().getPos().X > entityManager.getScreenWidth() * 0.8 && entityManager.getHero().IsMovingRight())
                    {
                        //Dont move position as screen is scrolling
                        if (facing == 1)
                            position.X -= speed * 2 * (float)theGameTime.ElapsedGameTime.TotalMilliseconds;
                        else
                            position.X -= speed * (float)theGameTime.ElapsedGameTime.TotalMilliseconds;
                    }
                    else if (entityManager.getHero().getPos().X < entityManager.getScreenWidth() * 0.2 && entityManager.getHero().IsMovingLeft())
                    {
                        if (facing == 1)
                            position.X += speed  * (float)theGameTime.ElapsedGameTime.TotalMilliseconds;
                        else
                            position.X += speed * 2 * (float)theGameTime.ElapsedGameTime.TotalMilliseconds;
                    }
                    else
                    {
                        //Wander direction of facing/player
                        if (facing == 1) position.X -= speed * theGameTime.ElapsedGameTime.Milliseconds;
                        else position.X += speed * theGameTime.ElapsedGameTime.Milliseconds;
                    }
                }
                else
                {
                    bodyAnimation.Active = false;
                }

                //If close enough to hero, raise sword
                if (Vector2.Distance(position, entityManager.getHero().getPos()) < playerStrikingDistance && countDownTimers[attackBufferTimer] < 0.0f)
                {
                    //Attack if not already attacking
                    if (!isAttacking)
                    {
                        isAttacking = true;
                        armAnimation.Active = true;
                        countDownTimers[attackTimer] = 180.0f;
                        countDownTimers[attackBufferTimer] = 1000.0f;
                        entityManager.broadcastAttackEnemy(10.0f, position);
                    }
                }

                if (isAttacking && countDownTimers[attackTimer] < 0.0f)
                {
                    isAttacking = false;
                    armAnimation.Active = false;
                }

                if (countDownTimers[attackTimer] < 0.0f)
                {
                    armAnimation.Active = false;
                }

                //Update health text position
                healthTextPos = new Vector2(position.X, position.Y - 15);
            }
            base.update(theGameTime);
        }

        public override void draw(Microsoft.Xna.Framework.Graphics.SpriteBatch theSpriteBatch)
        {
            if (health > 0)
            {
                //Draw the blood animation if the enemy has recently been hit
                if (bloodAnimation.Active)
                {
                    bloodAnimation.Draw(theSpriteBatch, 1.0f, flip);
                }

                //Draw the arm and body
                armAnimation.Draw(theSpriteBatch, scale, flip);
                bodyAnimation.Draw(theSpriteBatch, scale, flip);

                //Draw white health bar outline
                theSpriteBatch.Draw(healthBarOutline, new Vector2(position.X, position.Y - 17), Color.White);

                //Draw grey health bar area for health lost
                theSpriteBatch.Draw(healthBarOutline, new Rectangle((int)position.X + 1, (int)position.Y - 15, healthBarOutline.Width - 2, 8),
                    null, Color.Gray);

                //Draw a red health bar area for current health
                theSpriteBatch.Draw(healthBarOutline, new Rectangle((int)position.X + 1, (int)position.Y - 15, (int)(healthBarOutline.Width * ((double)health / 100) - 2), 8),
                    null, Color.Red);
            }

            base.draw(theSpriteBatch);
        }

        ///////////////////////////////////////////////////
        // GETTERS AND SETTERS ////////////////////////////
        ///////////////////////////////////////////////////

        public override Vector2 getPos()
        {
            return position;
        }

        ///////////////////////////////////////////////////
        // COMBAT /////////////////////////////////////////
        ///////////////////////////////////////////////////

        public override void beHit(float damage, Vector2 pointOfImpact)
        {
            health -= damage * armourLevel;
            splurgeBlood(pointOfImpact);
        }

        void splurgeBlood(Vector2 pointOfImpact)
        {
            countDownTimers[bloodTimer] = 500.0f;
            bloodPos = pointOfImpact;
        }

        public void die()
        {

            if (entityManager.getMaxEnemyCount() > 0)
            {
                entityManager.createPotion(Color.Red, new Vector2(position.X, entityManager.getPlatformHeight() - 150));
                entityManager.reduceMaxEnemyCount();
                resetSelf();
            }
        }

    }
}
