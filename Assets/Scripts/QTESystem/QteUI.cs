using UnityEditor.EditorTools;
using UnityEngine;

public class QteUI : MonoBehaviour
{
    [SerializeField]
    private GameObject qteButtonPrefab;

    private QteButton qteButton;

    [SerializeField]
    private CanvasGroup background;

    private void OnQteElementEnd(bool success)
    {
        qteButton.OnQteElementEnd(success);
        Destroy(qteButton.gameObject);
        background.gameObject.SetActive(false);
    }

    private void OnQteElementStart(KeyCode keyCode, float time)
    {
        qteButton = Instantiate(qteButtonPrefab, this.transform).GetComponent<QteButton>();

        qteButton.StartTimer(keyCode, time);
        background.gameObject.SetActive(true);
    }

    public void Start()
    {
        Debug.Log(QTEManager.Instance);
        QTEManager.Instance.OnQteElementStart += OnQteElementStart;
        QTEManager.Instance.OnQteElementEnd += OnQteElementEnd;
        background.gameObject.SetActive(false);
    }

    public void OnDestroy()
    {
        QTEManager.Instance.OnQteElementStart -= OnQteElementStart;
        QTEManager.Instance.OnQteElementEnd -= OnQteElementEnd;
    }
}
