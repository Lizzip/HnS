using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HnS
{
    //The main menu screen, which can be loaded before the action screen is loaded.
    class startScreen : gameScreen
    {
        #region Variables
        menuComponent menuComponent;
        Texture2D image;
        Rectangle imageRectangle;
        #endregion

        #region Getters and Setters
        public int SelectedIndex
        {
            get { return menuComponent.SelectedIndex; }
            set { menuComponent.SelectedIndex = value; }
        }
        #endregion

        #region Constructors and Loading
        public startScreen(Game theGame, SpriteBatch theSpriteBatch,
            SpriteFont theSpriteFont, Texture2D theImage, string[] theMenuItems)
            : base(theGame, theSpriteBatch)
        {
            string[] menuItems = theMenuItems;
            menuComponent = new menuComponent(theGame, theSpriteBatch, theSpriteFont,
                menuItems);
            Components.Add(menuComponent);
            this.image = theImage;
            imageRectangle = new Rectangle(0, 0, theGame.Window.ClientBounds.Width,
                theGame.Window.ClientBounds.Height);
        }
        #endregion

        #region Update and Draw
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Draw(image, imageRectangle, Color.White);
            base.Draw(gameTime);
        }
        #endregion
    }
}
