using System.Collections;

namespace QTESystem
{
    public interface IQtePlayer
    {
        void QteStart();
        
        //attacchiamo il nemico e si mette in guardia
        IEnumerator QteAttack();

        void QteSuccess();
        
        void QteFail();

        void QteOnHit();
    }
}