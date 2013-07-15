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
    class Potion : Entity
    {
        EntityManager entityManager;
        int UID;
        Vector2 position;            
        float scale, minLevel, velocityY, vChange = 0.15f;
        ContentManager contentManager;
        Texture2D image;
        Color colour;
        bool exists, isFalling;
        
        ///////////////////////////////////////////////////
        // CONSTRUCTORS AND LOADING ///////////////////////
        ///////////////////////////////////////////////////

        public Potion() { }

        public Potion(EntityManager eManager, int uid, Vector2 pos, ContentManager content, Color color)
        {
            entityManager = eManager;
            UID = uid;
            exists = true;
            position = pos;            
            scale = 0.5f;
            contentManager = content;
            colour = color;
            isFalling = false;
            loadContent();
        }

        void loadContent()
        {
            if (colour == Color.Yellow)
            {
                image = contentManager.Load<Texture2D>("pots\\yellowPot");
            }
            else
            {
                image = contentManager.Load<Texture2D>("pots\\redPot");
            }

            minLevel = entityManager.getPlatformHeight() - (image.Height * scale);
        }

        public override void update(GameTime theGameTime)
        {
            //Hero picks up pot when close enough
            if (Vector2.Distance(position, entityManager.getHero().getPos()) < 40) pickup();

            //If potion is above platform height, its falling
            if (position.Y < minLevel)isFalling = true;
            else isFalling = false;

            //If its falling, apply velocity
            if (isFalling) velocityY += vChange;
            else velocityY = 0;

            //Update Y pos for falling
            position.Y += velocityY;
            base.update(theGameTime);
        }

        public override void draw(SpriteBatch theSpriteBatch)
        {
            if (exists)
            {
                theSpriteBatch.Draw(image, position, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            }
        }

        public void pickup()
        {
            if (colour == Color.Red && exists == true)
            {
                entityManager.getHero().heal(25);
                exists = false;
            }
        }
    }
}
