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

        //Input states
        KeyboardState currentKB, prevKB;
        MouseState currentMouse, prevMouse;

        //Textures and Fonts
        List<Texture2D> images = new List<Texture2D>();
        Texture2D healthBarOutline;
        SpriteFont smallFont;
                
        //General Vars
        //facing 0 = right, 1 = left
        int activeImage = 0, health, facing = 0;
        Vector2 position, healthTextPos;
        float speed = 0.15f, countdownTimer = 100.0f;

        //Constructors
        public Hero() { }

        public Hero(Vector2 pos, ContentManager content, List<string> assets)
        {
            position = pos;
            health = 100;
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

            //Set other variables (adjust default draw height for image height - to draw hero standing on platform)
            position.Y -= images.ElementAt(0).Height;
            healthTextPos = new Vector2(25, 17);
        }

        public override void update(Microsoft.Xna.Framework.GameTime theGameTime)
        {
            //If there is a countdown timer going on, count it down
            if (countdownTimer > 0.0f) countdownTimer -= theGameTime.ElapsedGameTime.Milliseconds;

            //Get new mouse and keyboard states
            currentKB = Keyboard.GetState();
            currentMouse = Mouse.GetState();

            //Attack with left mouseclick
            if (currentMouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
            {
                if (activeImage == 0) activeImage = 2;
                else activeImage = 0;
            }

            //Switch directions
            if (currentKB.IsKeyDown(Keys.A) && prevKB.IsKeyDown(Keys.A) == false) facing = 1;
            if (currentKB.IsKeyDown(Keys.D) && prevKB.IsKeyDown(Keys.D) == false) facing = 0;

            //Walking movement
            if (currentKB.IsKeyDown(Keys.D))
            {
                if (countdownTimer < 0.0f)
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

                    countdownTimer = 100.0f;
                }

                position.X += speed * theGameTime.ElapsedGameTime.Milliseconds;
            }

            if (currentKB.IsKeyDown(Keys.A))
            {
                if (countdownTimer < 0.0f)
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

                    countdownTimer = 100.0f;
                }

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
            if (facing == 0) theSpriteBatch.Draw(images.ElementAt(activeImage), position, Color.White);
            else theSpriteBatch.Draw(images.ElementAt(activeImage), position, null,
                    Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.FlipHorizontally, 0);

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

            base.draw(theSpriteBatch);
        }

    }
}