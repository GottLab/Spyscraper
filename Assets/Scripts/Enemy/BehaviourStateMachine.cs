using UnityEngine;

namespace Enemy
{
    public class BehaviourStateMachine
    {
        private IBehaviourState currentState;

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