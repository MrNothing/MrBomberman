using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4
{
    public class ViewTile
    {
        public Vector3 position;

        //the entities in the ViewTile...
        public Dictionary<string, Entity> entities = new Dictionary<string,Entity>();

        public ViewTile(Vector3 _position)
        {
            position = _position;
        }

        public void onEnterTile(Entity newEntity)
        {
            foreach (string s in entities.Keys)
            {
                try
                {
                    if (newEntity.type == EntityType.hero)
                    {
                        if (entities[s].visibleHeroes[newEntity.id] == null)
                            entities[s].visibleHeroes.Add(newEntity.id, newEntity.id);
                    }

                    if (entities[s].type == EntityType.hero)
                    {
                        if (newEntity.visibleHeroes[s] == null)
                            newEntity.visibleHeroes.Add(s, s);
                    }

                    if (entities[s].visibleUnits[newEntity.id] == null)
                        entities[s].visibleUnits.Add(newEntity.id, newEntity.id);

                    if (newEntity.visibleUnits[s] == null)
                        newEntity.visibleUnits.Add(s, s);

                    if (!entities[s].team.Equals(newEntity.team))
                    {
                        if (entities[s].visibleEnemies[newEntity.id] == null)
                            entities[s].visibleEnemies.Add(newEntity.id, newEntity.id);

                        if (newEntity.visibleEnemies[s] == null)
                            newEntity.visibleEnemies.Add(s, s);
                    }
                    else
                    {
                        if (entities[s].visibleAllies[newEntity.id] == null)
                            entities[s].visibleAllies.Add(newEntity.id, newEntity.id);

                        if (newEntity.visibleAllies[s] == null)
                            newEntity.visibleAllies.Add(s, s);
                    }
                }
                catch
                {
                    //this unit should not be here...
                }
            }  
        }

        public void onLeaveTile(Entity newEntity)
        {
            foreach (string s in entities.Keys)
            {
                try
                {
                    if (entities[s].visibleHeroes[newEntity.id] != null)
                        entities[s].visibleHeroes.Remove(newEntity.id);

                    if (entities[s].visibleUnits[newEntity.id] != null)
                        entities[s].visibleUnits.Remove(newEntity.id);
                    if (entities[s].visibleEnemies[newEntity.id] != null)
                        entities[s].visibleEnemies.Remove(newEntity.id);
                    if (entities[s].visibleAllies[newEntity.id] != null)
                        entities[s].visibleAllies.Remove(newEntity.id);

                    if (newEntity.visibleUnits[s] != null)
                        newEntity.visibleUnits.Remove(s);
                    if (newEntity.visibleEnemies[s] != null)
                        newEntity.visibleEnemies.Remove(s);
                    if (newEntity.visibleAllies[s] != null)
                        newEntity.visibleAllies.Remove(s);
                }
                catch
                {
                    //this unit should not be here...
                }
            }  
        }
    }
}
