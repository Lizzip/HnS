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
        int walkingTimer = 0, bloodTimer = 1, attackTimer = 2;
        
        //Combat 
        Vector2 healthTextPos;
        int playerStrikingDistance = 20;
        bool isAttacking;

        //General Vars
        //facing 0 = right, 1 = left
        int activeImage, facing, UID;
        Vector2 position, bloodPos, originalPos;
        float speed, scale, health, armourLevel;
        bool walking = true;


        ///////////////////////////////////////////////////
        // CONSTRUCTORS AND LOADING ///////////////////////
        ///////////////////////////////////////////////////

        public Enemy() { }

        public Enemy(EntityManager EM, int uid, Vector2 pos, ContentManager content, List<string> assets)
        {
            entityManager = EM;
            UID = uid;
            isAttacking = false;
            position = pos;
            contentManager = content;
            health = 100.0f;
            armourLevel = 1.0f;
            speed = 0.05f;
            scale = 0.8f;
            facing = 1;
            activeImage = 2;
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
            healthBarOutline = contentManager.Load<Texture2D>("enemyHealthBarOutline");
            font = contentManager.Load<SpriteFont>("smallfont");

            //offset to draw ontop of the platform
            position.Y -= images.ElementAt(0).Height * scale;
            originalPos = position;

            //Set health text position to just above enemy position
            healthTextPos = new Vector2(position.X, position.Y - 10);

            //Create countdown timers
            countDownTimers.Add(250.0f);//walking timer
            countDownTimers.Add(0.0f);//blood splat timer
            countDownTimers.Add(0.0f);//Attack timer

            //Load bloodSplat image
            bloodSplat = contentManager.Load<Texture2D>("bloodSplat");
        }

        void resetSelf()
        {
            position = originalPos;
            health = 100.0f;
            facing = 1;
            activeImage = 2;
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
                //Count down all count down timers in progress
                for (int i = 0, len = countDownTimers.Count; i < len; i++)
                {
                    if (countDownTimers[i] > 0.0f)
                    {
                        countDownTimers[i] -= (float)theGameTime.ElapsedGameTime.Milliseconds;
                    }
                }

                //Walk towards player
                if (walking)
                {
                    //face player
                    if (entityManager.getHero().getPos().X > position.X) facing = 0;
                    else facing = 1;

                    if ((entityManager.getHero().getPos().X > entityManager.getScreenWidth() * 0.8 && entityManager.getHero().IsMovingRight()) ||
                        (entityManager.getHero().getPos().X < entityManager.getScreenWidth() * 0.2 && entityManager.getHero().IsMovingLeft()))
                    {
                        //Dont move position as screen is scrolling
                    }
                    else
                    {
                        //Wander direction of facing/player
                        if (facing == 1) position.X -= speed * theGameTime.ElapsedGameTime.Milliseconds;
                        else position.X += speed * theGameTime.ElapsedGameTime.Milliseconds;
                    }

                    //Cycle walking animations
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

                        countDownTimers[walkingTimer] = 250.0f;
                    }
                }
                else
                {
                    if (Vector2.Distance(position, entityManager.getHero().getPos()) > playerStrikingDistance)
                    {
                        walking = true;

                        if (activeImage == 1)
                        {
                            activeImage = 3;
                        }
                        else if (activeImage == 0)
                        {
                            activeImage = 2;
                        }
                    }
                }

                //If close enough to hero, raise sword
                if (Vector2.Distance(position, entityManager.getHero().getPos()) < playerStrikingDistance)
                {
                    if (activeImage == 3)
                    {
                        activeImage = 1;
                    }
                    else if (activeImage == 2)
                    {
                        activeImage = 0;
                    }
                    walking = false;

                    //Attack if not already attacking
                    if (!isAttacking)
                    {
                        isAttacking = true;
                        countDownTimers[attackTimer] = 1000.0f;
                        entityManager.broadcastAttackEnemy(10.0f, position);
                    }
                }

                if (isAttacking && countDownTimers[attackTimer] < 0.0f)
                {
                    isAttacking = false;
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
                //If facing right (0) draw normally, if facing left (1) flip sprite horizontally
                if (facing == 0) theSpriteBatch.Draw(images.ElementAt(activeImage), position, null,
                        Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                else theSpriteBatch.Draw(images.ElementAt(activeImage), position, null,
                        Color.White, 0, Vector2.Zero, scale, SpriteEffects.FlipHorizontally, 0);

                //draw blood if we've been hit recently
                if (countDownTimers[bloodTimer] > 0.0f)
                {
                    theSpriteBatch.Draw(bloodSplat, new Vector2(position.X + (images.ElementAt(activeImage).Width * scale) / 2,
                        position.Y + (images.ElementAt(activeImage).Height * scale) / 2),
                        null, Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
                }

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
            countDownTimers[bloodTimer] = 1000.0f;
            bloodPos = pointOfImpact;
        }

        public void die()
        {

            if (entityManager.getMaxEnemyCount() > 0)
            {
                entityManager.reduceMaxEnemyCount();
                resetSelf();
            }
        }

    }
}
