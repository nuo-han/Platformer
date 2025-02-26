using Cinemachine;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class PlayerController : HealthController
{
    private GroundDetector groundDetector;
    private WallDetector wallDetector;
    private PlayerInput input;
    private Rigidbody2D rb;
    private Animator anim;

    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 7f;
    public float crouchSpeed = 3f;
    private bool isCrouching => input.Crouch && IsGrounded;

    [Header("Dash Settings")]
    public float dashTime = 1f;
    private float dashTimeLeft;
    private float lastDash = -10f;
    public float dashSpeed = 2f;
    public float dashCoolDown = 1f;
    float dashDirection => Mathf.Sign(input.AxesX) == 0 ? transform.localScale.x : Mathf.Sign(input.AxesX);//如果不移动，默认向面对的方向冲刺
    private bool canPlayerDash => CompareTag("PlayerB");
    private bool IsDashPressed => input.Dash;
    private bool isDashing = false;


    [Header("Jump Settings")]
    public float jumpForce = 7f;
    public float airJumpForce = 5f;
    public float coyoteTime = 0.1f;
    private float coyoteTimeCounter;
    private bool canAirJump = true;
    private bool isJumping;

    [Header("Climb Wall Settings")]
    public float climbSlipSpeed = 3f;
    public float wallJumpForce = 10f;
    public float wallJumpControlDelay = 0.2f;
    private bool isWallSliding;
    private bool isWallJumping;

    [Header("Climb Else Settings")]
    private bool canClimb = false;

    [Header("Input Buffer")]
    public float jumpBufferTime = 0.1f;
    private bool hasJumpBuffer;

    [Header("Corner Correction")]
    public float raycastLength = 0.7f;
    public Vector3 cornerRaycastOffset = new Vector3(0.7f, 0, 0);
    public Vector3 innerRaycastOffset = new Vector3(0.25f, 0, 0);
    public LayerMask groundLayer;
    private bool cornerCorrect;

    [Header("Camera Settings")]
    public CinemachineVirtualCamera virtualCamera;
    public float scrollSpeed = 1f;
    public float minCameraSize = 1f;
    public float maxCameraSize = 20f;

    [Header("Umbrella")]
    public bool isHaveUmbrella = false;
    [SerializeField] UmbrellaArea umbrella;

    [Header("Hurt Settings")]
    public float invincibleTime = 1f;
    public bool isInvinable = false;

    private bool IsGrounded => groundDetector.IsGrounded;
    private bool IsTouchingWall => wallDetector.IsTouchingWall;
    //private bool IsFalling => rb.velocity.y < 0 && !IsGrounded;

    private void Awake()
    {
        groundDetector = GetComponentInChildren<GroundDetector>();
        wallDetector = GetComponentInChildren<WallDetector>();
        input = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if(umbrella !=null) HandleUmbralla();
        HandleMovement();
        HandleDash();
        if (isDashing) return;
        HandleJump();
        HandleClimb();
        HandleWallSlide();
        HandleWallJump();
        if (virtualCamera != null) HandleCameraZoom();
        HandleCornerCorrection();
        HandleJumpBuffer();
        UpdateAnimations();
        if (currentHealth <= 0 && !isDie) Die();
    }

    private void FixedUpdate()
    {
        Dash();
    }
    #region 移动
    private void HandleMovement()
    {
        float speed = isCrouching ? crouchSpeed : walkSpeed;

        if (input.Move)
        {
            transform.localScale = new Vector3(Mathf.Sign(input.AxesX), 1, 1);
            rb.velocity = new Vector2(speed * input.AxesX, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }
    #endregion

    #region 跳跃
    private void HandleJump()
    {
        // 重置地面状态
        if (IsGrounded)
        {
            canAirJump = true;
            coyoteTimeCounter = coyoteTime;
            isJumping = false;
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // 处理跳跃输入缓冲，防止吞键
        if (input.Jump)
        {
            input.SetJumpInputBufferTimer();
        }

        // 执行跳跃
        if (hasJumpBuffer)
        {
            if (IsGrounded || coyoteTimeCounter > 0)
            {
                GroundJump();
            }
            else if (IsTouchingWall)
            {
                WallJump();
            }
            else if (canAirJump && !isWallJumping)
            {
                AirJump();
            }
        }

        // 短按跳跃时降低高度
        if (input.StopJump && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            isJumping = false;
        }
    }
    #endregion

    #region 冲刺
    void HandleDash()
    {
        if(canPlayerDash && IsDashPressed)
        {
            if (Time.time - lastDash >= dashCoolDown)
            {
                //执行冲刺
                ReadyToDash();
            }

        }
    }

    void ReadyToDash()
    {
        isDashing = true;

        lastDash = Time.time;

        dashTimeLeft = dashTime;
    }

    void Dash()
    {
        if (isDashing)
        {
            if(dashTimeLeft > 0)
            {
                if (rb.velocity.y > 0 && !IsGrounded)
                {
                    rb.velocity = new Vector2(dashSpeed * dashDirection, jumpForce);
                }
                rb.velocity = new Vector2(dashSpeed * dashDirection, rb.velocity.y);

                dashTimeLeft -= Time.deltaTime;

                PlayerBShadowPool.Instance.GetFromPool();
            }
            else
            {
                isDashing = false;
                if (!IsGrounded)
                {
                    rb.velocity = new Vector2(dashSpeed * dashDirection, jumpForce);
                }
            }
        }
    }
    #endregion

    #region 攀爬
    public void EnterClimb()
    {
        canClimb = true;
        rb.gravityScale = 0f;
    }

    void HandleClimb()
    {
        if (canClimb)
        {
            rb.velocity = new Vector2(rb.velocity.x, input.Climb * 3);
        }
    }
    public void ExitClimb()
    {
        canClimb = false;
        rb.gravityScale = 4f;
    }
    #endregion

    #region 跳跃缓冲处理
    private void HandleJumpBuffer()
    {
        if (input.HasJumpInputBuffer && (IsGrounded || IsTouchingWall || canAirJump))
        {
            hasJumpBuffer = true;
            input.HasJumpInputBuffer = false;
        }
        else
        {
            hasJumpBuffer = false;
        }
    }
    #endregion

    #region 不同类型跳跃实现
    private void GroundJump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        isJumping = true;
        anim.Play("Jump");
    }

    private void AirJump()
    {
        rb.velocity = new Vector2(rb.velocity.x, airJumpForce);
        canAirJump = false;
        anim.Play("AirJump");
    }

    private void WallJump()
    {
        float direction = -Mathf.Sign(transform.localScale.x);
        rb.velocity = new Vector2(direction * wallJumpForce, jumpForce);
        isWallJumping = true;
        canAirJump = true;
        StartCoroutine(ResetWallJumpControl());
        anim.Play("ClimbHop");
    }

    private IEnumerator ResetWallJumpControl()
    {
        yield return new WaitForSeconds(wallJumpControlDelay);
        isWallJumping = false;
    }
    #endregion

    #region 墙壁滑落
    private void HandleWallSlide()
    {
        if (IsTouchingWall && !IsGrounded && rb.velocity.y < 0)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, -climbSlipSpeed);
            canAirJump = true;
        }
        else
        {
            isWallSliding = false;
        }
    }
    #endregion

    #region 墙壁跳跃
    private void HandleWallJump()
    {
        if (IsTouchingWall && input.Jump)
        {
            WallJump();
        }
    }
    #endregion

    #region 摄像机缩放
    private void HandleCameraZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            float newSize = virtualCamera.m_Lens.OrthographicSize - scroll * scrollSpeed;
            virtualCamera.m_Lens.OrthographicSize = Mathf.Clamp(newSize, minCameraSize, maxCameraSize);
        }
    }
    #endregion

    #region 角落修正
    private void HandleCornerCorrection()
    {
        bool leftCorner = Physics2D.Raycast(transform.position + cornerRaycastOffset, Vector2.up, raycastLength, groundLayer) &&
                         !Physics2D.Raycast(transform.position + innerRaycastOffset, Vector2.up, raycastLength, groundLayer);

        bool rightCorner = Physics2D.Raycast(transform.position - cornerRaycastOffset, Vector2.up, raycastLength, groundLayer) &&
                          !Physics2D.Raycast(transform.position - innerRaycastOffset, Vector2.up, raycastLength, groundLayer);

        cornerCorrect = leftCorner || rightCorner;

        if (cornerCorrect)
        {
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position - innerRaycastOffset + Vector3.up * raycastLength,
                Vector3.left, raycastLength, groundLayer);

            if (hit.collider != null)
            {
                float adjust = hit.point.x - (transform.position.x - cornerRaycastOffset.x);
                transform.position += new Vector3(adjust, 0, 0);
                return;
            }

            hit = Physics2D.Raycast(
                transform.position + innerRaycastOffset + Vector3.up * raycastLength,
                Vector3.right, raycastLength, groundLayer);

            if (hit.collider != null)
            {
                float adjust = hit.point.x - (transform.position.x + cornerRaycastOffset.x);
                transform.position += new Vector3(adjust, 0, 0);
            }
        }
    }
    #endregion

    #region 是否举伞
    void HandleUmbralla()
    {
        if (CompareTag("PlayerA") && Input.GetKeyDown(KeyCode.Space))
        {
            isHaveUmbrella = !isHaveUmbrella;
            umbrella.gameObject.SetActive(isHaveUmbrella);
        }
    }
    #endregion

    #region 更新动画状态
    private void UpdateAnimations()
    {
        if (isWallJumping)
        {
            anim.Play("ClimbHop");
        }
        else if (isWallSliding)
        {
            anim.Play("ClimbSlip");
        }
        else if (!IsGrounded)
        {
            anim.Play(isJumping ? "Jump" : "Fall");
        }
        else if (isCrouching)
        {
            anim.Play("Crouch");
        }
        else if (input.Move)
        {
            anim.Play("Walk");
        }
        else if (!isDie && !isHurt)
        {
            anim.Play("Idle");
        }
    }
    #endregion

    #region 受伤
    public void PlayerHurt(float damage)
    {
        if (!isInvinable)
        {
            TakeDamage(damage);
            anim.SetTrigger("Hurt");
            isHurt = true;
        }
    }
    public void SetPlayerHurt()
    {
        isHurt = false;
    }
    public IEnumerator PlayerInvincibleCoroutine()
    {
        isInvinable = true;
        yield return new WaitForSeconds(invincibleTime);
        isInvinable = false;
    }
    #endregion

    #region 死亡
    public void Die()
    {
        isDie = true;
        anim.Play("Die");
        //enabled = false;
    }
    #endregion
}
