using System;
using System.Collections.Generic;

namespace Csharp.WestWorld
{
    public class EntityManager
    {
        private Dictionary<EntityNamesEnum, BaseGameEntity> _entityMap = 
            new Dictionary<EntityNamesEnum, BaseGameEntity>();

        private EntityManager() { }
        private static EntityManager _instance;

        public static EntityManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new EntityManager();
            }

            return _instance;
        }

        public void RegisterEntity(BaseGameEntity entity)
        {
            _entityMap.Add(entity.Id, entity);
        }

        public BaseGameEntity GetEntityFromId(EntityNamesEnum id)
        {
            BaseGameEntity entity;
            var result = _entityMap.TryGetValue(id, out entity);

            if (result)
            {
                return entity;
            }

            return null;
        }

        public void RemoveEntity(EntityNamesEnum id)
        {
            _entityMap.Remove(id);
        }
    }
}
