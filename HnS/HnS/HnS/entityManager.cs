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
    class EntityManager
    {
        ///////////////////////////////////////////////////
        // VARIABLES //////////////////////////////////////
        ///////////////////////////////////////////////////

        //Loaders and Managers
        ContentManager contentManager;

        //UID Management
        int nextUID = 0, heroUID;
        public List<int> UIDs = new List<int>();

        //Entity Management
        Dictionary<int, Entity> entityMap = new Dictionary<int, Entity>();

        //Variables needed by all entities
        int platformHeight, maxEnemyCount;

        //Store screen width and height
        int screenWidth, screenHeight;
        

        ///////////////////////////////////////////////////
        // CONSTRUCTORS ///////////////////////////////////
        ///////////////////////////////////////////////////

        //Constructor
        public EntityManager(ContentManager content, int pHeight, int screenW, int screenH, int maxEnemies = 2)
        {
            contentManager = content;
            platformHeight = pHeight;
            screenWidth = screenW;
            screenHeight = screenH;
            maxEnemyCount = maxEnemies;
        }

        //Create player char entity
        public void createHero(Vector2 pos, List<string> assets)
        {
            Hero hero = new Hero(this, pos, contentManager, assets);
            entityMap.Add(nextUID, hero);
            UIDs.Add(nextUID);
            heroUID = nextUID;
            nextUID++;
        }

        //Create enemy entity
        public void createEnemy(Vector2 pos, List<string> assets)
        {
            if (maxEnemyCount > 0)
            {
                Enemy enemy = new Enemy(this, pos, contentManager, assets);
                entityMap.Add(nextUID, enemy);
                UIDs.Add(nextUID);
                nextUID++;
                maxEnemyCount--;
            }
        }
        
        //Create backgroundManager entity
        public void createBackground(List<string> textureNames, float scrollSpeed)
        {
            Background background = new Background(contentManager, this, textureNames, scrollSpeed);
            entityMap.Add(nextUID, background);
            UIDs.Add(nextUID);
            nextUID++;
        }


        ///////////////////////////////////////////////////
        // UPDATE AND DRAW ////////////////////////////////
        ///////////////////////////////////////////////////

        //Update all entities
        public void updateAll(GameTime theGameTime)
        {
            for (int i = 0, len = entityMap.Count; i < len; i++)
            {
                entityMap[i].update(theGameTime);
            }
        }

        //Draw all entities
        public void drawAll(SpriteBatch theSpriteBatch)
        {
            for (int i = 0, len = entityMap.Count; i < len; i++)
            {
                entityMap[i].draw(theSpriteBatch);
            }
        }


        ///////////////////////////////////////////////////
        // GETTERS ////////////////////////////////////////
        ///////////////////////////////////////////////////

        //Return hero entity
        public Hero getHero()
        {
            return (Hero)entityMap.ElementAt(UIDs.ElementAt(heroUID)).Value;
        }

        public int getPlatformHeight()
        {
            return platformHeight;
        }

        public int getScreenWidth()
        {
            return screenWidth;
        }

        public int getScreenHeight()
        {
            return screenHeight;
        }

        public int getMaxEnemyCount()
        {
            return maxEnemyCount;
        }

        ///////////////////////////////////////////////////
        // COMBAT /////////////////////////////////////////
        ///////////////////////////////////////////////////

        //hit an entity in given position with given damage (via UID)
        public void damageEntity(int UID, float damage, Vector2 pos)
        {
            entityMap.ElementAt(UID).Value.beHit(damage, pos);
        }

        //hit an entity in given position with given damage (via entity reference)
        public void damageEntity(Entity entity, float damage, Vector2 pos)
        {
            entity.beHit(damage, pos);
        }

        //broadcast to all enemies that hero has attacked. Each entity will recieve damage if it is within range
        public void broadcastAttack(float damage, Vector2 origin)
        {
            for (int i = 0, len = entityMap.Count; i < len; i++)
            {
                if (Vector2.Distance(origin, entityMap.ElementAt(i).Value.getPos()) < 30.0f)
                {
                    entityMap.ElementAt(i).Value.beHit(damage, origin);
                }
            }
        }

        public void reduceMaxEnemyCount(int reduction = 1)
        {
            maxEnemyCount -= reduction;
        }
    }
}
