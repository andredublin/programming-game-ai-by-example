namespace Csharp.WestWorld
{
    public class GameManager
    {
        public static MessageDispatcher Dispatch()
        {
            return MessageDispatcher.GetInstance();
        }

        public static EntityManager EntityMgr()
        {
            return EntityManager.GetInstance();
        }
    }
}
