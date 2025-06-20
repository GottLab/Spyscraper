using UnityEngine;

public class QteUI : MonoBehaviour
{
    [SerializeField]
    private GameObject qteButtonPrefab;

    private QteButton qteButton;
    
    private void OnQteElementEnd(bool success)
    {
        qteButton.OnQteElementEnd(success);
        Destroy(qteButton.gameObject);
    }

    private void OnQteElementStart(KeyCode keyCode, float time)
    {
        qteButton = Instantiate(qteButtonPrefab, this.transform).GetComponent<QteButton>();
        
        qteButton.StartTimer(keyCode, time);
    }

    public void Start()
    {
        Debug.Log(QTEManager.Instance);
        QTEManager.Instance.OnQteElementStart += OnQteElementStart;
        QTEManager.Instance.OnQteElementEnd += OnQteElementEnd;
    }

    public void OnDestroy()
    {
        QTEManager.Instance.OnQteElementStart -= OnQteElementStart;
        QTEManager.Instance.OnQteElementEnd -= OnQteElementEnd;
    }
}
