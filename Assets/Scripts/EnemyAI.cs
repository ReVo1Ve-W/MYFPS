using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum State { Idle, Chase, Attack }

    [Header("检测")]
    public float detectionRange = 20f;
    public float attackRange = 3f;
    public float loseSightTimeout = 5f;

    [Header("巡逻")]
    public float patrolRadius = 15f;
    public float patrolWaitTime = 2f;

    [Header("攻击")]
    public float attackDamage = 10f;
    public float attackCooldown = 1.5f;

    [Header("引用")]
    public Transform player;

    private State state;
    private NavMeshAgent agent;
    private EnemyControl enemyControl;
    private Animator anim;
    private Vector3 startPosition;

    private float idleTimer;
    private float loseTimer;
    private float attackTimer;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyControl = GetComponent<EnemyControl>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        startPosition = transform.position;

        if (player == null)
        {
            var go = GameObject.FindWithTag("Player");
            if (go != null) player = go.transform;
        }

        TransitionTo(State.Idle);
        idleTimer = 0; // 第一轮先巡逻一次再等
    }

    void Update()
    {
        if (enemyControl != null && enemyControl.HP <= 0) return;
        if (player == null) return;

        UpdateAnimator();

        switch (state)
        {
            case State.Idle:  StateIdle();  break;
            case State.Chase: StateChase(); break;
            case State.Attack: StateAttack(); break;
        }
    }

    // ═══════════════ 空闲 / 巡逻 ═══════════════

    void StateIdle()
    {
        float dist = DistanceToPlayer();
        if (dist <= detectionRange) { TransitionTo(State.Chase); return; }

        idleTimer += Time.deltaTime;
        if (idleTimer >= patrolWaitTime)
        {
            idleTimer = 0;
            if (FindPatrolPoint(out Vector3 point))
            {
                agent.SetDestination(point);
                agent.isStopped = false;
            }
        }
    }

    bool FindPatrolPoint(out Vector3 point)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 random = startPosition + Random.insideUnitSphere * patrolRadius;
            if (NavMesh.SamplePosition(random, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
            {
                point = hit.position;
                return true;
            }
        }
        point = Vector3.zero;
        return false;
    }

    // ═══════════════ 追击 ═══════════════

    void StateChase()
    {
        float dist = DistanceToPlayer();

        if (dist <= attackRange) { TransitionTo(State.Attack); return; }

        if (dist > detectionRange)
        {
            loseTimer += Time.deltaTime;
            if (loseTimer >= loseSightTimeout) { TransitionTo(State.Idle); return; }
        }
        else
        {
            loseTimer = 0;
        }

        agent.SetDestination(player.position);
        agent.isStopped = false;
    }

    // ═══════════════ 攻击 ═══════════════

    void StateAttack()
    {
        float dist = DistanceToPlayer();

        if (dist > detectionRange) { TransitionTo(State.Idle); return; }
        if (dist > attackRange)   { TransitionTo(State.Chase); return; }

        agent.isStopped = true;
        FacePlayer();

        attackTimer += Time.deltaTime;
        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0;
            var pc = player.GetComponent<PlayerControl>();
            if (pc != null) pc.TakeDamage(attackDamage);
        }
    }

    // ═══════════════ 工具 ═══════════════

    void TransitionTo(State newState)
    {
        state = newState;
        idleTimer = 0;
        loseTimer = 0;
        attackTimer = 0;
        agent.isStopped = newState == State.Attack;
    }

    float DistanceToPlayer()
    {
        return Vector3.Distance(transform.position, player.position);
    }

    void FacePlayer()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);
    }

    void UpdateAnimator()
    {
        if (anim != null)
            anim.SetFloat(SpeedHash, agent.velocity.magnitude);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(Application.isPlaying ? startPosition : transform.position, patrolRadius);
    }
}
