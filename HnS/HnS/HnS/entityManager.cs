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
        int nextUID = 0;
        public List<int> UIDs = new List<int>();
        Dictionary<int, Entity> entityMap = new Dictionary<int, Entity>();
        Entity temp = new Entity(Vector2.Zero);

        public void createHero(Vector2 pos)
        {
            Hero hero = new Hero(pos);
            entityMap.Add(nextUID, hero);
            UIDs.Add(nextUID);
            nextUID++;
        }

        public void createEnemy(Vector2 pos)
        {
            Enemy enemy = new Enemy(pos);
            entityMap.Add(nextUID, enemy);
            UIDs.Add(nextUID);
            nextUID++;
        }

        public void updateAll(GameTime theGameTime)
        {
            for (int i = 0, len = entityMap.Count; i < len; i++)
            {
                entityMap[i].update(theGameTime);
            }
        }

        public void drawAll(SpriteBatch theSpriteBatch)
        {
            for (int i = 0, len = entityMap.Count; i < len; i++)
            {
                entityMap[i].draw(theSpriteBatch);
            }
        }

    }
}
