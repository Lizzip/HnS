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
        
        public animation(Vector2 thePosition, Vector2 theNumFrames)
        {
            active = false;
            //Need to test this number, higher = slower frame switch, lower = quicker.
            frameSwitch = 90;
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
    }
}
