namespace Csharp.WestWorld
{
    public class BarFly : BaseGameEntity
    {
        private readonly StateMachine<BarFly> _stateMachine;
        public readonly string Name;

        public BarFly(EntityNamesEnum id, string name) : base(id)
        {
            Name = name;
            _stateMachine = new StateMachine<BarFly>(this);
            _stateMachine.SetCurrentState(SleepingAtSaloon.GetInstance());
            _stateMachine.SetGlobalState(BarFlyGlobalState.GetInstance());
        }

        public override bool HandleMessage(Telegram message)
        {
            return _stateMachine.HandleMessage(message);
        }

        public override void Update()
        {
            _stateMachine.Update();
        }

        public StateMachine<BarFly> GetFSM()
        {
            return _stateMachine;
        }
    }
}
