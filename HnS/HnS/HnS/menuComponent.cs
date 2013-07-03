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
    //Class to add a menu to a particular screen. It contains the necessary information
    //to hold different item selections, and registers the currently selected item
    //from its list, allowing transition up and down the items in the list.
    public class menuComponent : Microsoft.Xna.Framework.DrawableGameComponent
    {
        //List of menu strings that will be traversed with input keys.
        string[] menuItems;
        //Stores the currently selected item in the list.
        int selectedIndex;

        //Colours to render highlighted if selected.
        Color normal = Color.White;
        Color highLight = Color.Yellow;

        KeyboardState kbState;
        KeyboardState prevKbState;

        SpriteBatch spriteBatch;
        SpriteFont spriteFont;

        Vector2 position;
        float width = 0.0f;
        float height = 0.0f;

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public float Width
        {
            get { return width; }
        }

        public float Height
        {
            get { return height; }
        }

        //Gets and sets the current selection of the items in the list.
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                selectedIndex = value;
                //Keeps the selection with the bounds of 0 and the length of the item list.
                if (selectedIndex < 0)
                    selectedIndex = 0;
                if (selectedIndex >= menuItems.Length)
                    selectedIndex = menuItems.Length - 1;
            }
        }

        public menuComponent(Game theGame, SpriteBatch theSpriteBatch,
            SpriteFont theSpriteFont, string[] theMenuItems)
            : base(theGame)
        {
            this.spriteBatch = theSpriteBatch;
            this.spriteFont = theSpriteFont;
            this.menuItems = theMenuItems;
            MeasureMenu();
        }

        private void MeasureMenu()
        {
            height = 0;
            width = 0;

            //For each item in the item list, make the width of the menu component
            //equal to the length of the longest string in the menu items, and add
            //each menu item to the heigh of the menu component.
            foreach (string item in menuItems)
            {
                Vector2 size = spriteFont.MeasureString(item);
                if (size.X > width)
                    width = size.X;
                height += spriteFont.LineSpacing + 5;
            }

            position = new Vector2((Game.Window.ClientBounds.Width - width) / 2,
                (Game.Window.ClientBounds.Height - height) / 2);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        //Returns the currently pressed key.
        private bool CheckKey(Keys theKey)
        {
            return kbState.IsKeyUp(theKey) && prevKbState.IsKeyDown(theKey);
        }

        public override void Update(GameTime gameTime)
        {
            kbState = Keyboard.GetState();
            //If the down arrow is pressed, increase the index of the selected item, 
            //moving it down the list of items. Set the selected index back to the top
            //of the list if we pass the bottom of the list.
            if (CheckKey(Keys.Down))
            {
                selectedIndex++;
                if (selectedIndex == menuItems.Length)
                    selectedIndex = 0;
            }

            //Similar to above, but moving up the list instead of down.
            if (CheckKey(Keys.Up))
            {
                selectedIndex--;
                if (selectedIndex < 0)
                    selectedIndex = menuItems.Length - 1;
            }

            base.Update(gameTime);

            prevKbState = kbState;
        }

        public override void Draw(GameTime gameTime)
        {
            Vector2 location = position;
            Color tint;

            for (int i = 0; i < menuItems.Length; i++)
            {
                if (i == selectedIndex)
                    tint = highLight;
                else
                    tint = normal;
                spriteBatch.DrawString(spriteFont, menuItems[i], location, tint);
                location.Y += spriteFont.LineSpacing + 5;
            }

            base.Draw(gameTime);
        }
    }
}
