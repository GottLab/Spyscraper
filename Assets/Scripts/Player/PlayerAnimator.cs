using QTESystem;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour, IQtePlayer
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void QteSuccess()
    {
        Debug.Log("AFFANCULO");
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
