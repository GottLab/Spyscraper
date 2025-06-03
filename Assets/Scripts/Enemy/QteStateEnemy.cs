using QTESystem;
using UnityEngine;

namespace Enemy
{
    public class QteStateEnemy : IBehaviourState, IQtePlayer
    {
        
        private readonly StateEnemyAI stateAI;
        
        public QteStateEnemy(StateEnemyAI stateEnemyAI)
        {
            stateAI = stateEnemyAI;
        }
        
        public void Start()
        {
            QTEManager.Instance.StartQteEvent(this);
        }

        public void Update()
        {
        }

        public void End()
        {
        }
        
        public void QteSuccess()
        {
            stateAI.StateMachine.SetState(new PatrolStateEnemy(this.stateAI));
        }

        public void QteFail()
        {
        }

        public void QteOnHit()
        {
        }

        public void QteStop()
        {
        }

        public void QteStart()
        {
        }
    }
}