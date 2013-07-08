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
        private string skyName, mountainsName, hillsName, treesName;
        private string cloudsName;
        private string floorName; //name of floor texture

        //List of textures to create the layered background. Texture3 and Texture5 are passed the same texture name
        // (this is to create the scroll-layer effect). 
        private Texture2D skyTex, mountainsTex, hillsTex, trees1Tex, trees2Tex;
        //Clouds
        private Texture2D clouds1Tex, clouds2Tex; 
        private Texture2D floor1Tex, floor2Tex; // Two textures for floor (both same file)
        
        //-------------
        //Position variables
        private Vector2 skyPos, mountainsPos, hillsPos, trees1Pos, trees2Pos;
        private Vector2 clouds1Pos, clouds2Pos;
        private Vector2 floor1Pos, floor2Pos;
        //-------------
        //Scroll speed variables
        private float speed, floorSpeed, cloudsSpeed;


        //Constructor
        public Background(ContentManager contentManager, EntityManager eManager, List<string> textureNames, float scrollSpeed)
        {
            entityManager = eManager;
            skyTex = null;
            mountainsTex = null;
            hillsTex = null;
            trees1Tex = null;
            trees2Tex = null;
            skyName = textureNames.ElementAt(0);
            mountainsName = textureNames.ElementAt(1);
            hillsName = textureNames.ElementAt(2);
            treesName = textureNames.ElementAt(3);
            floorName = textureNames.ElementAt(4);
            cloudsName = textureNames.ElementAt(5);
            speed = scrollSpeed / 3;
            floorSpeed = scrollSpeed * 2;
            cloudsSpeed = scrollSpeed / 10;
            LoadContent(contentManager);
        }

        //Load content
        public void LoadContent(ContentManager contentManager)
        {
            skyTex = contentManager.Load<Texture2D>(skyName);
            mountainsTex = contentManager.Load<Texture2D>(mountainsName);
            hillsTex = contentManager.Load<Texture2D>(hillsName);
            trees1Tex = contentManager.Load<Texture2D>(treesName);
            trees2Tex = contentManager.Load<Texture2D>(treesName); // Same texture file as texture 3
            floor1Tex = contentManager.Load<Texture2D>(floorName);
            floor2Tex = contentManager.Load<Texture2D>(floorName); // Both floor textures loaded from same file
            clouds1Tex = contentManager.Load<Texture2D>(cloudsName);
            clouds2Tex = contentManager.Load<Texture2D>(cloudsName);
            skyPos = Vector2.Zero;
            mountainsPos = Vector2.Zero;
            hillsPos = Vector2.Zero;
            trees1Pos = Vector2.Zero;
            trees2Pos = new Vector2(trees1Tex.Width, 0);
            floor1Pos = Vector2.Zero;
            floor2Pos = new Vector2(floor1Tex.Width, 0);
            clouds1Pos = Vector2.Zero;
            clouds2Pos = new Vector2(clouds1Tex.Width, 0);
        }

        public override void update(GameTime gameTime)
        {
            //Set the scroll speed for if the player is moving right. Move left the third and fifth
            //texture positions so it looks like the player is moving in the right direction. Tag 
            //texture 5 onto the end of texture 3, and vice versa.
            if (entityManager.getHero().IsMovingRight())
            {
                if (entityManager.getHero().getPos().X > entityManager.getScreenWidth() * 0.8)
                {
                    //scroll trees image
                    trees1Pos.X -= speed;
                    trees2Pos.X -= speed;
                    if (trees1Pos.X <= -(trees1Tex.Width))
                        trees1Pos.X = trees2Pos.X + trees2Tex.Width;
                    if (trees2Pos.X <= -(trees2Tex.Width))
                        trees2Pos.X = trees1Pos.X + trees1Tex.Width;

                    //scroll floor image
                    floor1Pos.X -= floorSpeed;
                    floor2Pos.X -= floorSpeed;
                    if (floor1Pos.X <= -(floor1Tex.Width))
                        floor1Pos.X = floor2Pos.X + floor2Tex.Width;
                    if (floor2Pos.X <= -(floor2Tex.Width))
                        floor2Pos.X = floor1Pos.X + floor1Tex.Width;
                }
            }

            //scroll clouds
            clouds1Pos.X -= cloudsSpeed;
            clouds2Pos.X -= cloudsSpeed;
            if (clouds1Pos.X <= -(clouds1Tex.Width))
                clouds1Pos.X = clouds2Pos.X + clouds2Tex.Width;
            if (clouds2Pos.X <= -(clouds2Tex.Width))
                clouds2Pos.X = clouds1Pos.X + clouds1Tex.Width;
            
                
            //Same as above except increase the texture positions so it appears the player
            //is moving in the left direction.
            else if (entityManager.getHero().IsMovingLeft())
            {
                if (entityManager.getHero().getPos().X < entityManager.getScreenWidth() * 0.2)
                {
                    //scroll trees image
                    trees1Pos.X += speed;
                    trees2Pos.X += speed;
                    if (trees1Pos.X >= trees1Tex.Width)
                        trees1Pos.X = trees2Pos.X - trees2Tex.Width;
                    if (trees2Pos.X >= trees2Tex.Width)
                        trees2Pos.X = trees1Pos.X - trees1Tex.Width;

                    //scroll floor image
                    floor1Pos.X += floorSpeed;
                    floor2Pos.X += floorSpeed;
                    if (floor1Pos.X >= floor1Tex.Width)
                        floor1Pos.X = floor2Pos.X - floor2Tex.Width;
                    if (floor2Pos.X >= floor2Tex.Width)
                        floor2Pos.X = floor1Pos.X - floor1Tex.Width;
                }
            }

        }

        public override void draw(SpriteBatch spriteBatch)
        {
            //Draw sky
            spriteBatch.Draw(skyTex, skyPos, Color.White);

            //Draw clouds
            spriteBatch.Draw(clouds1Tex, clouds1Pos, Color.White);
            spriteBatch.Draw(clouds2Tex, clouds2Pos, Color.White);

            //Draw mountains
/*          spriteBatch.Draw(m_texture2, m_pos2, Color.White);
            
            //Draw grass texture
            spriteBatch.Draw(m_texture4, m_pos4, Color.White);
 */

            //Draw hills texture
            spriteBatch.Draw(hillsTex, hillsPos, Color.White);

            //Draw trees
            spriteBatch.Draw(trees1Tex, trees1Pos, Color.White);
            spriteBatch.Draw(trees2Tex, trees2Pos, Color.White);

            //Draw floor textures
            spriteBatch.Draw(floor1Tex, floor1Pos, Color.White);
            spriteBatch.Draw(floor2Tex, floor2Pos, Color.White);
        }
    }
}
