namespace Enemy
{
    public class BehaviourStateMachine
    {
        private IBehaviourState currentState;

        public IBehaviourState CurrentState
        {
            get => currentState;
        }

        public void SetState(IBehaviourState state)
        {
            currentState?.End();
            currentState = state;
            currentState.Start();
        }

        public void Update()
        {
            currentState?.Update();
        }
        
        
        
    }
}