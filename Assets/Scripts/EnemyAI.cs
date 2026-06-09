using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum State { Navigate, Attack }

    [Header("检测")]
    public float detectionRange = 30f;
    public float attackRange = 3f;

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

    private Transform currentTarget;
    private float attackTimer;
    private float pathUpdateTimer;
    private const float pathUpdateInterval = 0.3f;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyControl = GetComponent<EnemyControl>();
        anim = GetComponent<Animator>();
    }

    void OnEnable()
    {
        if (player == null)
        {
            var go = GameObject.FindWithTag("Player");
            if (go != null) player = go.transform;
        }
        if (defenseTarget == null)
            defenseTarget = FindObjectOfType<DefenseTarget>();

        state = State.Navigate;
        attackTimer = 1f;
        pathUpdateTimer = 0;
        agent.isStopped = false;
    }

    void Update()
    {
        if (enemyControl != null && enemyControl.HP <= 0) return;

        currentTarget = PickTarget();
        if (currentTarget == null) return;

        UpdateAnimator();

        switch (state)
        {
            case State.Navigate: StateNavigate(); break;
            case State.Attack:   StateAttack();   break;
        }
    }

    Transform PickTarget()
    {
        // 防御塔始终有效
        if (defenseTarget != null && defenseTarget.HP > 0)
        {
            if (player == null)
                return defenseTarget.transform;

            float sqrToPlayer = (player.position - transform.position).sqrMagnitude;
            float sqrToTower = (defenseTarget.transform.position - transform.position).sqrMagnitude;
            float sqrDetect = detectionRange * detectionRange;

            // 玩家在检测范围且比塔更近 → 攻击玩家，否则打塔
            if (sqrToPlayer <= sqrDetect && sqrToPlayer < sqrToTower)
                return player;
            else
                return defenseTarget.transform;
        }

        if (player != null)
            return player;

        return null;
    }

    void StateNavigate()
    {
        float sqrDist = (currentTarget.position - transform.position).sqrMagnitude;
        float sqrAttack = attackRange * attackRange;

        if (sqrDist <= sqrAttack)
        {
            TransitionTo(State.Attack);
            return;
        }

        pathUpdateTimer += Time.deltaTime;
        if (pathUpdateTimer >= pathUpdateInterval)
        {
            pathUpdateTimer = 0;
            agent.SetDestination(currentTarget.position);
        }
        agent.isStopped = false;
    }

    void StateAttack()
    {
        float sqrDist = (currentTarget.position - transform.position).sqrMagnitude;
        float sqrAttack = attackRange * attackRange * 1.3f; // 退出攻击距离稍远

        if (sqrDist > sqrAttack)
        {
            TransitionTo(State.Navigate);
            return;
        }

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

    void TransitionTo(State newState)
    {
        state = newState;
        attackTimer = 0;
        pathUpdateTimer = 0;
        agent.isStopped = (newState == State.Attack);
    }

    void FaceTarget()
    {
        Vector3 dir = currentTarget.position - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 8f);
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
    }
}
