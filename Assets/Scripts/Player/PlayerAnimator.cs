using System.Collections;
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
        Debug.Log("Player bene");
    }

    public void QteFail()
    {
        Debug.Log("Player faila");
    }

    public void QteOnHit()
    {
        Debug.Log("Player hittato");

    }

    public IEnumerator QteAttack()
    {   
        Debug.Log("Player attacca");
        yield return new WaitForSeconds(1f);
    }

    public void QteStart()
    {
        Debug.Log("Player partito");

    }
}
