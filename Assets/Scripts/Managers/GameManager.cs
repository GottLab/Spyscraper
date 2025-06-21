using UnityEngine;

public class GameManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status => ManagerStatus.Started;


    private bool isGameStopped = false;
    private float prevTimeScale = 1.0f;

    public void Startup()
    {

    }

    public void SetGameStopped(bool stop)
    {
        if (stop)
        {
            isGameStopped = true;
            this.prevTimeScale = Time.timeScale;
            Time.timeScale = 0.0f;
        }
        else
        {
            isGameStopped = false;
            Time.timeScale = this.prevTimeScale;
        }
    }

    public float GetUnscaledDeltaTime()
    {
        return this.isGameStopped ? 0.0f : Time.unscaledDeltaTime;
    }
}
