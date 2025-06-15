using System.Collections;
using UnityEngine;

namespace QTESystem
{
    public interface IQtePlayer
    {
        void QteStart(IQtePlayer other);

        //attacchiamo il nemico e si mette in guardia
        IEnumerator QteAttack();

        void QteSuccess();

        void QteFail();

        void QteOnHit();

        Transform GetTransform()
        {
            if (this is MonoBehaviour monoBehaviour)
            {
                return monoBehaviour.transform;
            }
            return null;
        }
    }
}