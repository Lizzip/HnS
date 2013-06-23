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
        //Texture members
        private string m_tex1Name, m_tex2Name;
        private Texture2D m_texture1, m_texture2;
        public Texture2D Texture1
        {
            get { return m_texture1; }
        }

        public Texture2D Texture2
        {
            get { return m_texture2; }
        }

        //-------------

        private Vector2 m_pos1, m_pos2;
        public Vector2 Tex1Position
        {
            get { return m_pos1; }
        }

        public Vector2 Tex2Position
        {
            get { return m_pos2; }
        }

        private float m_speed;
        public float Speed
        {
            get { return m_speed; }
        }

        //Constructor
        public Background(string tex1Name, string tex2Name, float scrollSpeed)
        {
            m_texture1 = null;
            m_texture2 = null;
            m_tex1Name = tex1Name;
            m_tex2Name = tex2Name;
            m_speed = scrollSpeed;
        }

        //Load content
        public void LoadContent(ContentManager contentManager)
        {
            m_texture1 = contentManager.Load<Texture2D>(m_tex1Name);
            m_texture2 = contentManager.Load<Texture2D>(m_tex2Name);
            m_pos1 = new Vector2(0, 0);
            m_pos2 = new Vector2(m_texture1.Width, 0);
        }

        public void Update(GameTime gameTime, bool moveLeft, bool moveRight)
        {
            //Set the background scroll speed
            //if (moveRight)
           //{
            if (moveRight)
            {
                m_pos1.X -= m_speed;
                m_pos2.X -= m_speed;
                if (m_pos1.X <= -(m_texture1.Width))
                    m_pos1.X = m_pos2.X + m_texture2.Width;

                if (m_pos2.X <= -(m_texture1.Width))
                    m_pos2.X = m_pos1.X + m_texture1.Width;
            }

            else if (moveLeft)
            {
                m_pos1.X += m_speed;
                m_pos2.X += m_speed;
                if (m_pos1.X >= m_texture1.Width)
                    m_pos1.X = m_pos2.X - m_texture2.Width;

                if (m_pos2.X >= m_texture1.Width)
                    m_pos2.X = m_pos1.X - m_texture1.Width;
            }
                
            
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(m_texture1, m_pos1, Color.White);
            spriteBatch.Draw(m_texture2, m_pos2, Color.White);
        }
    }
}
