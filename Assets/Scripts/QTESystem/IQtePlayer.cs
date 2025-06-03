namespace QTESystem
{
    public interface IQtePlayer
    {
        void QteStart();
        
        void QteStop();

        void QteSuccess();
        
        void QteFail();

        void QteOnHit();
    }
}