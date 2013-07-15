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
        float scale;
        ContentManager contentManager;
        Texture2D image;
        Color colour;
        
        ///////////////////////////////////////////////////
        // CONSTRUCTORS AND LOADING ///////////////////////
        ///////////////////////////////////////////////////

        public Potion() { }

        public Potion(EntityManager eManager, int uid, Vector2 pos, ContentManager content, Color color)
        {
            entityManager = eManager;
            UID = uid;
            position = pos;            
            scale = 0.5f;
            contentManager = content;
            colour = color;
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

            position.Y -= image.Height * scale;
        }

        public override void update(GameTime theGameTime)
        {
            base.update(theGameTime);
        }

        public override void draw(SpriteBatch theSpriteBatch)
        {
            theSpriteBatch.Draw(image, position, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
        }


    }
}
