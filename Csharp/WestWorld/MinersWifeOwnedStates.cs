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
            //1 in 10 chance of needing the bathroom (provided she is not already in the bathroom)
            if (rand.Next(0, 9) < 1 && !entity.GetFSM().IsInState(VisitBathroom.GetInstance()))
            {
                entity.GetFSM().ChangeState(VisitBathroom.GetInstance());
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
                    Console.WriteLine(
                        string.Format("Message handled by {0} at time: {1}", entity.Name, DateTime.UtcNow));
                    Console.WriteLine(
                        string.Format("{0}: Hi honey. Let me make you some of mah fine country stew", entity.Name));
                    entity.GetFSM().ChangeState(CookStew.GetInstance());
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
            Console.WriteLine(string.Format("{0}: Time to do some more housework!", entity.Name));
        }

        public override void Execute(MinersWife entity)
        {
            var rand = new Random();

            switch(rand.Next(0, 2))
            {
                case 0:
                    Console.WriteLine(string.Format("{0}: Moppin' the floor", entity.Name));
                    break;
                case 1:
                    Console.WriteLine(string.Format("{0}: Washin' the dishes", entity.Name));
                    break;
                case 2:
                    Console.WriteLine(string.Format("{0}: Makin' the bed", entity.Name));
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
            Console.WriteLine(string.Format("{0}: Walkin' to the can. Need to powda mah pretty li'lle nose", entity.Name));
        }

        public override void Execute(MinersWife entity)
        {
            Console.WriteLine(string.Format("{0}: Ahhhhhh! Sweet relief!", entity.Name));
            entity.GetFSM().RevertToPreviousState();
        }

        public override void Exit(MinersWife entity)
        {
            Console.WriteLine(string.Format("{0}: Leavin' the Jon", entity.Name));
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
                Console.WriteLine(string.Format("{0}: Putting the stew in the oven", entity.Name));

                // send a delayed message myself so that I know when to take the stew out of the oven
                GameManager.Dispatch().DispatchMessage(2, entity.Id, entity.Id, MessageTypeEnum.StewReady, null);
                entity.SetCooking(true);
            }
        }

        public override void Execute(MinersWife entity)
        {
            Console.WriteLine(string.Format("{0}: Fussin' over food", entity.Name));
        }

        public override void Exit(MinersWife entity)
        {
            Console.WriteLine(string.Format("{0}: Puttin' the stew on the table", entity.Name));
        }

        public override bool OnMessage(MinersWife entity, Telegram message)
        {
            switch(message.Message)
            {
                case MessageTypeEnum.StewReady:
                    Console.WriteLine(
                        string.Format("Message received by {0} at time: {1}", entity.Name, DateTime.UtcNow));
                    Console.WriteLine(string.Format("{0}: StewReady! Lets eat", entity.Name));
                    GameManager.Dispatch().DispatchMessage(
                        MessageDispatcher.SEND_MESSAGE_IMMEDIATELY,
                        entity.Id,
                        EntityNamesEnum.MinerBob,
                        MessageTypeEnum.StewReady,
                        null);
                    entity.SetCooking(false);
                    entity.GetFSM().ChangeState(DoHouseWork.GetInstance());
                    return true;
            }

            return false;
        }
    }
}
