using Unity.VisualScripting;
using UnityEngine;

namespace Enemy
{
    public interface IBehaviourState
    {

        void Start();
        void End();
        void Update();
        //void OnCollide(Collision collision){}
        bool CanAttackPlayer()
        {
            return true;
        }
    }
}