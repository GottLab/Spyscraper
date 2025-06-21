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
        KeyCode.A,
        KeyCode.B,
        KeyCode.C,
        KeyCode.D,
        KeyCode.E,
        KeyCode.F,
        KeyCode.G,
        KeyCode.H,
        KeyCode.I,
        KeyCode.J,
        KeyCode.K,
        KeyCode.L,
        KeyCode.M,
        KeyCode.N,
        KeyCode.O,
        KeyCode.P,
        KeyCode.Q,
        KeyCode.R,
        KeyCode.S,
        KeyCode.T,
        KeyCode.U,
        KeyCode.V,
        KeyCode.W,
        KeyCode.X,
        KeyCode.Y,
        KeyCode.Z,
        KeyCode.Space // Anche la barra spaziatrice è un'opzione comune
    };


    [SerializeField]
    private float slowMoScale = 0.2f;

    [SerializeField]
    private PlayerAnimator mainPlayer;

    public delegate void QteElementStart(KeyCode keyCode, float time);
    public event QteElementStart OnQteElementStart;
    public delegate void QteElementEnd(bool success);
    public event QteElementEnd OnQteElementEnd;

    public delegate void QteEvent(IQtePlayer target);
    public static event QteEvent OnQteSequenceStart;
    public static event QteEvent OnQteSequenceEnd;

    private bool isQtetting = false;

    //describes if the given key was pressed last fram
    private Dictionary<KeyCode, bool> pressedKeys = new();

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

        Time.timeScale = slowMoScale;

        float timer = 0;
        bool success = false;

        KeyCode inputKey = possibleQteKeys[Random.Range(0, possibleQteKeys.Count)];

        OnQteElementStart?.Invoke(inputKey, element.time);

        while (timer < element.time)
        {
            if (TotalKeyPressed() > 0)
            {
                success = Input.GetKeyDown(inputKey);
                break;
            }

            timer += Managers.game.GetUnscaledDeltaTime();
            yield return null;
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


    private int TotalKeyPressed()
    {
        int total = 0;
        foreach (KeyCode key in possibleQteKeys)
        {
            bool pressed = Input.GetKeyDown(key);
            if (pressed)
                total += 1;
        }
        return total;
    }
}
