using Unity.VisualScripting;

namespace Enemy
{
    public interface IBehaviourState
    {

        void Start();
        void End();
        void Update();

    }
}