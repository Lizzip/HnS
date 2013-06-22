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
    class Entity
    {
        public virtual Vector2 position;
        Texture2D image;
        EntityManager entityManager;

        public Entity() { }

        public Entity(Vector2 pos)
        {
            position = pos;
        }

        public virtual void update(GameTime theGameTime)
        { 

        }


        public virtual void draw(SpriteBatch theSpriteBatch)
        {

        }


    }
}
