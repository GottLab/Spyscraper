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
            return Managers.playerManager.IsState(PlayerManager.PlayerState.NORMAL) && !Managers.game.IsChangingScene;
        }
    }
}