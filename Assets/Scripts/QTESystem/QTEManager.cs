using System;
using System.Collections;
using System.Collections.Generic;
using QTESystem;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class QTEManager : MonoBehaviour
{
    public enum QteType
    {
        Easy,
        Medium,
        Difficult
    }
    
    [Tooltip("Il set di tasti da cui scegliere casualmente per il QTE.")]
    private List<KeyCode> possibleQteKeys = new List<KeyCode>
    {
        KeyCode.Q,
        KeyCode.E,
        KeyCode.Z,
        KeyCode.X,
        KeyCode.R,
        KeyCode.C,
        KeyCode.G,
        KeyCode.T,
        KeyCode.F,
        KeyCode.Space // Anche la barra spaziatrice è un'opzione comune
    };
    
    
    [SerializeField]
    private float slowMoScale = 0.2f;
    
    [SerializeField]
    private PlayerAnimator mainPlayer;

    public delegate void QteElementStart(KeyCode keyCode, float time);
    public static event QteElementStart OnQteElementStart;
    public delegate void QteElementEnd(bool success);
    public static event QteElementEnd OnQteElementEnd;

    public delegate void QteEvent(IQtePlayer target);
    public static event QteEvent OnQteSequenceStart;
    public static event QteEvent OnQteSequenceEnd;
    
    private bool isQtetting = false;

    //this variable is used to avoid the situation when qte starts while already holding a key resulting in instant loss.
    private bool canTakeInput = false;
    
    public static QTEManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }
    
    private IEnumerator StartSingleQteEventCoroutine(IQtePlayer enemy, QteSequence.QteSequenceElement element, Action<bool> callback)
    {

        canTakeInput = false;
        Time.timeScale = slowMoScale;
        
        float timer = 0;
        bool success = false;
        
        KeyCode inputKey = possibleQteKeys[Random.Range(0, possibleQteKeys.Count)];

        OnQteElementStart?.Invoke(inputKey, element.time);

        while (timer < element.time)
        {


            if (Input.anyKeyDown && canTakeInput)
            {
                success = Input.GetKeyDown(inputKey);
                break;
            }

            timer += Time.unscaledDeltaTime;
            yield return null;
            
            if (!Input.anyKeyDown)
                canTakeInput = true;
        }
        Time.timeScale = 1f;
        OnQteElementEnd?.Invoke(success);
        
        
        if (success)
        {
            yield return StartCoroutine(mainPlayer.QteAttack());
            enemy.QteOnHit();
        }
        else
        {
            yield return StartCoroutine(enemy.QteAttack());
            mainPlayer.QteOnHit();
        }
        
        callback.Invoke(success);
    }

    public void StartQteEvent(IQtePlayer enemy, QteSequence qteSequence)
    {
        if (!isQtetting)
            StartCoroutine(StartQteSequenceEventCoroutine(enemy, qteSequence));
    }

    private IEnumerator StartQteSequenceEventCoroutine(IQtePlayer enemy, QteSequence qteSequence)
    {
        isQtetting = true;

        OnQteSequenceStart?.Invoke(enemy);
        mainPlayer.QteStart(enemy);
        enemy.QteStart(mainPlayer);
        
        bool sequenceSuccess = true;

        foreach (var element in qteSequence.sequence)
        {   
            yield return StartCoroutine(StartSingleQteEventCoroutine(enemy, element, success => sequenceSuccess = success));
            if (!sequenceSuccess)
                break;
        }

        if (sequenceSuccess)
        {
            mainPlayer.QteSuccess();
            enemy.QteFail();
        }
        else
        {
            mainPlayer.QteFail();
            enemy.QteSuccess();
        }
        OnQteSequenceEnd?.Invoke(enemy);
        
        isQtetting = false;
            
    }
}
