using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [Header("移动")]
    public float walkSpeed = 5f;
    public float runSpeed = 7f;
    public float jumpForce = 5f;

    [Header("视角")]
    public float sensitivity = 3f;
    [Range(0, 90)] public float lookClamp = 90f;

    [HideInInspector] public bool highSpeed;
    [HideInInspector] public bool isAiming;

    private float xRotation;
    private float mouseX, mouseY;
    private Rigidbody rb;
    private Animator anim;
    private float moveSpeed;
    private Vector3 moveVelocity;
    private bool jump;
    private bool grounded;

    private static readonly int ParamAim       = Animator.StringToHash("Aim");
    private static readonly int ParamAiming    = Animator.StringToHash("Aiming");
    private static readonly int ParamHolstered = Animator.StringToHash("Holstered");
    private static readonly int ParamMovement  = Animator.StringToHash("Movement");

    void Awake()
    {
        // 240Hz 优化：提高物理频率，关闭 VSync
        Time.fixedDeltaTime = 1f / 120f;
        Application.targetFrameRate = 240;
        QualitySettings.vSyncCount = 0;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();

        rb.interpolation = RigidbodyInterpolation.Extrapolate;
        rb.freezeRotation = true;

        Cursor.lockState = CursorLockMode.Locked;
        moveSpeed = walkSpeed;
    }

    void Update()
    {
        // 只采集输入，不做旋转（等 LateUpdate 在 Animator 之后统一做）
        mouseX = Input.GetAxisRaw("Mouse X") * sensitivity;
        mouseY = Input.GetAxisRaw("Mouse Y") * sensitivity;

        Aim();
        HighSpeed();

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 dir = (transform.forward * v) + (transform.right * h);
        if (dir.sqrMagnitude > 1f) dir.Normalize();
        moveVelocity = dir * moveSpeed;
        anim.SetFloat(ParamMovement, dir.magnitude);

        if (Input.GetKeyDown(KeyCode.Space) && grounded)
            jump = true;
    }

    void LateUpdate()
    {
        // 全部旋转在 LateUpdate 执行——Animator 已经跑完，不会冲突
        transform.Rotate(0, mouseX, 0);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -lookClamp, lookClamp);
        anim.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
    }

    void FixedUpdate()
    {
        grounded = IsGround();

        Vector3 vel = moveVelocity;
        vel.y = rb.velocity.y;
        rb.velocity = vel;

        if (jump)
        {
            jump = false;
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        }
    }

    void Aim()
    {
        bool aiming = Input.GetMouseButton(1);
        isAiming = aiming;
        anim.SetBool(ParamAim, aiming);

        float target = aiming ? 1f : 0f;
        anim.SetFloat(ParamAiming, Mathf.Lerp(anim.GetFloat(ParamAiming), target, 0.1f));
    }

    void HighSpeed()
    {
        bool running = Input.GetKey(KeyCode.LeftShift) && grounded;
        highSpeed = running;
        moveSpeed = running ? runSpeed : walkSpeed;
        anim.SetBool(ParamHolstered, running);
    }

    bool IsGround()
    {
        return Physics.Raycast(transform.position + Vector3.up * 0.2f, Vector3.down, 0.4f, LayerMask.GetMask("Ground"));
    }

    public float playerHP = 100f;

    public void TakeDamage(float damage)
    {
        playerHP -= damage;
        if (playerHP <= 0)
        {
            playerHP = 0;
            Debug.Log("Player died!");
        }
    }
}
