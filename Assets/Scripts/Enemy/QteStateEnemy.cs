using System.Collections;
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
            this.stateAI.NavMeshAgent.velocity = Vector3.zero;
            QTEManager.Instance.StartQteEvent(this, stateAI.QteSequence);
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
            stateAI.StateMachine.SetState(new DedStateEnemy(this.stateAI));
        }

        public void QteOnHit()
        {
            this.stateAI.enemyAnimation.StartHit();
        }

        public IEnumerator QteAttack()
        {
            yield return this.stateAI.enemyAnimation.Attack();
        }

        public void QteStart(IQtePlayer enemy)
        {
            Vector3 lookDir = enemy.GetTransform().position - this.stateAI.transform.position;
            lookDir.y = 0.0f;
            this.stateAI.transform.rotation = Quaternion.LookRotation(lookDir);

        }

        public bool CanAttackPlayer()
        {
            return false;
        }

        public Transform GetTransform()
        {
            return this.stateAI.transform;
        }
    }
}