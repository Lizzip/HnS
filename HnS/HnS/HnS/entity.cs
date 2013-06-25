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
        //Update class to override
        public virtual void update(GameTime theGameTime){}

        //Draw class to override
        public virtual void draw(SpriteBatch theSpriteBatch){}

        //Recieve damage / be hit
        public virtual void beHit(float damage, Vector2 pointOfImpact) { }

        //get position of entity
        public virtual Vector2 getPos()
        {
            return Vector2.Zero;
        }

    }
}
