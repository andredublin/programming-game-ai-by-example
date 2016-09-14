namespace Csharp.WestWorld
{
    public class Miner : BaseGameEntity
    {
        public const int ComfortLevel = 5;
        private const int _maxNuggets = 3;
        private const int _thirstLevel = 5;
        private const int _tirednessThreshold = 5;

        private readonly StateMachine<Miner> _stateMachine;
        public readonly string Name;

        private int _thrist;

        public Location Location { get; private set; }
        public int GoldCarried { get; private set; }
        public int MoneyInBank { get; private set; }
        public int Fatigue { get; private set; }
        public bool IsInFight { get; private set; }

        public Miner(EntityNamesEnum id, string name) : base(id)
        {
            Name = name;
            Location = Location.shack;
            GoldCarried = 0;
            MoneyInBank = 0;
            _thrist = 0;
            Fatigue = 0;
            IsInFight = false;
            _stateMachine = new StateMachine<Miner>(this);
            _stateMachine.SetCurrentState(GoHomeAndSleepTilRested.GetInstance());
        }

        public override void Update()
        {
            _thrist = _thrist + 1;
            _stateMachine.Update();
        }

        public override bool HandleMessage(Telegram message)
        {
            return _stateMachine.HandleMessage(message);
        }

        public StateMachine<Miner> GetFSM()
        {
            return _stateMachine;
        }

        public void ChangeLocation(Location newLocation)
        {
            Location = newLocation;
        }

        public void AddToGoldCarried(int amount)
        {
            GoldCarried = GoldCarried + amount;
        }

        public void AddToWealth(int amount)
        {
            MoneyInBank = MoneyInBank + amount;
            GoldCarried = 0;
        }

        public void SetWealth(int amount)
        {
            MoneyInBank = amount;
        }

        public bool PocketsFull()
        {
            return GoldCarried >= _maxNuggets;
        }

        public bool Fatigued()
        {
            return Fatigue >= _tirednessThreshold;
        }

        public void IncreaseFatigue()
        {
            Fatigue = Fatigue + 1;
        }

        public void DecreaseFatigue()
        {
            Fatigue = Fatigue - 1;
        }

        public bool Thirsty()
        {
            return _thrist >= _thirstLevel;
        }

        public void BuyAndDrinkWhiskey()
        {
            _thrist = 0;
            MoneyInBank = MoneyInBank - 2;
        }

        public void SetInAFight(bool val)
        {
            IsInFight = val;
        }
    }
}
