using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace HnS
{
    class Background : Entity
    {
        //-------------
        //Loaders and Managers
        EntityManager entityManager;

        //------------------
        //Texture members
        //
        // Only 4 textures names for 5 texture memmbers, 2 textures are loaded from the same file.
        private string m_tex1Name, m_tex2Name, m_tex3Name, m_tex4Name;
        private string m_floorName; //name of floor texture

        //List of textures to create the layered background. Texture3 and Texture5 are passed the same texture name
        // (this is to create the scroll-layer effect). 
        private Texture2D m_texture1, m_texture2, m_texture3, m_texture4, m_texture5;
        private Texture2D m_floorTex1, m_floorTex2; // Two textures for floor (both same file)
        
        //-------------
        //Position variables
        private Vector2 m_pos1, m_pos2, m_pos3, m_pos4, m_pos5, m_pos6, m_pos7; //6 + 7 used for floor

        //-------------
        //Scroll speed variables
        private float m_speed, m_floorSpeed;


        //Constructor
        public Background(ContentManager contentManager, EntityManager eManager, List<string> textureNames, float scrollSpeed)
        {
            entityManager = eManager;
            m_texture1 = null;
            m_texture2 = null;
            m_texture3 = null;
            m_texture4 = null;
            m_texture5 = null;
            m_tex1Name = textureNames.ElementAt(0);
            m_tex2Name = textureNames.ElementAt(1);
            m_tex3Name = textureNames.ElementAt(2);
            m_tex4Name = textureNames.ElementAt(3);
            m_floorName = textureNames.ElementAt(4);
            m_speed = scrollSpeed;
            m_floorSpeed = scrollSpeed / 4;
            LoadContent(contentManager);
        }

        //Load content
        public void LoadContent(ContentManager contentManager)
        {
            m_texture1 = contentManager.Load<Texture2D>(m_tex1Name);
            m_texture2 = contentManager.Load<Texture2D>(m_tex2Name);
            m_texture3 = contentManager.Load<Texture2D>(m_tex3Name);
            m_texture4 = contentManager.Load<Texture2D>(m_tex4Name);
            m_texture5 = contentManager.Load<Texture2D>(m_tex3Name); // Same texture file as texture 3
            m_floorTex1 = contentManager.Load<Texture2D>(m_floorName);
            m_floorTex2 = contentManager.Load<Texture2D>(m_floorName); // Both floor textures loaded from same file
            m_pos1 = Vector2.Zero;
            m_pos2 = Vector2.Zero;
            m_pos3 = Vector2.Zero;
            m_pos4 = Vector2.Zero;
            m_pos5 = new Vector2(m_texture3.Width, 0);
            m_pos6 = Vector2.Zero;
            m_pos7 = new Vector2(m_floorTex1.Width, 0);
        }

        public override void update(GameTime gameTime)
        {
            //Set the scroll speed for if the player is moving right. Move left the third and fifth
            //texture positions so it looks like the player is moving in the right direction. Tag 
            //texture 5 onto the end of texture 3, and vice versa.
            if (entityManager.getHero().MoveRight())
            {
                if (entityManager.getHero().getPos().X > entityManager.getScreenWidth() * 0.8)
                {
                    //scroll background image
                    m_pos3.X -= m_speed;
                    m_pos5.X -= m_speed;
                    if (m_pos3.X <= -(m_texture3.Width))
                        m_pos3.X = m_pos5.X + m_texture5.Width;
                    if (m_pos5.X <= -(m_texture5.Width))
                        m_pos5.X = m_pos3.X + m_texture3.Width;

                    //scroll floor image
                    m_pos6.X -= m_floorSpeed;
                    m_pos7.X -= m_floorSpeed;
                    if (m_pos6.X <= -(m_floorTex1.Width))
                        m_pos6.X = m_pos7.X + m_floorTex2.Width;
                    if (m_pos7.X <= -(m_floorTex2.Width))
                        m_pos7.X = m_pos6.X + m_floorTex1.Width;
                }
            }
                
            //Same as above except increase the texture positions so it appears the player
            //is moving in the left direction.
            else if (entityManager.getHero().MoveLeft())
            {
                if (entityManager.getHero().getPos().X < entityManager.getScreenWidth() * 0.2)
                {
                    //scroll background image
                    m_pos3.X += m_speed;
                    m_pos5.X += m_speed;
                    if (m_pos3.X >= m_texture3.Width)
                        m_pos3.X = m_pos5.X - m_texture5.Width;
                    if (m_pos5.X >= m_texture5.Width)
                        m_pos5.X = m_pos3.X - m_texture3.Width;

                    //scroll floor image
                    m_pos6.X += m_floorSpeed;
                    m_pos7.X += m_floorSpeed;
                    if (m_pos6.X >= m_floorTex1.Width)
                        m_pos6.X = m_pos7.X - m_floorTex2.Width;
                    if (m_pos7.X >= m_floorTex2.Width)
                        m_pos7.X = m_pos6.X - m_floorTex1.Width;
                }
            }

        }

        public override void draw(SpriteBatch spriteBatch)
        {
            //Draw sky and mountains
            spriteBatch.Draw(m_texture1, m_pos1, Color.White);
            spriteBatch.Draw(m_texture2, m_pos2, Color.White);

            //Draw texture three and five (connected, scroll textures) before
            //drawing the foreground texture (texture4)
            spriteBatch.Draw(m_texture3, m_pos3, Color.White);
            spriteBatch.Draw(m_texture5, m_pos5, Color.White);

            //Draw grass texture
            spriteBatch.Draw(m_texture4, m_pos4, Color.White);

            //Draw floor textures
            spriteBatch.Draw(m_floorTex1, m_pos6, Color.White);
            spriteBatch.Draw(m_floorTex2, m_pos7, Color.White);
        }
    }
}
