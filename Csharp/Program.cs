using System;
using System.Threading;
using Csharp.WestWorld;

namespace Csharp
{
    public class Program
    {
        static void Main(string[] args)
        {
            RunWestWorld();
        }

        static void RunWestWorld()
        {
            var Bob = new Miner(EntityNamesEnum.MinerBob, "Bob");
            var Elsa = new MinersWife(EntityNamesEnum.Elsa, "Elsa");
            GameManager.EntityMgr().RegisterEntity(Bob);
            GameManager.EntityMgr().RegisterEntity(Elsa);

            for (int i = 0; i < 30; i++)
            {
                Bob.Update();
                Elsa.Update();

                GameManager.Dispatch().DisplayDelayedMessages();
                Thread.Sleep(300);
            }

            Console.ReadKey();
        }
    }
}
