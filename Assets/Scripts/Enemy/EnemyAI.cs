using UnityEngine;
using UnityEngine.AI; // Necessario per NavMeshAgent

public class EnemyAI : MonoBehaviour
{
    // --- Riferimenti ---
    [Tooltip("Assegna qui il GameObject del tuo Giocatore.")]
    public Transform playerTransform; // Riferimento al Transform del giocatore
    [Tooltip("Il Layer del Giocatore (impostato nel GameObject del giocatore).")]
    public LayerMask playerLayer;     // Layer del giocatore per il Raycast
    [Tooltip("I Layer degli oggetti che bloccano la visione (es. Muri, Ambiente).")]
    public LayerMask obstacleLayer;   // Layer degli ostacoli per il Raycast
    // --- Riferimenti UI & Luce ---
    [Header("UI & Light References")]
    [Tooltip("Assegna la Spot Light che simula la torcia per questo nemico.")]
    public Light visionSpotLight;     // Riferimento alla Spot Light per la visione
    [Tooltip("Colore della torcia nello stato di Pattugliamento.")]
    public Color patrolVisionColor = Color.green; // Colore per lo stato di pattugliamento
    [Tooltip("Colore della torcia nello stato di Inseguimento.")]
    public Color chaseVisionColor = Color.red;   // Colore per lo stato di inseguimento
    // --- Componenti AI ---
    private NavMeshAgent agent;

    // --- Stati dell'AI ---
    public enum AIState
    {
        Patrolling,
        Chasing
    }
    public AIState currentState;

    // --- Parametri di Pattugliamento ---
    [Header("Patrolling Settings")]
    public float patrolSpeed = 2f;
    public float patrolRadius = 20f; // Raggio entro cui scegliere un punto di pattuglia dalla posizione iniziale
    public float patrolPointThreshold = 1f; // Distanza per considerare raggiunto il punto di pattuglia
    public float newPatrolPointCooldown = 3f; // Tempo di attesa prima di cercare un nuovo punto
    private Vector3 currentPatrolTarget;
    private float nextPatrolPointTime;
    private Vector3 initialPatrolPosition; // Posizione da cui iniziare il pattugliamento

    // --- Parametri di Inseguimento ---
    [Header("Chasing Settings")]
    public float chaseSpeed = 5f;
    public float losePlayerDistance = 20f; // Distanza oltre la quale il nemico smette di inseguire
    public float timeToLosePlayer = 3f; // Tempo dopo cui l'allerta cala e si torna a pattugliare
    private float playerLostTimer;

