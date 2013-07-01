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
    //This is the "main game" screen, where the action in the game takes place. All
    //code from the Game1 class can be placed here. 
    class actionScreen : gameScreen
    {
        KeyboardState kbState;
        
        Background background;

        public actionScreen(Game theGame, SpriteBatch theSpriteBatch)
            : base(theGame, theSpriteBatch)
        {

        }

        public void loadContent(ContentManager theContent)
        {
            base.LoadContent();

        }

        public override void Update(GameTime theGameTime)
        {
            base.Update(theGameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

    }
}
