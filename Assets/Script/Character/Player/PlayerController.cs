<<<<<<< Updated upstream
using Cinemachine;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class PlayerController : HealthController
{
    [SerializeField] private GroundDetector[] groundDetector;
    private WallDetector wallDetector;
    private PlayerInput input;
    private Rigidbody2D rb;
    private Animator anim;

    [Header("Endurance Value")]
    public float enegyDecrease = 10f;
    public float enegyIncreaseBuffer = 1f;
    private Coroutine energyIncreaseCoroutine;

    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 7f;
    public float crouchSpeed = 3f;
    private bool isCrouching => input.Crouch && IsGrounded;

    [Header("Dash Settings")]
    public float dashTime = 1f;
    public float dashSpeed = 2f;
    public float dashCoolDown = 1f;
    public float dashJumpForce = 8f;
    public bool activeDash = false;
    private float dashTimeLeft;
    private float lastDash = -10f;

    float dashDirection => Mathf.Sign(input.AxesX) == 0 ? transform.localScale.x : Mathf.Sign(input.AxesX);//������ƶ���Ĭ������Եķ�����
    private bool canPlayerDash => CompareTag("PlayerB");
    private bool IsDashPressed => input.Dash;
    private bool isDashing = false;

    [Header("Jump Settings")]
    public float jumpForce = 7f;
    public float airJumpForce = 5f;
    public float coyoteTime = 0.1f;
    public float jumpBufferTime = 0.1f;
    public bool activeAirJump = false;
    private float jumpBufferTimeCounter;
    private float coyoteTimeCounter;
    private bool isJumping;
    private bool canAirJump = false;

    [Header("Climb Wall Settings")]
    public float climbSlipSpeed = 3f;
    public float wallJumpForce = 10f;
    public float wallJumpControlDelay = 0.2f;
    public bool activeWallJump = false;
    private bool isWallSliding;
    private bool isWallJumping;

    [Header("Climb Else Settings")]
    public bool canClimb = false;

    [Header("Corner Correction")]
    public float raycastLength = 0.7f;
    public Vector3 cornerRaycastOffset = new Vector3(0.7f, 0, 0);
    public Vector3 innerRaycastOffset = new Vector3(0.25f, 0, 0);
    public LayerMask groundLayer;
    [HideInInspector] public bool canCornerCorrect = true;
    private bool cornerCorrect;

    [Header("Camera Settings")]
    public CinemachineVirtualCamera virtualCamera;
    public GameObject cameraFollow;
    //public float scrollSpeed = 1f;
    //public float minCameraSize = 1f;
    //public float maxCameraSize = 20f;
    private CameraObjectFollow cameraObjectFollow;
    private float fallSpeedYDampingChangeThreshold;

    [Header("Umbrella")]
    public bool activeUseUmbrella = false;
    public bool isHaveUmbrella = false;
    [SerializeField] UmbrellaArea umbrella;

    [Header("Water Move")]
    public bool isInWater = false;
    public bool activeBreath = false;

    [Header("Hurt Settings")]
    public float invincibleTime = 1f;
    private float lastinvincibleTime = -10f;

    private bool IsGrounded
    {
        get
        {
            for (int i = 0; i < groundDetector.Length; i++)
            {
                if (groundDetector[i].IsGrounded)
                {
                    return true;
                }
            }
            return false;
        }
    }

    private bool IsTouchingWall => wallDetector.IsTouchingWall;
    [HideInInspector] public bool IsFacingRight => transform.localScale.x > 0;
    //private bool IsFalling => rb.velocity.y < 0 && !IsGrounded;

    private void Awake()
    {
        wallDetector = GetComponentInChildren<WallDetector>();
        input = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if(cameraFollow!=null) cameraObjectFollow = cameraFollow.GetComponent<CameraObjectFollow>();
    
        if(CameraManager.instance!=null) fallSpeedYDampingChangeThreshold = CameraManager.instance.fallSpeedYDampingChangeThreshold;
    }

    private void Start()
    {
        UpdateEnegy();
        UpdateHealth();
    }

    private void Update()
    {
        // ��������ʱ��
        if (IsGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // ������Ծ���뻺��
        if (input.Jump)
        {
            jumpBufferTimeCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferTimeCounter -= Time.deltaTime;
        }

        if (rb.velocity.y< fallSpeedYDampingChangeThreshold && !CameraManager.instance.IsLerpingYDamping && !CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpYDamping(true);
        }

        if(rb.velocity.y >=0 && !CameraManager.instance.IsLerpingYDamping && CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpedFromPlayerFalling = false;

            CameraManager.instance.LerpYDamping(false);
        }

        HandleEnegy(enegyDecrease);
        if (umbrella != null) HandleUmbralla();
        HandleDash();
        if (isDashing) return;
        //if (virtualCamera != null) HandleCameraZoom();
        UpdateAnimations();
        if (currentHealth <= 0 && !isDie) Die();
    }

    private void FixedUpdate()
    {
        HandleJump();
        HandleMovement();
        HandleClimb();
        HandleWallSlide();
        HandleWallJump();
        if(canCornerCorrect) HandleCornerCorrection();
        Dash();
    }

    #region �ƶ�
    private void HandleMovement()
    {
        if (IsFacingRight && cameraObjectFollow != null) cameraObjectFollow.CallTurn();

        if (isInWater)
        {
            // ��ˮ��ʱȡ������Ӱ��
            rb.gravityScale = 0;

            Vector2 waterMovement = new Vector2(input.AxesX, input.Climb);

            // ������Ӧ����仯
            if (Mathf.Approximately(waterMovement.x, 0) && Mathf.Approximately(waterMovement.y, 0))
            {
                // ��û������ʱ����ֹͣ
                rb.velocity = Vector2.zero;
            }
            else
            {
                rb.velocity = waterMovement * walkSpeed;
                // ���³���
                if (input.AxesX != 0)
                {
                    transform.localScale = new Vector3(Mathf.Sign(input.AxesX), 1, 1);
                }
            }
        }
        else
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
    }

    //��ˮʱ
    public void ExitWater(float jumpForce)
    {
        rb.velocity += new Vector2(0,jumpForce);
    }
    #endregion

    #region ��Ծ
    private void HandleJump()
    {
        if (IsGrounded)
        {
            canAirJump = true;
            isJumping = false;
            isWallJumping = false;
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }

        // ����Ƿ������Ծ
        if (jumpBufferTimeCounter > 0)
        {
            if (coyoteTimeCounter > 0)
            {
                GroundJump();
                jumpBufferTimeCounter = 0;
            }
            else if (activeWallJump && IsTouchingWall)
            {
                WallJump();
                jumpBufferTimeCounter = 0;
            }
            else if (activeAirJump && canAirJump && !isWallJumping && !IsGrounded)
            {
                AirJump();
                jumpBufferTimeCounter = 0;
            }
        }

        // �̰���Ծʱ������Ծ�߶�
        if (input.StopJump && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            coyoteTimeCounter = 0;
            isJumping = false;
        }
    }

    #endregion

    #region ���
    void HandleDash()
    {
        if(canPlayerDash && IsDashPressed && activeDash)
        {
            if (Time.time - lastDash >= dashCoolDown)
            {
                //ִ�г��
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
                    rb.velocity = new Vector2(dashSpeed * dashDirection, dashJumpForce);
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
                    rb.velocity = new Vector2(dashSpeed * dashDirection, dashJumpForce);
                }
            }
        }
    }
    #endregion

    #region ����
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

    #region ��ͬ������Ծʵ��
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

    #region ǽ�ڻ���
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

    #region ǽ����Ծ
    private void HandleWallJump()
    {
        if (IsTouchingWall && input.Jump && activeWallJump)
        {
            WallJump();
        }
    }
    #endregion

    //#region ���������
    //private void HandleCameraZoom()
    //{
    //    float scroll = Input.GetAxis("Mouse ScrollWheel");
    //    if (scroll != 0)
    //    {
    //        float newSize = virtualCamera.m_Lens.OrthographicSize - scroll * scrollSpeed;
    //        virtualCamera.m_Lens.OrthographicSize = Mathf.Clamp(newSize, minCameraSize, maxCameraSize);
    //    }
    //}
    //#endregion

    #region ��������
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position - innerRaycastOffset + Vector3.up * raycastLength,
            transform.position - innerRaycastOffset + Vector3.up * raycastLength + Vector3.left * raycastLength);
        Gizmos.DrawLine(transform.position + innerRaycastOffset + Vector3.up * raycastLength,
            transform.position + innerRaycastOffset + Vector3.up * raycastLength + Vector3.right * raycastLength);
        Gizmos.DrawLine(transform.position + cornerRaycastOffset,transform.position + cornerRaycastOffset + Vector3.up * raycastLength);
        Gizmos.DrawLine(transform.position - cornerRaycastOffset, transform.position - cornerRaycastOffset + Vector3.up * raycastLength);
        Gizmos.DrawLine(transform.position + innerRaycastOffset, transform.position + innerRaycastOffset + Vector3.up * raycastLength);
        Gizmos.DrawLine(transform.position - innerRaycastOffset, transform.position - innerRaycastOffset + Vector3.up * raycastLength);
    }
    #endregion

    #region �Ƿ��ɡ
    void HandleUmbralla()
    {
        if (CompareTag("PlayerA") && Input.GetKeyDown(KeyCode.Space) && activeUseUmbrella)
        {
            isHaveUmbrella = !isHaveUmbrella;
            umbrella.gameObject.SetActive(isHaveUmbrella);
        }
    }
    #endregion

    #region ����ֵ
    private void HandleEnegy(float value)
    {
        if(enegyBar != null)
        {
            if ((jumpBufferTimeCounter > 0 && activeAirJump && canAirJump && !isWallJumping && !IsGrounded && input.Jump) || (jumpBufferTimeCounter > 0 && activeWallJump && IsTouchingWall && input.Jump) || (input.Dash && activeDash))
            {
                if (energyIncreaseCoroutine != null)
                {
                    StopCoroutine(energyIncreaseCoroutine);
                    energyIncreaseCoroutine = null;
                }
                EnegyDecrease(value);
                UpdateEnegy();
            }
            else if (energyIncreaseCoroutine == null)
            {
                energyIncreaseCoroutine = StartCoroutine(EnegyIncreaseCoroutine());
                UpdateEnegy();
            }
        }
    }

    IEnumerator EnegyIncreaseCoroutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < enegyIncreaseBuffer)
        {
            if ((jumpBufferTimeCounter > 0 && activeAirJump && canAirJump && !isWallJumping && !IsGrounded && input.Jump) || (jumpBufferTimeCounter > 0 && activeWallJump && IsTouchingWall && input.Jump) || (input.Dash && activeDash))
            {
                energyIncreaseCoroutine = null;
                yield break;
            }

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        EnegyIncrease();
        UpdateEnegy();

        energyIncreaseCoroutine = null;
    }
    #endregion

    #region ���¶���״̬
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

    #region ����
    public void PlayerHurt(float damage)
    {
        if (Time.time - lastinvincibleTime >= invincibleTime)
        {
            TakeDamage(damage);
            anim.SetTrigger("Hurt");
            isHurt = true;
            UpdateHealth();

            lastinvincibleTime = Time.time;
        }
    }
    public void SetPlayerHurt()
    {
        isHurt = false;
    }
    #endregion

    #region ����
    public void Die()
    {
        isDie = true;
        anim.Play("Die");
        //enabled = false;
    }
    #endregion

    #region �����ٶ�

    public void SetVelocityX(float value)
    {
        rb.velocity = new Vector2(value, rb.velocity.y);
    }

    public void SetVelocityY(float value)
    {
        rb.velocity = new Vector2(rb.velocity.x, value);
    }

    #endregion
}
=======
using Cinemachine;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class PlayerController : HealthController
{
    [SerializeField] private GroundDetector[] groundDetector;
    private WallDetector wallDetector;
    private PlayerInput input;
    private Rigidbody2D rb;
    private Animator anim;

    [Header("Endurance Value")]
    public float enegyDecrease = 10f;
    public float enegyIncreaseBuffer = 1f;
    private Coroutine energyIncreaseCoroutine;

    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 7f;
    public float crouchSpeed = 3f;
    private bool isCrouching => input.Crouch && IsGrounded;

    [Header("Dash Settings")]
    public float dashTime = 1f;
    public float dashSpeed = 2f;
    public float dashCoolDown = 1f;
    public float dashJumpForce = 8f;
    public bool activeDash = false;
    private float dashTimeLeft;
    private float lastDash = -10f;

    float dashDirection => Mathf.Sign(input.AxesX) == 0 ? transform.localScale.x : Mathf.Sign(input.AxesX);//������ƶ���Ĭ������Եķ�����
    private bool canPlayerDash => CompareTag("PlayerB");
    private bool IsDashPressed => input.Dash;
    private bool isDashing = false;

    [Header("Jump Settings")]
    public float jumpForce = 7f;
    public float airJumpForce = 5f;
    public float coyoteTime = 0.1f;
    public float jumpBufferTime = 0.1f;
    public bool activeAirJump = false;
    private float jumpBufferTimeCounter;
    private float coyoteTimeCounter;
    private bool isJumping;
    private bool canAirJump = false;

    [Header("Climb Wall Settings")]
    public float climbSlipSpeed = 3f;
    public float wallJumpForce = 10f;
    public float wallJumpControlDelay = 0.2f;
    public bool activeWallJump = false;
    private bool isWallSliding;
    private bool isWallJumping;

    [Header("Climb Else Settings")]
    public bool canClimb = false;

    [Header("Corner Correction")]
    public float raycastLength = 0.7f;
    public Vector3 cornerRaycastOffset = new Vector3(0.7f, 0, 0);
    public Vector3 innerRaycastOffset = new Vector3(0.25f, 0, 0);
    public LayerMask groundLayer;
    [HideInInspector] public bool canCornerCorrect = true;
    private bool cornerCorrect;

    [Header("Camera Settings")]
    public CinemachineVirtualCamera virtualCamera;
    public GameObject cameraFollow;
    //public float scrollSpeed = 1f;
    //public float minCameraSize = 1f;
    //public float maxCameraSize = 20f;
    private CameraObjectFollow cameraObjectFollow;
    private float fallSpeedYDampingChangeThreshold;

    [Header("Umbrella")]
    public bool activeUseUmbrella = false;
    public bool isHaveUmbrella = false;
    [SerializeField] UmbrellaArea umbrella;

    [Header("Water Move")]
    public bool isInWater = false;
    public bool activeBreath = false;

    [Header("Hurt Settings")]
    public float invincibleTime = 1f;
    private float lastinvincibleTime = -10f;

    private bool IsGrounded
    {
        get
        {
            for (int i = 0; i < groundDetector.Length; i++)
            {
                if (groundDetector[i].IsGrounded)
                {
                    return true;
                }
            }
            return false;
        }
    }

    private bool IsTouchingWall => wallDetector.IsTouchingWall;
    [HideInInspector] public bool IsFacingRight => transform.localScale.x > 0;
    //private bool IsFalling => rb.velocity.y < 0 && !IsGrounded;

    private void Awake()
    {
        wallDetector = GetComponentInChildren<WallDetector>();
        input = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if(cameraFollow!=null) cameraObjectFollow = cameraFollow.GetComponent<CameraObjectFollow>();
    
        if(CameraManager.instance!=null) fallSpeedYDampingChangeThreshold = CameraManager.instance.fallSpeedYDampingChangeThreshold;
    }

    private void Start()
    {
        UpdateEnegy();
        UpdateHealth();
    }

    private void Update()
    {
        // ��������ʱ��
        if (IsGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // ������Ծ���뻺��
        if (input.Jump)
        {
            jumpBufferTimeCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferTimeCounter -= Time.deltaTime;
        }

        if (rb.velocity.y< fallSpeedYDampingChangeThreshold && !CameraManager.instance.IsLerpingYDamping && !CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpYDamping(true);
        }

        if(rb.velocity.y >=0 && !CameraManager.instance.IsLerpingYDamping && CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpedFromPlayerFalling = false;

            CameraManager.instance.LerpYDamping(false);
        }

        HandleEnegy(enegyDecrease);
        if (umbrella != null) HandleUmbralla();
        HandleDash();
        if (isDashing) return;
        //if (virtualCamera != null) HandleCameraZoom();
        UpdateAnimations();
        if (currentHealth <= 0 && !isDie) Die();
    }

    private void FixedUpdate()
    {
        HandleJump();
        HandleMovement();
        HandleClimb();
        HandleWallSlide();
        HandleWallJump();
        if(canCornerCorrect) HandleCornerCorrection();
        Dash();
    }

    #region �ƶ�
    private void HandleMovement()
    {
        if (IsFacingRight && cameraObjectFollow != null) cameraObjectFollow.CallTurn();

        if (isInWater)
        {
            // ��ˮ��ʱȡ������Ӱ��
            rb.gravityScale = 0;

            Vector2 waterMovement = new Vector2(input.AxesX, input.Climb);

            // ������Ӧ����仯
            if (Mathf.Approximately(waterMovement.x, 0) && Mathf.Approximately(waterMovement.y, 0))
            {
                // ��û������ʱ����ֹͣ
                rb.velocity = Vector2.zero;
            }
            else
            {
                rb.velocity = waterMovement * walkSpeed;
                // ���³���
                if (input.AxesX != 0)
                {
                    transform.localScale = new Vector3(Mathf.Sign(input.AxesX), 1, 1);
                }
            }
        }
        else
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
    }

    //��ˮʱ
    public void ExitWater(float jumpForce)
    {
        rb.velocity += new Vector2(0,jumpForce);
    }
    #endregion

    #region ��Ծ
    private void HandleJump()
    {
        if (IsGrounded)
        {
            canAirJump = true;
            isJumping = false;
            isWallJumping = false;
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }

        // ����Ƿ������Ծ
        if (jumpBufferTimeCounter > 0)
        {
            if (coyoteTimeCounter > 0)
            {
                GroundJump();
                jumpBufferTimeCounter = 0;
            }
            else if (activeWallJump && IsTouchingWall)
            {
                WallJump();
                jumpBufferTimeCounter = 0;
            }
            else if (activeAirJump && canAirJump && !isWallJumping && !IsGrounded)
            {
                AirJump();
                jumpBufferTimeCounter = 0;
            }
        }

        // �̰���Ծʱ������Ծ�߶�
        if (input.StopJump && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            coyoteTimeCounter = 0;
            isJumping = false;
        }
    }

    #endregion

    #region ���
    void HandleDash()
    {
        if(canPlayerDash && IsDashPressed && activeDash)
        {
            if (Time.time - lastDash >= dashCoolDown)
            {
                //ִ�г��
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
                    rb.velocity = new Vector2(dashSpeed * dashDirection, dashJumpForce);
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
                    rb.velocity = new Vector2(dashSpeed * dashDirection, dashJumpForce);
                }
            }
        }
    }
    #endregion

    #region ����
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

    #region ��ͬ������Ծʵ��
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

    #region ǽ�ڻ���
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

    #region ǽ����Ծ
    private void HandleWallJump()
    {
        if (IsTouchingWall && input.Jump && activeWallJump)
        {
            WallJump();
        }
    }
    #endregion

    //#region ���������
    //private void HandleCameraZoom()
    //{
    //    float scroll = Input.GetAxis("Mouse ScrollWheel");
    //    if (scroll != 0)
    //    {
    //        float newSize = virtualCamera.m_Lens.OrthographicSize - scroll * scrollSpeed;
    //        virtualCamera.m_Lens.OrthographicSize = Mathf.Clamp(newSize, minCameraSize, maxCameraSize);
    //    }
    //}
    //#endregion

    #region ��������
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position - innerRaycastOffset + Vector3.up * raycastLength,
            transform.position - innerRaycastOffset + Vector3.up * raycastLength + Vector3.left * raycastLength);
        Gizmos.DrawLine(transform.position + innerRaycastOffset + Vector3.up * raycastLength,
            transform.position + innerRaycastOffset + Vector3.up * raycastLength + Vector3.right * raycastLength);
        Gizmos.DrawLine(transform.position + cornerRaycastOffset,transform.position + cornerRaycastOffset + Vector3.up * raycastLength);
        Gizmos.DrawLine(transform.position - cornerRaycastOffset, transform.position - cornerRaycastOffset + Vector3.up * raycastLength);
        Gizmos.DrawLine(transform.position + innerRaycastOffset, transform.position + innerRaycastOffset + Vector3.up * raycastLength);
        Gizmos.DrawLine(transform.position - innerRaycastOffset, transform.position - innerRaycastOffset + Vector3.up * raycastLength);
    }
    #endregion

    #region �Ƿ��ɡ
    void HandleUmbralla()
    {
        if (CompareTag("PlayerA") && Input.GetKeyDown(KeyCode.Space) && activeUseUmbrella)
        {
            isHaveUmbrella = !isHaveUmbrella;
            umbrella.gameObject.SetActive(isHaveUmbrella);
        }
    }
    #endregion

    #region ����ֵ
    private void HandleEnegy(float value)
    {
        if(enegyBar != null)
        {
            if ((jumpBufferTimeCounter > 0 && activeAirJump && canAirJump && !isWallJumping && !IsGrounded && input.Jump) || (jumpBufferTimeCounter > 0 && activeWallJump && IsTouchingWall && input.Jump) || (input.Dash && activeDash))
            {
                if (energyIncreaseCoroutine != null)
                {
                    StopCoroutine(energyIncreaseCoroutine);
                    energyIncreaseCoroutine = null;
                }
                EnegyDecrease(value);
                UpdateEnegy();
            }
            else if (energyIncreaseCoroutine == null)
            {
                energyIncreaseCoroutine = StartCoroutine(EnegyIncreaseCoroutine());
                UpdateEnegy();
            }
        }
    }

    IEnumerator EnegyIncreaseCoroutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < enegyIncreaseBuffer)
        {
            if ((jumpBufferTimeCounter > 0 && activeAirJump && canAirJump && !isWallJumping && !IsGrounded && input.Jump) || (jumpBufferTimeCounter > 0 && activeWallJump && IsTouchingWall && input.Jump) || (input.Dash && activeDash))
            {
                energyIncreaseCoroutine = null;
                yield break;
            }

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        EnegyIncrease();
        UpdateEnegy();

        energyIncreaseCoroutine = null;
    }
    #endregion

    #region ���¶���״̬
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

    #region ����
    public void PlayerHurt(float damage)
    {
        if (Time.time - lastinvincibleTime >= invincibleTime)
        {
            TakeDamage(damage);
            anim.SetTrigger("Hurt");
            isHurt = true;
            UpdateHealth();

            lastinvincibleTime = Time.time;
        }
    }
    public void SetPlayerHurt()
    {
        isHurt = false;
    }
    #endregion

    #region ����
    public void Die()
    {
        isDie = true;
        anim.Play("Die");
        //enabled = false;
    }
    #endregion

    #region �����ٶ�

    public void SetVelocityX(float value)
    {
        rb.velocity = new Vector2(value, rb.velocity.y);
    }

    public void SetVelocityY(float value)
    {
        rb.velocity = new Vector2(rb.velocity.x, value);
    }

    #endregion
}
>>>>>>> Stashed changes