    // --- Parametri di Visione ---
    [Header("Vision Settings")]
    public float visionRange = 15f;   // Raggio di visione (dovrebbe corrispondere al raggio dello Sphere Collider)
    public float visionAngle = 90f;   // Angolo di visione (es. 90 per un cono di 45 gradi per lato)

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        initialPatrolPosition = transform.position; // Memorizza la posizione iniziale per il pattugliamento
        currentState = AIState.Patrolling;          // Inizia in modalità pattuglia
        SetNextPatrolTarget();                      // Imposta subito un primo punto di pattuglia

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
                Debug.LogWarning("MobileEnemyAI: Il GameObject del Giocatore non è assegnato e non trovato con tag 'Player'. La visione AI non funzionerà.");
            }
        }

        // Assicura che lo Sphere Collider sia un trigger e che il suo raggio sia coerente
        SphereCollider visionCollider = GetComponent<SphereCollider>();
        if (visionCollider != null)
        {
            visionCollider.radius = visionRange;
            visionCollider.isTrigger = true;
        }
        // ... (codice Awake esistente) ...

        if (visionSpotLight != null)
        {
            visionSpotLight.color = patrolVisionColor; // Inizia con il colore di pattugliamento
        }
    }

    void Update()
    {
        switch (currentState)
        {
            case AIState.Patrolling:
                PatrolLogic();
                break;
            case AIState.Chasing:
                ChaseLogic();
                break;
        }
    }

    // --- Logica dello Stato di Pattugliamento ---
    void PatrolLogic()
    {
        agent.speed = patrolSpeed;

        // Controlla se il nemico ha raggiunto il punto di pattuglia corrente
        // agent.remainingDistance < patrolPointThreshold e !agent.pathPending indicano che l'agente è vicino o ha finito il percorso.
        if (!agent.pathPending && agent.remainingDistance < patrolPointThreshold)
        {
            if (Time.time >= nextPatrolPointTime)
            {
                SetNextPatrolTarget();
                nextPatrolPointTime = Time.time + newPatrolPointCooldown;
            }
        }

        // Controlla se il giocatore è a vista
        if (IsPlayerInVisionRange())
        {
            currentState = AIState.Chasing;
            playerLostTimer = timeToLosePlayer; // Reset del timer quando si inizia a inseguire
            Debug.Log("MobileEnemy: Giocatore rilevato! Inseguimento iniziato.");
            if (visionSpotLight != null)
            {
                visionSpotLight.color = chaseVisionColor; // Cambia colore a rosso
            }
        }
        else // Se non è nel cono di visione, assicurati che il colore sia quello di pattugliamento
        {
            if (visionSpotLight != null)
            {
                visionSpotLight.color = patrolVisionColor; // Assicurati che sia verde
            }
        }
    }

    // --- Logica dello Stato di Inseguimento ---
    void ChaseLogic()
    {
        agent.speed = chaseSpeed;

        if (playerTransform != null)
        {
            agent.SetDestination(playerTransform.position);

            // Controlla se il giocatore è ancora visibile
            if (IsPlayerInVisionRange())
            {
                playerLostTimer = timeToLosePlayer; // Reset del timer se il giocatore è ancora visibile
                if (visionSpotLight != null)
                {
                    visionSpotLight.color = chaseVisionColor; // Assicurati che sia rosso
                }
            }
            else
            {
                // Se il giocatore non è più visibile, avvia il conto alla rovescia
                playerLostTimer -= Time.deltaTime;
                if (playerLostTimer <= 0f)
                {
                    currentState = AIState.Patrolling;
                    SetNextPatrolTarget(); // Imposta subito un nuovo punto di pattuglia
                    Debug.Log("MobileEnemy: Giocatore perso di vista. Torno a pattugliare.");
                    if (visionSpotLight != null)
                    {
                        visionSpotLight.color = patrolVisionColor; // Cambia colore a verde
                    }
                }
            }

            // Controlla anche se il giocatore è uscito dal raggio massimo di inseguimento
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer > losePlayerDistance)
            {
                currentState = AIState.Patrolling;
                SetNextPatrolTarget();
                Debug.Log("MobileEnemy: Giocatore troppo lontano. Torno a pattugliare.");
                if (visionSpotLight != null)
                {
                    visionSpotLight.color = patrolVisionColor; // Cambia colore a verde
                }
            }
        }
        else
        {
            // Il giocatore è stato distrutto o non è assegnato, torna a pattugliare
            currentState = AIState.Patrolling;
            SetNextPatrolTarget();
            Debug.LogWarning("MobileEnemy: Player transform è null. Torno a pattugliare.");
        }
    }

    // --- Funzioni di Utility ---

    // Imposta una nuova destinazione casuale per il pattugliamento entro un raggio
    void SetNextPatrolTarget()
    {
        Vector3 randomPoint = initialPatrolPosition + Random.insideUnitSphere * patrolRadius;
        NavMeshHit hit;
        // Cerca il punto più vicino sulla NavMesh
        if (NavMesh.SamplePosition(randomPoint, out hit, patrolRadius, NavMesh.AllAreas))
        {
            currentPatrolTarget = hit.position;
            agent.SetDestination(currentPatrolTarget);
            Debug.Log("MobileEnemy: Impostato nuovo punto di pattuglia: " + currentPatrolTarget);
        }
        else
        {
            // Se non trova un punto valido, riprova subito o pattuglia sul posto
            Debug.LogWarning("MobileEnemy: Impossibile trovare un punto di pattuglia valido. Riprovo o pattuglio sul posto.");
            agent.SetDestination(transform.position); // Rimane fermo se non trova
            nextPatrolPointTime = Time.time + 1f; // Tenta di nuovo tra 1 secondo
        }
    }

    // Controlla se il giocatore è nel campo visivo (distanza, angolo, ostacoli)
    bool IsPlayerInVisionRange()
    {
        if (playerTransform == null) return false;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer > visionRange) return false;

        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        if (angle < visionAngle / 2)
        {
            RaycastHit hit;
            // Aggiungi un piccolo offset verso l'alto per il Raycast per evitare di colpire il terreno se l'origine è troppo bassa
            Vector3 raycastOrigin = transform.position + Vector3.up * 0.5f;

            // Il Raycast ignorerà il layer del giocatore, ma colliderà con tutti gli altri (ostacoli)
            if (Physics.Raycast(raycastOrigin, directionToPlayer, out hit, visionRange, ~playerLayer))
            {
                // Se il Raycast ha colpito qualcosa E non è il giocatore, allora c'è un ostacolo
                if (hit.transform != playerTransform)
                {
                    // Debug.Log("Visione Mobile bloccata da: " + hit.collider.name);
                    return false;
                }
            }
            return true; // Visione libera verso il giocatore
        }
        return false; // Giocatore fuori dall'angolo di visione
    }

    // --- DEBUG: Disegna il raggio e il cono di visione in scena ---
    void OnDrawGizmos()
    {
        // Disegna la sfera del raggio di visione (gialla)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        // Disegna il cono di visione (blu per pattuglia, rosso per inseguimento)
        if (currentState == AIState.Chasing)
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.blue;
        }

        Vector3 forwardDir = transform.forward * visionRange;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-visionAngle / 2, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(visionAngle / 2, Vector3.up);
        Vector3 leftRayDirection = leftRayRotation * forwardDir;
        Vector3 rightRayDirection = rightRayRotation * forwardDir;

        Gizmos.DrawRay(transform.position, leftRayDirection);
        Gizmos.DrawRay(transform.position, rightRayDirection);
        Gizmos.DrawLine(transform.position + leftRayDirection, transform.position + rightRayDirection);

        // Disegna il raggio di pattugliamento iniziale
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(initialPatrolPosition, patrolRadius);
    }
}
