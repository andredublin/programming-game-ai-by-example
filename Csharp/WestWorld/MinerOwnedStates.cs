using System;

namespace Csharp.WestWorld
{
    //------------------------------------------------------------------------
    //
    //  In this state the miner will walk to a goldmine and pick up a nugget
    //  of gold. If the miner already has a nugget of gold he'll change state
    //  to VisitBankAndDepositGold. If he gets thirsty he'll change state
    //  to QuenchThirst
    //------------------------------------------------------------------------
    public class EnterMineAndDigForNuggetState : State<Miner>
    {
        private EnterMineAndDigForNuggetState() { }
        private static EnterMineAndDigForNuggetState _instance;

        public static EnterMineAndDigForNuggetState GetInstance()
        {
            if (_instance == null)
            {
                _instance = new EnterMineAndDigForNuggetState();
            }

            return _instance;
        }

        public override void Enter(Miner entity)
        {
            if (entity.Location != Location.Goldmine)
            {
                Console.WriteLine($"{entity.Name}: Walking to the gold mine");
                entity.ChangeLocation(Location.Goldmine);
            }
        }

        public override void Execute(Miner entity)
        {
            entity.AddToGoldCarried(1);
            entity.IncreaseFatigue();
            Console.WriteLine($"{entity.Name}: Pickin' up a nugget");

            if (entity.PocketsFull())
            {
                entity.GetFsm().ChangeState(VisitBankAndDepositGold.GetInstance());
            }

            if (entity.Thirsty())
            {
                entity.GetFsm().ChangeState(QuenchThirst.GetInstance());
            }
        }

        public override void Exit(Miner entity)
        {
            Console.WriteLine($"{entity.Name}: Ah'm leavin' the gold mine with mah pockets full o' sweet gold");
        }

        public override bool OnMessage(Miner entity, Telegram message)
        {
            return false;
        }
    }

    //------------------------------------------------------------------------
    //
    //  Entity will go to a bank and deposit any nuggets he is carrying. If the 
    //  miner is subsequently wealthy enough he'll walk home, otherwise he'll
    //  keep going to get more gold
    //------------------------------------------------------------------------
    public class VisitBankAndDepositGold : State<Miner>
    {
        private VisitBankAndDepositGold() { }
        private static VisitBankAndDepositGold _instance;

        public static VisitBankAndDepositGold GetInstance()
        {
            if (_instance == null)
            {
                _instance = new VisitBankAndDepositGold();
            }

            return _instance;
        }

        public override void Enter(Miner entity)
        {
            if (entity.Location != Location.Bank)
            {
                Console.WriteLine($"{entity.Name}: Goin' to the bank. Yes siree");
            }

            entity.ChangeLocation(Location.Bank);
        }

        public override void Execute(Miner entity)
        {
            // deposit gold
            entity.AddToWealth(entity.GoldCarried);
            Console.WriteLine($"{entity.Name}: Depositing gold. Total savings now: {entity.MoneyInBank}");

            if (entity.MoneyInBank >= Miner.ComfortLevel)
            {
                Console.WriteLine($"{entity.Name}: WooHoo! Rich enough for now. Back home to mah li'lle lady");
                entity.GetFsm().ChangeState(GoHomeAndSleepTilRested.GetInstance());
            }
            else
            {
                entity.GetFsm().ChangeState(EnterMineAndDigForNuggetState.GetInstance());
            }
        }

        public override void Exit(Miner entity)
        {
            Console.WriteLine($"{entity.Name}: Leavin' the bank");
        }

        public override bool OnMessage(Miner entity, Telegram message)
        {
            return false;
        }
    }

    //------------------------------------------------------------------------
    //
    //  miner will go home and sleep until his fatigue is decreased
    //  sufficiently
    //------------------------------------------------------------------------
    public class GoHomeAndSleepTilRested : State<Miner>
    {
        private GoHomeAndSleepTilRested() { }
        private static GoHomeAndSleepTilRested _instance;

        public static GoHomeAndSleepTilRested GetInstance()
        {
            if (_instance == null)
            {
                _instance = new GoHomeAndSleepTilRested();
            }

            return _instance;
        }

