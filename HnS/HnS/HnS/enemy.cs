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
        //General Vars
        Vector2 position;

        //Constructors
        public Enemy() { }

        public Enemy(Vector2 pos)
        {
            position = pos;
        }

        public override void update(Microsoft.Xna.Framework.GameTime theGameTime)
        {
            base.update(theGameTime);
        }

        public override void draw(Microsoft.Xna.Framework.Graphics.SpriteBatch theSpriteBatch)
        {
            base.draw(theSpriteBatch);
        }

    }
}
