using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace HnS
{
    public class Background
    {
        //------------------
        //Texture members
        //
        // Only 4 textures names for 5 texture memmbers, 2 textures are loaded from the same file.
        private string m_tex1Name, m_tex2Name, m_tex3Name, m_tex4Name;

        //List of textures to create the layered background. Texture3 and Texture5 are passed the same texture name
        // (this is to create the scroll-layer effect). 
        private Texture2D m_texture1, m_texture2, m_texture3, m_texture4, m_texture5;
        
        //-------------

        private Vector2 m_pos1, m_pos2, m_pos3, m_pos4, m_pos5;
       
        private float m_speed;
        

        //Constructor
        public Background(string tex1Name, string tex2Name, string tex3Name, string tex4Name, float scrollSpeed)
        {
            m_texture1 = null;
            m_texture2 = null;
            m_texture3 = null;
            m_texture4 = null;
            m_texture5 = null;
            m_tex1Name = tex1Name;
            m_tex2Name = tex2Name;
            m_tex3Name = tex3Name;
            m_tex4Name = tex4Name;
            m_speed = scrollSpeed;
        }

        //Load content
        public void LoadContent(ContentManager contentManager)
        {
            m_texture1 = contentManager.Load<Texture2D>(m_tex1Name);
            m_texture2 = contentManager.Load<Texture2D>(m_tex2Name);
            m_texture3 = contentManager.Load<Texture2D>(m_tex3Name);
            m_texture4 = contentManager.Load<Texture2D>(m_tex4Name);
            m_texture5 = contentManager.Load<Texture2D>(m_tex3Name); // Same texture file as texture 3
            m_pos1 = new Vector2(0, 0);
            m_pos2 = new Vector2(0, 0);
            m_pos3 = new Vector2(0, 0);
            m_pos4 = new Vector2(0, 0);
            m_pos5 = new Vector2(m_texture3.Width, 0);
        }

        public void Update(GameTime gameTime, bool moveLeft, bool moveRight)
        {
            //Set the scroll speed for if the player is moving right. Move left the third and fifth
            //texture positions so it looks like the player is moving in the right direction. Tag 
            //texture 5 onto the end of texture 3, and vice versa.
            if (moveRight)
            {
                m_pos3.X -= m_speed;
                m_pos5.X -= m_speed;
                if (m_pos3.X <= -(m_texture3.Width))
                    m_pos3.X = m_pos5.X + m_texture5.Width;
                if (m_pos5.X <= -(m_texture5.Width))
                    m_pos5.X = m_pos3.X + m_texture3.Width;
            }

            //Same as above except increase the texture positions so it appears the player
            //is moving in the left direction.
            else if (moveLeft)
            {
                m_pos3.X += m_speed;
                m_pos5.X += m_speed;
                if (m_pos3.X >= m_texture3.Width)
                    m_pos3.X = m_pos5.X - m_texture5.Width;
                if (m_pos5.X >= m_texture5.Width)
                    m_pos5.X = m_pos3.X - m_texture3.Width;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(m_texture1, m_pos1, Color.White);
            spriteBatch.Draw(m_texture2, m_pos2, Color.White);
            //Draw texture three and five (connected, scroll textures) before
            //drawing the foreground texture (texture4)
            spriteBatch.Draw(m_texture3, m_pos3, Color.White);
            spriteBatch.Draw(m_texture5, m_pos5, Color.White);
            spriteBatch.Draw(m_texture4, m_pos4, Color.White);
        }
    }
}
