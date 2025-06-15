namespace Enemy
{
    public abstract class AbstractBehaviourState : IBehaviourState
    {
        
        protected BehaviourStateMachine StateMachine;
        
        protected AbstractBehaviourState(BehaviourStateMachine behaviourStateMachine)
        {
            StateMachine = behaviourStateMachine;
        }
        
        public abstract void Start();
        public abstract void Update();
        public abstract void End();
        
    }
}