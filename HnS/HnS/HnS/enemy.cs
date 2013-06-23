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

        //General Vars
        //facing 0 = right, 1 = left
        int activeImage = 2, facing = 1;
        Vector2 position;
        float speed = 0.05f, countdownTimer = 250.0f;

        //Constructors
        public Enemy() { }

        public Enemy(EntityManager EM, Vector2 pos, ContentManager content, List<string> assets)
        {
            entityManager = EM;
            position = pos;
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

            //offset to draw ontop of the platform
            position.Y -= images.ElementAt(0).Height;
        }
        
        public override void update(Microsoft.Xna.Framework.GameTime theGameTime)
        {
            //If there is a countdown timer going on, count it down
            if (countdownTimer > 0.0f) countdownTimer -= theGameTime.ElapsedGameTime.Milliseconds;

            //Wander to the left of the screen
            position.X -= speed * theGameTime.ElapsedGameTime.Milliseconds;

            //Walking animations
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

                countdownTimer = 250.0f;
            }

            //If close enough to hero, raise sword
            if (Vector2.Distance(position, entityManager.getHero().getPos()) < 150.0f)
            {
                if (activeImage == 3)
                {
                    activeImage = 1;
                }
                else if (activeImage == 2)
                {
                    activeImage = 0;
                }
            }

            base.update(theGameTime);
        }

        public override void draw(Microsoft.Xna.Framework.Graphics.SpriteBatch theSpriteBatch)
        {
            //If facing right (0) draw normally, if facing left (1) flip sprite horizontally
            if (facing == 0) theSpriteBatch.Draw(images.ElementAt(activeImage), position, Color.White);
            else theSpriteBatch.Draw(images.ElementAt(activeImage), position, null,
                    Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.FlipHorizontally, 0);

            base.draw(theSpriteBatch);
        }

    }
}
