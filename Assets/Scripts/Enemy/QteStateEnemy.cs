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
            Debug.Log("Nemico faila");

        }

        public void QteOnHit()
        {
            Debug.Log("Nemico hittato");

        }

        public IEnumerator QteAttack()
        {
            Debug.Log("Nemico attacca");
            yield return new WaitForSeconds(3f);
        }

        public void QteStart(IQtePlayer enemy)
        {
            Debug.Log("Nemico starta");

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