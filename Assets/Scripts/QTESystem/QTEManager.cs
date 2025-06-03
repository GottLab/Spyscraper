using System.Collections;
using QTESystem;
using Unity.VisualScripting;
using UnityEngine;

public class QTEManager : MonoBehaviour
{
    [SerializeField]
    private float qteDuration = 2f;
    
    [SerializeField]
    private float slowMoScale = 0.2f;
    
    [SerializeField]
    private KeyCode inputKey = KeyCode.F;
    
    [SerializeField]
    private PlayerAnimator mainPlayer;
    
    private bool isQtetting = false;
    
    public static QTEManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }
    
    private IEnumerator StartQteEventCoroutine(IQtePlayer enemy)
    {   
        isQtetting = true;
        
        Time.timeScale = slowMoScale;
        
        mainPlayer.QteStart();
        enemy.QteStart();
        
        float timer = 0;
        bool success = false;
        
        
        while (timer < qteDuration)
        {
            if (Input.GetKeyDown(inputKey))
            {
                success = true;
                break;
            }

            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        
        Time.timeScale = 1f;
        
        if (success)
        {   
            mainPlayer.QteSuccess();
            enemy.QteFail();
        }
        else
        {
            mainPlayer.QteFail();
            enemy.QteSuccess();
        }
        
        isQtetting = false;
    }

    public void StartQteEvent(IQtePlayer enemy)
    {
        if (!isQtetting)
            StartCoroutine(StartQteEventCoroutine(enemy));
    }
}
