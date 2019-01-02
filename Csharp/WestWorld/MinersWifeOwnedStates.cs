using System;

namespace Csharp.WestWorld
{
    public class WifesGlobalState : State<MinersWife>
    {
        private WifesGlobalState() { }
        private static WifesGlobalState _instance;

        public static WifesGlobalState GetInstance()
        {
            if (_instance == null)
            {
                _instance = new WifesGlobalState();
            }

            return _instance;
        }

        public override void Enter(MinersWife entity)
        {
        }

        public override void Execute(MinersWife entity)
        {
            var rand = new Random();
            ;
            // 1 in 10 chance of needing the bathroom (provided she is not already in the bathroom)
            if (rand.Next(0, 9) < 1 && !entity.GetFsm().IsInState(VisitBathroom.GetInstance()))
            {
                entity.GetFsm().ChangeState(VisitBathroom.GetInstance());
            }
        }

        public override void Exit(MinersWife entity)
        {
        }

        public override bool OnMessage(MinersWife entity, Telegram message)
        {
            switch (message.Message)
            {
                case MessageTypeEnum.HiHoneyImHome:
                    Console.WriteLine($"Message handled by {entity.Name} at time: {DateTime.UtcNow.Ticks}");
                    Console.WriteLine($"{entity.Name}: Hi honey. Let me make you some of mah fine country stew");
                    entity.GetFsm().ChangeState(CookStew.GetInstance());
                    return true;
            }

            return false;
        }
    }

    public class DoHouseWork : State<MinersWife>
    {
        private DoHouseWork() { }
        private static DoHouseWork _instance;

        public static DoHouseWork GetInstance()
        {
            if (_instance == null)
            {
                _instance = new DoHouseWork();
            }

            return _instance;
        }

        public override void Enter(MinersWife entity)
        {
            Console.WriteLine($"{entity.Name}: Time to do some more housework!");
        }

        public override void Execute(MinersWife entity)
        {
            var rand = new Random();

            switch(rand.Next(0, 2))
            {
                case 0:
                    Console.WriteLine($"{entity.Name}: Moppin' the floor");
                    break;
                case 1:
                    Console.WriteLine($"{entity.Name}: Washin' the dishes");
                    break;
                case 2:
                    Console.WriteLine($"{entity.Name}: Makin' the bed");
                    break;
            }
        }

        public override void Exit(MinersWife entity)
        {
        }

        public override bool OnMessage(MinersWife entity, Telegram message)
        {
            return false;
        }
    }

    public class VisitBathroom : State<MinersWife>
    {
        private VisitBathroom() { }
        private static VisitBathroom _instance;

        public static VisitBathroom GetInstance()
        {
            if (_instance == null)
            {
                _instance = new VisitBathroom();
            }

            return _instance;
        }

        public override void Enter(MinersWife entity)
        {
            Console.WriteLine($"{entity.Name}: Walkin' to the can. Need to powda mah pretty li'lle nose");
        }

        public override void Execute(MinersWife entity)
        {
            Console.WriteLine($"{entity.Name}: Ahhhhhh! Sweet relief!");
            entity.GetFsm().RevertToPreviousState();
        }

        public override void Exit(MinersWife entity)
        {
            Console.WriteLine($"{entity.Name}: Leavin' the Jon");
        }

        public override bool OnMessage(MinersWife entity, Telegram message)
        {
            return false;
        }
    }

    public class CookStew : State<MinersWife>
    {
        private CookStew() { }
        private static CookStew _instance;

        public static CookStew GetInstance()
        {
            if (_instance == null)
            {
                _instance = new CookStew();
            }

            return _instance;
        }
        public override void Enter(MinersWife entity)
        {
            if (!entity.IsCooking)
            {
                Console.WriteLine($"{entity.Name}: Putting the stew in the oven");
                // send a delayed message myself so that I know when to take the stew out of the oven
                GameManager.Dispatch().DispatchMessage(2, entity.Id, entity.Id, MessageTypeEnum.StewReady, null);
                entity.SetCooking(true);
            }
        }

        public override void Execute(MinersWife entity)
        {
            Console.WriteLine($"{entity.Name}: Fussin' over food");
        }

        public override void Exit(MinersWife entity)
        {
            Console.WriteLine($"{entity.Name}: Puttin' the stew on the table");
        }

        public override bool OnMessage(MinersWife entity, Telegram message)
        {
            switch(message.Message)
            {
                case MessageTypeEnum.StewReady:
                    Console.WriteLine($"Message received by {entity.Name} at time: {DateTime.UtcNow.Ticks}");
                    Console.WriteLine($"{entity.Name}: StewReady! Lets eat");
                    GameManager.Dispatch().DispatchMessage(
                        MessageDispatcher.SendMessageImmediately,
                        entity.Id,
                        EntityNamesEnum.MinerBob,
                        MessageTypeEnum.StewReady,
                        null);
                    entity.SetCooking(false);
                    entity.GetFsm().ChangeState(DoHouseWork.GetInstance());
                    return true;
            }

            return false;
        }
    }
}
