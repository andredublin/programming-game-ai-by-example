using System;

namespace Csharp.WestWorld
{
    public class MinersWife : BaseGameEntity
    {
        private StateMachine<MinersWife> _stateMachine;

        public readonly string Name;

        public Location Location { get; private set; }
        public bool IsCooking { get; private set; }

        public MinersWife(EntityNamesEnum id, string name) : base(id)
        {
            Name = name;
            IsCooking = false;
            _stateMachine = new StateMachine<MinersWife>(this);
            _stateMachine.SetCurrentState(DoHouseWork.GetInstance());
            _stateMachine.SetGlobalState(WifesGlobalState.GetInstance());
        }

        ~MinersWife()
        {
            _stateMachine = null;
        }

        public override void Update()
        {
            _stateMachine.Update();
        }

        public override bool HandleMessage(Telegram message)
        {
            return _stateMachine.HandleMessage(message);
        }

        public StateMachine<MinersWife> GetFSM()
        {
            return _stateMachine;
        }

        public void ChangeLocation(Location newLocation)
        {
            Location = newLocation;
        }

        public void SetCooking(bool isCooking)
        {
            IsCooking = isCooking;
        }
    }
}
