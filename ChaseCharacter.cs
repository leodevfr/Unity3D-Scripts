using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class ChaseCharacter : MonoBehaviour
{
    public Transform player;
    public Animator animator;
    public NavMeshAgent navMeshAgent;

    public float chaseRange = 10f;
    public float deathRange = 1f;
    public float randomMoveInterval = 5f;

    public float walkSpeed = 3.0f;  // Vitesse de marche
    public float chaseSpeed = 6.0f; // Vitesse de poursuite
    public float rotationSpeed = 360.0f; // Vitesse de rotation en degrés par seconde

    private float timeSinceLastRandomMove = 0f;

    private enum EnemyState
    {
        Idle,
        Walking,
        Running
    }

    private EnemyState currentState = EnemyState.Idle;

    private void Start()
    {
        navMeshAgent.speed = walkSpeed; // Commencez avec la vitesse de marche
    }

    private void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance < deathRange)
        {
            PlayerDeath();
            return;
        }

        if (distance < chaseRange)
        {
            ChasePlayer();
        }
        else
        {
            if (currentState != EnemyState.Walking && timeSinceLastRandomMove >= randomMoveInterval)
            {
                MoveRandomly();
                timeSinceLastRandomMove = 0f;
            }
            else
            {
                timeSinceLastRandomMove += Time.deltaTime;

                // Si l'ennemi était en train de courir, rétablissez la vitesse de marche
                if (currentState == EnemyState.Running)
                {
                    SetWalkSpeed();
                }
            }
        }

        // Vérifie si l'ennemi est en train de marcher et s'il a atteint sa destination.
        if (currentState == EnemyState.Walking && !navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            // L'ennemi a atteint sa destination, remettre en état "Idle".
            SetEnemyState(EnemyState.Idle);
            SetWalkSpeed(); // Revenez à la vitesse de marche
        }
    }

    private void MoveRandomly()
    {
        Vector3 randomPosition = GetRandomNavMeshPosition();
        navMeshAgent.SetDestination(randomPosition);
        SetEnemyState(EnemyState.Walking);
    }

    private void SetWalkSpeed()
    {
        navMeshAgent.speed = walkSpeed; // Définir la vitesse de marche
    }

    private void SetEnemyState(EnemyState newState)
    {
        if (newState == currentState)
            return;

        animator.SetBool("isIdle", false);
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);

        switch (newState)
        {
            case EnemyState.Idle:
                animator.SetBool("isIdle", true);
                break;
            case EnemyState.Walking:
                animator.SetBool("isWalking", true);
                break;
            case EnemyState.Running:
                animator.SetBool("isRunning", true);
                break;
        }

        currentState = newState;
    }

    private void PlayerDeath()
    {
        SceneManager.LoadScene("Lost");
    }


    private void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;

        // Calcul de la rotation souhaitée pour faire face au joueur
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

        // Interpolation de la rotation actuelle vers la rotation souhaitée
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        navMeshAgent.SetDestination(player.position);
        navMeshAgent.speed = chaseSpeed; // Utilisez la vitesse de poursuite
        SetEnemyState(EnemyState.Running);
    }

    private Vector3 GetRandomNavMeshPosition()
    {
        Vector3 randomPosition = Vector3.zero;
        NavMeshHit hit;
        bool found = false;

        while (!found)
        {
            Vector3 randomDirection = Random.insideUnitSphere * chaseRange;
            randomDirection += transform.position;

            if (NavMesh.SamplePosition(randomDirection, out hit, chaseRange, NavMesh.AllAreas))
            {
                randomPosition = hit.position;
                found = true;
            }
        }

        return randomPosition;
    }
}
