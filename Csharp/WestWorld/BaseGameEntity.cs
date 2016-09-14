using System;

namespace Csharp.WestWorld
{
    public abstract class BaseGameEntity
    {
        public EntityNamesEnum Id { get; private set; }

        public BaseGameEntity(EntityNamesEnum id)
        {
            Id = id;
        }

        public abstract void Update();
        public abstract bool HandleMessage(Telegram message);
    }
}
