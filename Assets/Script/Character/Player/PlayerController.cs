<<<<<<< Updated upstream
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : HealthController
{
    GroundDetector groundDetector;

    WallDetector wallDetector;

    PlayerInput input;

    Rigidbody2D rb;

    Animator anim;

    [Header("Player Move")]
    public float walkSpeed = 5f;
    public  float runSpeed = 7f;

    [Header("Jump Corner Correct")]
    public float raycastLength = 0.7f;
    public Vector3 cornerRaycastPos = new Vector3(0.7f, 0, 0);
    public Vector3 innerRaycastPos = new Vector3(0.25f, 0, 0);
    public bool cornerCorrect;
    public LayerMask ground;

    [Header("Player Camera Controller")]
    public CinemachineVirtualCamera virtualCamera;
    public float scrollSpeed = 1.0f;
    public float minScale = 1f;
    public float maxScale = 20f;
    public AudioSource VoicePlayer { get; private set; }

    public bool isHurting = false;
    public bool CanAirJump { get; set; } = true;
    public bool CanWallJump { get; set; } = true;
    public bool IsGrounded => groundDetector.IsGrounded;
    public bool IsFalling => rb.velocity.y < 0f && !IsGrounded;
    public bool IsTouchingWall => wallDetector.IsTouchingWall;

    public float MoveSpeed => Mathf.Abs(rb.velocity.x);

    private void Awake()
    {
        groundDetector = GetComponentInChildren<GroundDetector>();
        wallDetector = GetComponentInChildren<WallDetector>();
        input = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        VoicePlayer = GetComponentInChildren<AudioSource>();
    }

    //private void Start()
    //{
    //    input.EnableGameplayInputs();
    //}

    private void Update()
    {
        //Camera Controller
        UpdateCameraScale();
        //Jump Corner Correct
        RaycastCollision();
        if (cornerCorrect)
        {
            CornerCorrect(rb.velocity.y);
            Debug.Log("Corner Correct");
        }
        if (currentHealth <= 0 && !isDie)
        {
            Die();
        }
    }

    private void UpdateCameraScale()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput != 0)
        {
            float newSize = virtualCamera.m_Lens.OrthographicSize - scrollInput * scrollSpeed;

            newSize = Mathf.Clamp(newSize, minScale, maxScale);

            virtualCamera.m_Lens.OrthographicSize = newSize;
        }
    }

    #region Move
    public void Move(float speed) 
    {
        if (input.Move)
        { 
            transform.localScale = new Vector3(Mathf.Sign(input.AxesX), 1f, 1f);
        }
        SetVelocityX(speed * input.AxesX);
    }
    #endregion

    #region Jump Corner Correct
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position - innerRaycastPos + Vector3.up * raycastLength,
                        transform.position - innerRaycastPos + Vector3.up * raycastLength + Vector3.left * raycastLength);
        Gizmos.DrawLine(transform.position + innerRaycastPos + Vector3.up * raycastLength,
                        transform.position + innerRaycastPos + Vector3.up * raycastLength + Vector3.right * raycastLength);
        Gizmos.DrawLine(transform.position + cornerRaycastPos, transform.position + cornerRaycastPos + Vector3.up * raycastLength);
        Gizmos.DrawLine(transform.position - cornerRaycastPos, transform.position - cornerRaycastPos + Vector3.up * raycastLength);
        Gizmos.DrawLine(transform.position + innerRaycastPos, transform.position + innerRaycastPos + Vector3.up * raycastLength);
        Gizmos.DrawLine(transform.position - innerRaycastPos, transform.position - innerRaycastPos + Vector3.up * raycastLength);
    }

    void RaycastCollision()
    {
        cornerCorrect = Physics2D.Raycast(transform.position + cornerRaycastPos, Vector2.up, raycastLength, ground) &&
                        !Physics2D.Raycast(transform.position + innerRaycastPos, Vector2.up, raycastLength, ground) ||
                        Physics2D.Raycast(transform.position - cornerRaycastPos, Vector2.up, raycastLength, ground) &&
                        !Physics2D.Raycast(transform.position - innerRaycastPos, Vector2.up, raycastLength, ground);
    }

    void CornerCorrect(float Yvelocity)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position - innerRaycastPos + Vector3.up * raycastLength,
                                             Vector3.left, raycastLength, ground);
        if (hit.collider != null)
        {
            float newPos = hit.point.x - (transform.position.x - cornerRaycastPos.x);
            transform.position = new Vector3(transform.position.x + newPos, transform.position.y, 0);
            rb.velocity = new Vector2(rb.velocity.x, Yvelocity);
            return;
        }

        hit = Physics2D.Raycast(transform.position + innerRaycastPos + Vector3.up * raycastLength,
                                             Vector3.right, raycastLength, ground);
        if (hit.collider != null)
        {
            float newPos = hit.point.x - (transform.position.x + cornerRaycastPos.x);
            transform.position = new Vector3(transform.position.x + newPos, transform.position.y, 0);
            rb.velocity = new Vector2(rb.velocity.x, Yvelocity);
            return;
        }
    }
    #endregion

    //���ˣ������������״̬��ʵ��
    #region Hurt
    public void PlayerHurt(float damage)
    {
        anim.SetTrigger("Hurt");
        TakeDamage(damage);
    }
    #endregion

    #region Die
    public void Die()
    {
        isDie = true;
        anim.Play("Die");
        //����������߼����������UI��
    }
    #endregion

    public void SetVelocity(Vector2 velocity)
    {
        rb.velocity = velocity;
    }

    public void SetVelocityX(float velocityX)
    {
        rb.velocity = new Vector2(velocityX, rb.velocity.y);
    }

    public void SetVelocityY(float velocityY)
    {
        rb.velocity = new Vector2(rb.velocity.x, velocityY);
    }

    public void SetGravity(float value)
    {
        rb.gravityScale = value;
    }
}
=======
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
    private bool isRunning => input.Run;

    [Header("Jump Settings")]
    public float jumpForce = 7f;
    public float airJumpForce = 5f;
    public float coyoteTime = 0.1f;
    private float coyoteTimeCounter;
    private bool canAirJump = true;
    private bool isJumping;

    [Header("Climb Settings")]
    public float climbSlipSpeed = 3f;
    public float wallJumpForce = 10f;
    public float wallJumpControlDelay = 0.2f;
    private bool isWallSliding;
    private bool isWallJumping;

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
        HandleMovement();
        HandleJump();
        HandleWallSlide();
        HandleWallJump();
        if (virtualCamera != null) HandleCameraZoom();
        HandleCornerCorrection();
        HandleJumpBuffer();
        UpdateAnimations();
        if (currentHealth <= 0 && !isDie) Die();
    }

    #region ����
    private void HandleMovement()
    {
        float speed = isCrouching ? crouchSpeed :
                     isRunning ? runSpeed : walkSpeed;

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

    #region ���
    private void HandleJump()
    {
        // ���õ���״̬
        if (IsGrounded)
        {
            canAirJump = true;
            coyoteTimeCounter = coyoteTime;
            isJumping = false;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // ������Ծ���뻺��
        if (input.Jump)
        {
            input.SetJumpInputBufferTimer();
        }

        // ִ����Ծ
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

        // �̰���Ծʱ���͸߶�
        if (input.StopJump && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            isJumping = false;
        }
    }
    #endregion

    #region ��Ծ���崦��
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
        if (IsTouchingWall && input.Jump)
        {
            WallJump();
        }
    }
    #endregion

    #region ���������
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
            anim.Play(isRunning ? "Run" : "Walk");
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
        isHurt = true;
        anim.SetTrigger("Hurt");
        TakeDamage(damage);
        //rb.velocity = new Vector2(-transform.localScale.x * 3f, 5f);����
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
        enabled = false;
    }
    #endregion
}

>>>>>>> Stashed changes
