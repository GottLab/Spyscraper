using UnityEngine;

public class StationaryEnemyAI : MonoBehaviour
{
    // --- Riferimenti ---
    [Tooltip("Assegna qui il GameObject del tuo Giocatore.")]
    public Transform playerTransform; // Riferimento al Transform del giocatore
    [Tooltip("Il Layer del Giocatore (impostato nel GameObject del giocatore).")]
    public LayerMask playerLayer;     // Layer del giocatore per il Raycast
    [Tooltip("I Layer degli oggetti che bloccano la visione (es. Muri, Ambiente).")]
    public LayerMask obstacleLayer;   // Layer degli ostacoli per il Raycast

    // --- Parametri di Visione ---
    [Tooltip("Il raggio massimo entro cui il nemico può vedere. Corrisponde al Radius dello Sphere Collider.")]
    public float visionRange = 15f;   // Raggio di visione
    [Tooltip("L'angolo totale del cono di visione del nemico (es. 90 per 45 gradi per lato).")]
    public float visionAngle = 90f;   // Angolo di visione

    // --- Stato AI ---
    [Tooltip("Indica se il giocatore è attualmente rilevato dal nemico.")]
    public bool playerDetected = false; // Flag per lo stato di rilevamento del giocatore

    // Riferimento al collider di visione (opzionale, ma utile per debug/coerenza)
    private SphereCollider visionCollider;
    
    public Light visionSpotLight; // Assegna la Spot Light dall'Inspector
    public Color normalVisionColor = Color.yellow; // Colore normale della torcia
    public Color detectedVisionColor = Color.red;   // Colore della torcia quando il giocatore è rilevato
    
    void Awake()
    {
        // Ottiene il riferimento allo Sphere Collider attaccato a questo GameObject
        visionCollider = GetComponent<SphereCollider>();
        if (visionCollider != null)
        {
            // Assicura che il raggio del collider sia coerente con visionRange e che sia un trigger
            visionCollider.radius = visionRange;
            visionCollider.isTrigger = true;
        }
        else
        {
            Debug.LogWarning("StationaryEnemyAI: Nessuno Sphere Collider trovato sul GameObject! La visione potrebbe non funzionare correttamente.");
        }

        // Se playerTransform non è stato assegnato, prova a cercarlo per tag "Player"
        if (playerTransform == null)
        {
            GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
            if (playerGO != null)
            {
                playerTransform = playerGO.transform;
            }
            else
            {
                Debug.LogWarning("StationaryEnemyAI: Il GameObject del Giocatore non è assegnato e non trovato con tag 'Player'. La visione AI non funzionerà.");
            }
        }
        
        if (visionSpotLight != null)
        {
            visionSpotLight.color = normalVisionColor; // Inizia con il colore normale
        }
    }

    void Update()
    {
        // Controlla il giocatore ogni frame
        CheckForPlayer();

        // --- Qui la logica di cosa FA il nemico quando rileva il giocatore ---
        if (playerDetected)
        {
            RotateTowardsPlayer(); // Se vuoi che la torretta giri

            // Modifica il colore della luce quando il giocatore è rilevato
            if (visionSpotLight != null)
            {
                visionSpotLight.color = detectedVisionColor;
            }
            // Debug.Log("Nemico Immobile: Giocatore RILEVATO!");
        }
        else
        {
            // Ripristina il colore normale quando il giocatore non è rilevato
            if (visionSpotLight != null)
            {
                visionSpotLight.color = normalVisionColor;
            }
            // Debug.Log("Nemico Immobile: Giocatore NON rilevato.");
        }
    }

    // Metodo che viene chiamato quando un collider entra o rimane nel trigger
    void OnTriggerStay(Collider other)
    {
        // Controlla se l'oggetto nel trigger è il giocatore (tramite tag)
        if (other.CompareTag("Player"))
        {
            // Se il giocatore è nel raggio, affina il controllo con la funzione di visione
            if (IsPlayerInVisionRange())
            {
                playerDetected = true;
            }
            else
            {
                // Se prima era rilevato ma ora è fuori dall'angolo o bloccato, reimposta
                playerDetected = false;
            }
        }
    }

