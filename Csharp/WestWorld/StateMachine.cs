namespace Csharp.WestWorld
{
    public class StateMachine<T>
    {
        private T _instanceOwner;
        public State<T> CurrentState { get; private set; }
        public State<T> PreviousState { get; private set; }
        public State<T> GlobalState { get; private set; }

        public StateMachine(T instanceOwner)
        {
            _instanceOwner = instanceOwner;
        }

        public void SetCurrentState(State<T> state)
        {
            CurrentState = state;
        }

        public void SetGlobalState(State<T> state)
        {
            GlobalState = state;
        }

        public void SetPreviousState(State<T> state)
        {
            PreviousState = state;
        }

        // Updates the FSM
        public void Update()
        {
            if (GlobalState != null)
            {
                GlobalState.Execute(_instanceOwner);
            }

            if (CurrentState != null)
            {
                CurrentState.Execute(_instanceOwner);
            }
        }

        public bool HandleMessage(Telegram message)
        {
            if (CurrentState != null && CurrentState.OnMessage(_instanceOwner, message))
            {
                return true;
            }

            if (CurrentState != null && GlobalState.OnMessage(_instanceOwner, message))
            {
                return true;
            }

            return false;
        }

        public void ChangeState(State<T> newState)
        {
            PreviousState = CurrentState;
            CurrentState.Exit(_instanceOwner);
            CurrentState = newState;
            CurrentState.Enter(_instanceOwner);
        }

        public void RevertToPreviousState()
        {
            ChangeState(PreviousState);
        }

        public bool IsInState(State<T> state)
        {
            return CurrentState == state;
        }

        public string GetNameOfCurrentState()
        {
            return nameof(CurrentState);
        }
    }
}
