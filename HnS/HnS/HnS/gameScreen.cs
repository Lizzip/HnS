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
    #region Constructors and Loading
    //Game screen layout class. Not instantiated itself but contains all items
    //to instantiate a basic screen: main menu, pop-up menu, main game etc.
    public abstract class gameScreen : Microsoft.Xna.Framework.DrawableGameComponent
    {
        List<GameComponent> components = new List<GameComponent>();
        protected Game game;
        protected SpriteBatch spriteBatch;

        public List<GameComponent> Components
        {
            get { return components; }
        }

        public gameScreen(Game theGame, SpriteBatch theSpriteBatch)
            : base(theGame)
        {
            this.game = theGame;
            this.spriteBatch = theSpriteBatch;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Only update the components in the list if enabled is set to true
            foreach (GameComponent component in components)
                if (component.Enabled == true)
                    component.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            //Only draw the each component if it is a drawable game component
            foreach (GameComponent component in components)
            {
                if (component is DrawableGameComponent && ((DrawableGameComponent)component).Visible)
                    ((DrawableGameComponent)component).Draw(gameTime);
            }
        }

        public virtual void Show()
        {
            this.Visible = true;
            this.Enabled = true;
            foreach (GameComponent component in components)
            {
                component.Enabled = true;
                if (component is DrawableGameComponent)
                    ((DrawableGameComponent)component).Visible = true;
            }
        }

        public virtual void Hide()
        {
            this.Visible = false;
            this.Enabled = false;
            foreach (GameComponent component in components)
            {
                if (component is DrawableGameComponent)
                    ((DrawableGameComponent)component).Visible = false;
            }
        }
    }
    #endregion
}