        public override void Enter(Miner entity)
        {
            if (entity.Location != Location.Shack)
            {
                Console.WriteLine($"{entity.Name}: Walkin' home");
                GameManager.Dispatch().DispatchMessage(
                    MessageDispatcher.SendMessageImmediately,
                    entity.Id,
                    EntityNamesEnum.Elsa,
                    MessageTypeEnum.HiHoneyImHome,
                    null);
                entity.ChangeLocation(Location.Shack);
            }
        }

        public override void Execute(Miner entity)
        {
            if (!entity.Fatigued())
            {
                Console.WriteLine($"{entity.Name}: All mah fatigue has drained away. Time to find more gold!");
                entity.GetFsm().ChangeState(EnterMineAndDigForNuggetState.GetInstance());
            }
            else
            {
                entity.DecreaseFatigue();
                Console.WriteLine($"{entity.Name}: ZZZZ...");
            }
        }

        public override void Exit(Miner entity)
        {
        }

        public override bool OnMessage(Miner entity, Telegram message)
        {
            switch (message.Message)
            {
                case MessageTypeEnum.StewReady:
                    Console.WriteLine($"Message handled by {entity.Name} at time: {DateTime.UtcNow.Ticks}");
                    Console.WriteLine($"{entity.Name}: Okay Hun, ahm a comin'!");
                    entity.GetFsm().ChangeState(EatStew.GetInstance());
                    return true;
                default:
                    return false;
            }
        }
    }

    //------------------------------------------------------------------------
    //
    //  miner changes location to the saloon and keeps buying Whiskey until
    //  his thirst is quenched. When satisfied he returns to the goldmine
    //  and resumes his quest for nuggets.
    //------------------------------------------------------------------------
    public class QuenchThirst : State<Miner>
    {
        private QuenchThirst() { }
        private static QuenchThirst _instance;

        public static QuenchThirst GetInstance()
        {
            if (_instance == null)
            {
                _instance = new QuenchThirst();
            }

            return _instance;
        }

        public override void Enter(Miner entity)
        {
            if (entity.Location != Location.Saloon)
            {
                entity.ChangeLocation(Location.Saloon);
                Console.WriteLine($"{entity.Name}: Boy, ah sure is thusty! Walking to the saloon");

                GameManager.Dispatch().DispatchMessage(
                    MessageDispatcher.SendMessageImmediately,
                    entity.Id,
                    EntityNamesEnum.BarFly,
                    MessageTypeEnum.MinerEnteredTheBar,
                    null);
            }
        }

        public override void Execute(Miner entity)
        {
            if (!entity.IsInFight)
            {
                entity.BuyAndDrinkWhiskey();
                Console.WriteLine($"{entity.Name}: That's mighty fine sippin' liquer");
                entity.GetFsm().ChangeState(EnterMineAndDigForNuggetState.GetInstance());
            }
        }

        public override void Exit(Miner entity)
        {
            Console.WriteLine($"{entity.Name}: Leaving the saloon, feelin' good");
        }

        public override bool OnMessage(Miner entity, Telegram message)
        {
            switch (message.Message)
            {
                case MessageTypeEnum.LetsDanceBuddy:
                    Console.WriteLine($"{entity.Name}: This here varmint is tryin' to take me fer all my gold!");
                    entity.SetInAFight(true);
                    return true;
                case MessageTypeEnum.YouThrowAMeanHook:
                    Console.WriteLine($"{entity.Name}: Next time pick on someone your own size");
                    entity.SetInAFight(false);
                    return true;
                default:
                    return false;
            }
        }
    }

    //------------------------------------------------------------------------EatStew
    public class EatStew : State<Miner>
    {
        private EatStew() { }
        private static EatStew _instance;

        public static EatStew GetInstance()
        {
            if (_instance == null)
            {
                _instance = new EatStew();
            }

            return _instance;
        }

        public override void Enter(Miner entity)
        {
            Console.WriteLine($"{entity.Name}: Smells Reaaal good Elsa!");
        }

        public override void Execute(Miner entity)
        {
            Console.WriteLine($"{entity.Name}: Tastes real good too!");
            entity.GetFsm().RevertToPreviousState();
        }

        public override void Exit(Miner entity)
        {
            Console.WriteLine($"{entity.Name}: Thankya li'lle lady. Ah better get back to whatever ah wuz doin");
        }

        public override bool OnMessage(Miner entity, Telegram message)
        {
            return false;
        }
    }
}
