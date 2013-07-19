using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HnS
{
    public class animation
    {
        #region Variables
        //Variables to store the number of frames passed and
        //a counter to determine when to switch to the next frame.
        int frameCount;
        int frameSwitch;

        bool active;
        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        ///////////////
        //Variables for determining and drawing the current frame
        //of the spritesheet.
        Vector2 position, numFrames, currentFrame;
        Texture2D image;
        Rectangle source;
        #endregion

        #region Getters and Setters
        //Getters and setters for the frame variables
        public Vector2 CurrentFrame
        {
            get { return currentFrame; }
            set { currentFrame = value; }
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Texture2D AnimationImage
        {
            set { image = value; }
        }

        public int FrameWidth
        {
            get { return image.Width / (int)numFrames.X; }
        }

        public int FrameHeight
        {
            get { return image.Height / (int)numFrames.Y; }
        }

        //End getters and setters
        ///////////////////////////
        #endregion

        #region Update and Draw
        public animation(Vector2 thePosition, Vector2 theNumFrames, int theFrameSwitch)
        {
            active = false;
            //Need to test this number, higher = slower frame switch, lower = quicker.
            frameSwitch = theFrameSwitch;
            this.position = thePosition;
            this.numFrames = theNumFrames;
        }

        public void Update(GameTime gameTime)
        {
            if (active)
                frameCount += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            else
                frameCount = 0;
            if (frameCount >= frameSwitch)
            {
                frameCount = 0;
                currentFrame.X += FrameWidth;
                if (currentFrame.X >= image.Width)
                    currentFrame.X = 0;
            }

            source = new Rectangle((int)currentFrame.X, (int)currentFrame.Y * FrameHeight,
                FrameWidth, FrameHeight);
        }

        public void Draw(SpriteBatch spriteBatch, float scale, bool flip)
        {
            SpriteEffects effect;
            if (flip)
                effect = SpriteEffects.FlipHorizontally;
            else
                effect = SpriteEffects.None;
            //spriteBatch.Draw(image, position, source, Color.White);
            spriteBatch.Draw(image, position, source, Color.White, 0, Vector2.Zero, scale, effect, 0);
        }

        //Draw a specifc frame (such as when jumping or hero is stationary -- possibly better way of doing this)
        public void forceDraw(SpriteBatch spriteBatch, float scale, bool flip, int offsetX, int offsetY)
        {
            SpriteEffects effect;
            if (flip)
                effect = SpriteEffects.FlipHorizontally;
            else
                effect = SpriteEffects.None;

            spriteBatch.Draw(image, position, new Rectangle(offsetX * FrameWidth, offsetY * FrameHeight,
                FrameWidth, FrameHeight), Color.White, 0, Vector2.Zero, scale, effect, 0);
        }
        #endregion

        #region Network
        public void sendAnimation()
        {

        }
        #endregion
    }
}
