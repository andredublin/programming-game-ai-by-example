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
            if (entity.Location != Location.goldmine)
            {
                Console.WriteLine(string.Format("{0}: Walking to the gold mine", entity.Name));
                entity.ChangeLocation(Location.goldmine);
            }
        }

        public override void Execute(Miner entity)
        {
            entity.AddToGoldCarried(1);
            entity.IncreaseFatigue();
            Console.WriteLine(string.Format("{0}: Pickin' up a nugget", entity.Name));

            if (entity.PocketsFull())
            {
                entity.GetFSM().ChangeState(VisitBankAndDepositGold.GetInstance());
            }

            if (entity.Thirsty())
            {
                entity.GetFSM().ChangeState(QuenchThirst.GetInstance());
            }
        }

        public override void Exit(Miner entity)
        {
            Console.WriteLine(
                string.Format("{0}: Ah'm leavin' the gold mine with mah pockets full o' sweet gold", entity.Name));
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
            if (entity.Location != Location.bank)
            {
                Console.WriteLine(string.Format("{0}: Goin' to the bank. Yes siree", entity.Name));
            }

            entity.ChangeLocation(Location.bank);
        }

        public override void Execute(Miner entity)
        {
            // deposit gold
            entity.AddToWealth(entity.GoldCarried);
            Console.WriteLine(
                string.Format("{0}: Depositing gold. Total savings now: {1}", entity.Name, entity.MoneyInBank));

            if (entity.MoneyInBank >= Miner.ComfortLevel)
            {
                Console.WriteLine(
                    string.Format("{0}: WooHoo! Rich enough for now. Back home to mah li'lle lady", entity.Name));
                entity.GetFSM().ChangeState(GoHomeAndSleepTilRested.GetInstance());
            }
            else
            {
                entity.GetFSM().ChangeState(EnterMineAndDigForNuggetState.GetInstance());
            }
        }

        public override void Exit(Miner entity)
        {
            Console.WriteLine(string.Format("{0}: Leavin' the bank", entity.Name));
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
            if (entity.Location != Location.shack)
            {
                Console.WriteLine(string.Format("{0}: Walkin' home", entity.Name));
                GameManager.Dispatch().DispatchMessage(
                    MessageDispatcher.SEND_MESSAGE_IMMEDIATELY,
                    entity.Id,
                    EntityNamesEnum.Elsa,
                    MessageTypeEnum.HiHoneyImHome,
                    null);
                entity.ChangeLocation(Location.shack);
            }
        }

        public override void Execute(Miner entity)
        {
            if (!entity.Fatigued())
            {
                Console.WriteLine(
                    string.Format("{0}: All mah fatigue has drained away. Time to find more gold!", entity.Name));
                entity.GetFSM().ChangeState(EnterMineAndDigForNuggetState.GetInstance());
            }
            else
            {
                entity.DecreaseFatigue();
                Console.WriteLine(string.Format("{0}: ZZZZ...", entity.Name));
            }
        }

        public override void Exit(Miner entity)
        {
        }

        public override bool OnMessage(Miner entity, Telegram message)
        {
            switch(message.Message)
            {
                case MessageTypeEnum.StewReady:
                    Console.WriteLine(
                        string.Format("Message handled by {0} at time: {1}", entity.Name, DateTime.UtcNow.Ticks));
                    Console.WriteLine(string.Format("{0}: Okay Hun, ahm a comin'!", entity.Name));
                    entity.GetFSM().ChangeState(EatStew.GetInstance());
                    return true;
            }

            return false;
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
            if (entity.Location != Location.saloon)
            {
                entity.ChangeLocation(Location.saloon);
                Console.WriteLine(string.Format("{0}: Boy, ah sure is thusty! Walking to the saloon", entity.Name));

                GameManager.Dispatch().DispatchMessage(
                    MessageDispatcher.SEND_MESSAGE_IMMEDIATELY,
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
                Console.WriteLine(string.Format("{0}: That's mighty fine sippin' liquer", entity.Name));
                entity.GetFSM().ChangeState(EnterMineAndDigForNuggetState.GetInstance());
            }
        }

        public override void Exit(Miner entity)
        {
            Console.WriteLine(string.Format("{0}: Leaving the saloon, feelin' good", entity.Name));
        }

        public override bool OnMessage(Miner entity, Telegram message)
        {
            switch (message.Message)
            {
                case MessageTypeEnum.LetsDanceBuddy:
                    Console.WriteLine(string.Format("{0}: This here varmint is tryin' to take me fer all my gold!", entity.Name));
                    entity.SetInAFight(true);
                    return true;
                case MessageTypeEnum.YouThrowAMeanHook:
                    Console.WriteLine(string.Format("{0}: Next time pick on someone your own size", entity.Name));
                    entity.SetInAFight(false);
                    return true;
            }

            return false;
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
            Console.WriteLine(string.Format("{0}: Smells Reaaal good Elsa!", entity.Name));
        }

        public override void Execute(Miner entity)
        {
            Console.WriteLine(string.Format("{0}: Tastes real good too!", entity.Name));
            entity.GetFSM().RevertToPreviousState();
        }

        public override void Exit(Miner entity)
        {
            Console.WriteLine(
                string.Format("{0}: Thankya li'lle lady. Ah better get back to whatever ah wuz doin", entity.Name));
        }

        public override bool OnMessage(Miner entity, Telegram message)
        {
            return false;
        }
    }
}
