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
    public DefenseTarget defenseTarget;

    private State state;
    private NavMeshAgent agent;
    private EnemyControl enemyControl;
    private Animator anim;
    private Vector3 startPosition;

    private Transform currentTarget;
    private float idleTimer;
    private float loseTimer;
    private float attackTimer;
    private float pathUpdateTimer;
    private const float pathUpdateInterval = 0.3f;

    private float sqrDetectionRange;
    private float sqrAttackRange;

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

        sqrDetectionRange = detectionRange * detectionRange;
        sqrAttackRange = attackRange * attackRange;

        if (player == null)
        {
            var go = GameObject.FindWithTag("Player");
            if (go != null) player = go.transform;
        }

        if (defenseTarget == null)
            defenseTarget = FindObjectOfType<DefenseTarget>();

        TransitionTo(State.Idle);
        idleTimer = 0;
    }

    void Update()
    {
        if (enemyControl != null && enemyControl.HP <= 0) return;

        currentTarget = PickTarget();
        if (currentTarget == null) return;

        UpdateAnimator();

        switch (state)
        {
            case State.Idle:  StateIdle();  break;
            case State.Chase: StateChase(); break;
            case State.Attack: StateAttack(); break;
        }
    }

    // ═══════════════ 目标选择 ═══════════════

    Transform PickTarget()
    {
        // 优先攻击防御目标（水塔），其次攻击玩家，谁近打谁
        Transform best = null;

        if (defenseTarget != null && defenseTarget.HP > 0)
            best = defenseTarget.transform;

        if (player != null)
        {
            if (best == null)
                best = player;
            else
            {
                float sqrToPlayer = (player.position - transform.position).sqrMagnitude;
                float sqrToBest   = (best.position - transform.position).sqrMagnitude;
                if (sqrToPlayer < sqrToBest)
                    best = player;
            }
        }

        return best;
    }

    // ═══════════════ 空闲 / 巡逻 ═══════════════

    void StateIdle()
    {
        if (TargetInRange(detectionRange)) { TransitionTo(State.Chase); return; }

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
        float sqrDist = SqrDistanceToTarget();

        if (sqrDist <= sqrAttackRange) { TransitionTo(State.Attack); return; }

        if (sqrDist > sqrDetectionRange)
        {
            loseTimer += Time.deltaTime;
            if (loseTimer >= loseSightTimeout) { TransitionTo(State.Idle); return; }
        }
        else
        {
            loseTimer = 0;
        }

        pathUpdateTimer += Time.deltaTime;
        if (pathUpdateTimer >= pathUpdateInterval)
        {
            pathUpdateTimer = 0;
            agent.SetDestination(currentTarget.position);
        }
        agent.isStopped = false;
    }

    // ═══════════════ 攻击 ═══════════════

    void StateAttack()
    {
        float sqrDist = SqrDistanceToTarget();

        if (sqrDist > sqrDetectionRange) { TransitionTo(State.Idle); return; }
        if (sqrDist > sqrAttackRange)   { TransitionTo(State.Chase); return; }

        agent.isStopped = true;
        FaceTarget();

        attackTimer += Time.deltaTime;
        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0;
            if (currentTarget.TryGetComponent<PlayerControl>(out var pc))
                pc.TakeDamage(attackDamage);
            else if (currentTarget.TryGetComponent<DefenseTarget>(out var dt))
                dt.TakeDamage(attackDamage);
        }
    }

    // ═══════════════ 工具 ═══════════════

    void TransitionTo(State newState)
    {
        state = newState;
        idleTimer = 0;
        loseTimer = 0;
        attackTimer = 0;
        pathUpdateTimer = 0;
        agent.isStopped = (newState == State.Attack);
    }

    float SqrDistanceToTarget()
    {
        return (currentTarget.position - transform.position).sqrMagnitude;
    }

    bool TargetInRange(float range)
    {
        return SqrDistanceToTarget() <= range * range;
    }

    void FaceTarget()
    {
        Vector3 dir = currentTarget.position - transform.position;
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
