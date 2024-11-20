using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("The player")] [SerializeField]
    private Transform target;

    [Header("Configuration")] [SerializeField]
    private float attackDistance = 1.1f;

    [SerializeField] private float timeBetweenAttacks = 2f;
    [SerializeField] private float attackDuration = 0.25f;

    [Header("References")] [SerializeField]
    private Transform hitCollider;

    NavMeshAgent navMeshAgent;
    Animator animator;
    float timeOfLastAttack;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        timeOfLastAttack = Time.time;
    }

    private void Update()
    {
        if (Vector3.Distance(target.position, transform.position) < attackDistance)
        {
            navMeshAgent.isStopped = true;
            if (Time.time - timeOfLastAttack > timeBetweenAttacks)
            {
                animator.SetTrigger("Attack");
                hitCollider.gameObject.SetActive(true);
                DOVirtual.DelayedCall(attackDuration, () => hitCollider.gameObject.SetActive(false));
                timeOfLastAttack = Time.time;
            }
        }
        else
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.destination = target.position;
        }
    }
}