namespace Csharp.WestWorld
{
    public class Miner : BaseGameEntity
    {
        public const int ComfortLevel = 5;
        private const int MaxNuggets = 3;
        private const int ThirstLevel = 5;
        private const int TirednessThreshold = 5;

        private readonly StateMachine<Miner> _stateMachine;
        public readonly string Name;

        private int _thirst;

        public Location Location { get; private set; }
        public int GoldCarried { get; private set; }
        public int MoneyInBank { get; private set; }
        public int Fatigue { get; private set; }
        public bool IsInFight { get; private set; }

        public Miner(EntityNamesEnum id, string name) : base(id)
        {
            Name = name;
            Location = Location.Shack;
            GoldCarried = 0;
            MoneyInBank = 0;
            _thirst = 0;
            Fatigue = 0;
            IsInFight = false;
            _stateMachine = new StateMachine<Miner>(this);
            _stateMachine.SetCurrentState(GoHomeAndSleepTilRested.GetInstance());
        }

        public override void Update()
        {
            _thirst = _thirst + 1;
            _stateMachine.Update();
        }

        public override bool HandleMessage(Telegram message)
        {
            return _stateMachine.HandleMessage(message);
        }

        public StateMachine<Miner> GetFsm()
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
            return GoldCarried >= MaxNuggets;
        }

        public bool Fatigued()
        {
            return Fatigue >= TirednessThreshold;
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
            return _thirst >= ThirstLevel;
        }

        public void BuyAndDrinkWhiskey()
        {
            _thirst = 0;
            MoneyInBank = MoneyInBank - 2;
        }

        public void SetInAFight(bool val)
        {
            IsInFight = val;
        }
    }
}
