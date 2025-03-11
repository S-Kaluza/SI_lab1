using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    public Transform target;
    public float stopDistance = 3.0f;

    private NavMeshAgent agent;
    private Animator animator;
    private bool hasStopped = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (agent == null)
        {
            Debug.LogError("ERROR: NavMeshAgent nie został znaleziony na " + gameObject.name);
            return;
        }
        if (!agent.isOnNavMesh)
        {
            Debug.LogError("ERROR: Agent nie znajduje się na NavMesh! Sprawdź pozycję startową.");
            return;
        }
        if (target == null)
        {
            target = GameObject.Find("Target")?.transform;
            if (target == null)
                target = GameObject.FindGameObjectWithTag("Target")?.transform;
            if (target == null)
            {
                Debug.LogError("ERROR: Nie znaleziono obiektu 'Target' w Hierarchy.");
                return;
            }
            else
            {
                Debug.Log("Target przypisany: " + target.name);
            }
        }

        // Ustawienia agenta
        agent.autoRepath = true;
        agent.updateRotation = true;
        agent.stoppingDistance = stopDistance;
        agent.SetDestination(target.position);
    }

    void Update()
    {
        if (agent == null || target == null)
            return;

        // Sprawdzenie, czy agent jest na NavMesh
        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning("Agent nie znajduje się na NavMesh. Próba naprawy...");
            TryPlaceOnNavMesh();
            return;
        }

        float distanceToTarget = Vector3.Distance(agent.transform.position, target.position);

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && !hasStopped)
        {
            StopAgent();
            return;
        }

        if (!agent.hasPath && !hasStopped)
        {
            agent.SetDestination(target.position);
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
    }

    void StopAgent()
    {
        hasStopped = true;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        agent.ResetPath();
        // Wymuszenie ustawienia agenta dokładnie na pozycji target
        agent.Warp(target.position);
        if (animator != null)
        {
            animator.SetFloat("Speed", 0);
        }
        // Wyłączenie dalszej aktualizacji skryptu
        this.enabled = false;
    }

    void TryPlaceOnNavMesh()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 10.0f, NavMesh.AllAreas))
        {
            Debug.Log("Naprawiono pozycję Agenta: " + hit.position);
            agent.Warp(hit.position);
            agent.SetDestination(target.position);
        }
        else
        {
            Debug.LogError("ERROR: Agent nie może zostać umieszczony na NavMesh!");
        }
    }

    bool IsTargetOnNavMesh()
    {
        NavMeshHit hit;
        return NavMesh.SamplePosition(target.position, out hit, 1.0f, NavMesh.AllAreas);
    }

    void ClampTargetPosition()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(target.position, out hit, 10.0f, NavMesh.AllAreas))
        {
            target.position = hit.position;
            Debug.Log("Target został przypięty do NavMesh: " + hit.position);
        }
        else
        {
            Debug.LogError("ERROR: Nie udało się znaleźć punktu na NavMesh dla Targetu!");
        }
    }
}