    // Metodo che viene chiamato quando un collider esce dal trigger
    void OnTriggerExit(Collider other)
    {
        // Se il giocatore esce dal trigger, non è più rilevato
        if (other.CompareTag("Player"))
        {
            playerDetected = false;
            // Debug.Log("Nemico Immobile: Giocatore uscito dal raggio.");
        }
    }

    // Funzione principale per la verifica della visione
    void CheckForPlayer()
    {
        // Se il playerTransform non è assegnato, non possiamo controllare la visione
        if (playerTransform == null)
        {
            playerDetected = false;
            return;
        }

        // Esegue la logica di IsPlayerInVisionRange() anche se non ci sono trigger,
        // per catturare casi limite o se si preferisce non usare OnTriggerStay per la logica principale.
        // Manteniamo playerDetected aggiornato qui.
        playerDetected = IsPlayerInVisionRange();
    }


    // Controlla se il giocatore è nel campo visivo conico E non è bloccato da ostacoli
    bool IsPlayerInVisionRange()
    {
        // 1. Controllo della distanza (se per qualche motivo non siamo già nel trigger)
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer > visionRange)
        {
            return false;
        }

        // 2. Controllo dell'angolo di visione (cono)
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        // Calcola l'angolo tra la direzione frontale del nemico e la direzione verso il giocatore
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        if (angle < visionAngle / 2) // Dividiamo per 2 perché visionAngle è l'angolo totale (es. 45 a sinistra, 45 a destra)
        {
            // 3. Raycast per verificare la presenza di ostacoli
            RaycastHit hit;
            // Origine del Raycast leggermente più alta per evitare di colpire il terreno
            Vector3 raycastOrigin = transform.position + Vector3.up * 0.5f;

            // Il Raycast ignorerà il layer del giocatore, ma colliderà con tutto il resto
            // Combinando obstacleLayer e playerLayer per ignorare solo il player
            // Oppure, se obstacleLayer contiene già tutti i layer che bloccano la visione:
            // ~playerLayer è il modo più semplice se vuoi che il Raycast ignori solo il giocatore
            if (Physics.Raycast(raycastOrigin, directionToPlayer, out hit, visionRange, ~playerLayer))
            {
                // Se il Raycast ha colpito qualcosa E non è il giocatore, allora c'è un ostacolo
                if (hit.transform != playerTransform)
                {
                    // Debug.Log("Visione bloccata da: " + hit.collider.name);
                    return false;
                }
            }
            // Se il Raycast non ha colpito nulla, o ha colpito il giocatore, la visione è libera
            return true;
        }

        return false; // Il giocatore non è all'interno dell'angolo di visione
    }

    // Metodo per far ruotare il nemico verso il giocatore (esempio di reazione)
    void RotateTowardsPlayer()
    {
        if (playerTransform == null) return;

        Vector3 direction = (playerTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z)); // Solo rotazione sull'asse Y
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); // Velocità di rotazione 5f
    }


    // --- DEBUG: Disegna il raggio e il cono di visione in scena ---
    void OnDrawGizmos()
    {
        // Disegna la sfera del raggio di visione (gialla)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        // Disegna il cono di visione (blu)
        Gizmos.color = Color.blue;
        Vector3 forwardDir = transform.forward * visionRange; // Direzione frontale del nemico
        // Calcola le direzioni dei raggi laterali del cono
        Quaternion leftRayRotation = Quaternion.AngleAxis(-visionAngle / 2, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(visionAngle / 2, Vector3.up);
        Vector3 leftRayDirection = leftRayRotation * forwardDir;
        Vector3 rightRayDirection = rightRayRotation * forwardDir;

        // Disegna i raggi
        Gizmos.DrawRay(transform.position, leftRayDirection);
        Gizmos.DrawRay(transform.position, rightRayDirection);
        // Disegna la "base" del cono per una migliore visualizzazione
        Gizmos.DrawLine(transform.position + leftRayDirection, transform.position + rightRayDirection);
    }
}