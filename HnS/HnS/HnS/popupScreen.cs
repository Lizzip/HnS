using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HnS
{
    class popupScreen : gameScreen
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
        public popupScreen(Game theGame, SpriteBatch theSpriteBatch,
            SpriteFont theSpriteFont, Texture2D theImage, string[] theMenuItems)
            : base(theGame, theSpriteBatch)
        {
            menuComponent = new menuComponent(theGame, theSpriteBatch, theSpriteFont,
                theMenuItems);
            Components.Add(menuComponent);
            image = theImage;
            imageRectangle = new Rectangle((theGame.Window.ClientBounds.Width - image.Width) / 2,
                (theGame.Window.ClientBounds.Height - image.Height) / 2,
                image.Width, image.Height);

            menuComponent.Position = new Vector2((imageRectangle.Width - menuComponent.Width) / 2 + imageRectangle.Left,
                (imageRectangle.Height - menuComponent.Height) / 2 + imageRectangle.Top + 20);
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
