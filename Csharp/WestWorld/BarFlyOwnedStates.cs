using System;

namespace Csharp.WestWorld
{
    public class BarFlyGlobalState : State<BarFly>
    {
        private BarFlyGlobalState() { }
        private static BarFlyGlobalState _instance;

        public static BarFlyGlobalState GetInstance()
        {
            if (_instance == null)
            {
                _instance = new BarFlyGlobalState();
            }

            return _instance;
        }

        public override void Enter(BarFly entity)
        {
        }

        public override void Execute(BarFly entity)
        {
        }

        public override void Exit(BarFly entity)
        {
        }

        public override bool OnMessage(BarFly entity, Telegram message)
        {
            switch (message.Message)
            {
                case MessageTypeEnum.MinerEnteredTheBar:
                    var rand = new Random();

                    // 5 in 10 chance of getting into a fight
                    if (rand.Next(0, 9) < 4 && !entity.GetFsm().IsInState(FightWithMiner.GetInstance()))
                    {
                        entity.GetFsm().ChangeState(FightWithMiner.GetInstance());
                    }
                    return true;
            }

            return false;
        }
    }

    public class SleepingAtSaloon : State<BarFly>
    {
        private SleepingAtSaloon() { }
        private static SleepingAtSaloon _instance;

        public static SleepingAtSaloon GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SleepingAtSaloon();
            }

            return _instance;
        }

        public override void Enter(BarFly entity)
        {
        }

        public override void Execute(BarFly entity)
        {
        }

        public override void Exit(BarFly entity)
        {
        }

        public override bool OnMessage(BarFly entity, Telegram message)
        {
            return false;
        }
    }

    public class FightWithMiner : State<BarFly>
    {
        private FightWithMiner() { }
        private static FightWithMiner _instance;

        public static FightWithMiner GetInstance()
        {
            if (_instance == null)
            {
                _instance = new FightWithMiner();
            }

            return _instance;
        }

        public override void Enter(BarFly entity)
        {
            Console.WriteLine($"{entity.Name}: I don't like the way ya look buddy");
            GameManager.Dispatch().DispatchMessage(
                MessageDispatcher.SendMessageImmediately,
                entity.Id,
                EntityNamesEnum.MinerBob,
                MessageTypeEnum.LetsDanceBuddy,
                null);
        }

        public override void Execute(BarFly entity)
        {
            Console.WriteLine($"{entity.Name}: Take that and that and some of these!");
            entity.GetFsm().RevertToPreviousState();
        }

        public override void Exit(BarFly entity)
        {
            GameManager.Dispatch().DispatchMessage(
                MessageDispatcher.SendMessageImmediately,
                entity.Id,
                EntityNamesEnum.MinerBob,
                MessageTypeEnum.YouThrowAMeanHook,
                null);
        }

        public override bool OnMessage(BarFly entity, Telegram message)
        {
            return false;
        }
    }
}
