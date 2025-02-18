//using Cinemachine;
//using Unity.VisualScripting;
//using UnityEngine;

//public class PlayerController : HealthController
//{
//    GroundDetector groundDetector;

//    WallDetector wallDetector;

//    PlayerInput input;

//    Rigidbody2D rb;

//    Animator anim;

//    [Header("Player Move")]
//    public float walkSpeed = 5f;
//    public  float runSpeed = 7f;

//    [Header("Jump Corner Correct")]
//    public float raycastLength = 0.7f;
//    public Vector3 cornerRaycastPos = new Vector3(0.7f, 0, 0);
//    public Vector3 innerRaycastPos = new Vector3(0.25f, 0, 0);
//    public bool cornerCorrect;
//    public LayerMask ground;

//    [Header("Player Camera Controller")]
//    public CinemachineVirtualCamera virtualCamera;
//    public float scrollSpeed = 1.0f;
//    public float minScale = 1f;
//    public float maxScale = 20f;
//    public AudioSource VoicePlayer { get; private set; }

//    public bool isHurting = false;
//    public bool CanAirJump { get; set; } = true;
//    public bool CanWallJump { get; set; } = true;
//    public bool IsGrounded => groundDetector.IsGrounded;
//    public bool IsFalling => rb.velocity.y < 0f && !IsGrounded;
//    public bool IsTouchingWall => wallDetector.IsTouchingWall;

//    public float MoveSpeed => Mathf.Abs(rb.velocity.x);

//    private void Awake()
//    {
//        groundDetector = GetComponentInChildren<GroundDetector>();
//        wallDetector = GetComponentInChildren<WallDetector>();
//        input = GetComponent<PlayerInput>();
//        rb = GetComponent<Rigidbody2D>();
//        anim = GetComponent<Animator>();
//        VoicePlayer = GetComponentInChildren<AudioSource>();
//    }

//    //private void Start()
//    //{
//    //    input.EnableGameplayInputs();
//    //}

//    private void Update()
//    {
//        //Camera Controller
//        UpdateCameraScale();
//        //Jump Corner Correct
//        RaycastCollision();
//        if (cornerCorrect)
//        {
//            CornerCorrect(rb.velocity.y);
//            Debug.Log("Corner Correct");
//        }
//        if (currentHealth <= 0 && !isDie)
//        {
//            Die();
//        }
//    }

//    private void UpdateCameraScale()
//    {
//        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

//        if (scrollInput != 0)
//        {
//            float newSize = virtualCamera.m_Lens.OrthographicSize - scrollInput * scrollSpeed;

//            newSize = Mathf.Clamp(newSize, minScale, maxScale);

//            virtualCamera.m_Lens.OrthographicSize = newSize;
//        }
//    }

//    #region Move
//    public void Move(float speed) 
//    {
//        if (input.Move)
//        { 
//            transform.localScale = new Vector3(Mathf.Sign(input.AxesX), 1f, 1f);
//        }
//        SetVelocityX(speed * input.AxesX);
//    }
//    #endregion

//    #region Jump Corner Correct
//    private void OnDrawGizmos()
//    {
//        Gizmos.DrawLine(transform.position - innerRaycastPos + Vector3.up * raycastLength,
//                        transform.position - innerRaycastPos + Vector3.up * raycastLength + Vector3.left * raycastLength);
//        Gizmos.DrawLine(transform.position + innerRaycastPos + Vector3.up * raycastLength,
//                        transform.position + innerRaycastPos + Vector3.up * raycastLength + Vector3.right * raycastLength);
//        Gizmos.DrawLine(transform.position + cornerRaycastPos, transform.position + cornerRaycastPos + Vector3.up * raycastLength);
//        Gizmos.DrawLine(transform.position - cornerRaycastPos, transform.position - cornerRaycastPos + Vector3.up * raycastLength);
//        Gizmos.DrawLine(transform.position + innerRaycastPos, transform.position + innerRaycastPos + Vector3.up * raycastLength);
//        Gizmos.DrawLine(transform.position - innerRaycastPos, transform.position - innerRaycastPos + Vector3.up * raycastLength);
//    }

//    void RaycastCollision()
//    {
//        cornerCorrect = Physics2D.Raycast(transform.position + cornerRaycastPos, Vector2.up, raycastLength, ground) &&
//                        !Physics2D.Raycast(transform.position + innerRaycastPos, Vector2.up, raycastLength, ground) ||
//                        Physics2D.Raycast(transform.position - cornerRaycastPos, Vector2.up, raycastLength, ground) &&
//                        !Physics2D.Raycast(transform.position - innerRaycastPos, Vector2.up, raycastLength, ground);
//    }

//    void CornerCorrect(float Yvelocity)
//    {
//        RaycastHit2D hit = Physics2D.Raycast(transform.position - innerRaycastPos + Vector3.up * raycastLength,
//                                             Vector3.left, raycastLength, ground);
//        if (hit.collider != null)
//        {
//            float newPos = hit.point.x - (transform.position.x - cornerRaycastPos.x);
//            transform.position = new Vector3(transform.position.x + newPos, transform.position.y, 0);
//            rb.velocity = new Vector2(rb.velocity.x, Yvelocity);
//            return;
//        }

//        hit = Physics2D.Raycast(transform.position + innerRaycastPos + Vector3.up * raycastLength,
//                                             Vector3.right, raycastLength, ground);
//        if (hit.collider != null)
//        {
//            float newPos = hit.point.x - (transform.position.x + cornerRaycastPos.x);
//            transform.position = new Vector3(transform.position.x + newPos, transform.position.y, 0);
//            rb.velocity = new Vector2(rb.velocity.x, Yvelocity);
//            return;
//        }
//    }
//    #endregion

//    //���ˣ������������״̬��ʵ��
//    #region Hurt
//    public void PlayerHurt(float damage)
//    {
//        anim.SetTrigger("Hurt");
//        TakeDamage(damage);
//    }
//    #endregion

//    #region Die
//    public void Die()
//    {
//        isDie = true;
//        anim.Play("Die");
//        //����������߼����������UI��
//    }
//    #endregion

//    public void SetVelocity(Vector2 velocity)
//    {
//        rb.velocity = velocity;
//    }

//    public void SetVelocityX(float velocityX)
//    {
//        rb.velocity = new Vector2(velocityX, rb.velocity.y);
//    }

//    public void SetVelocityY(float velocityY)
//    {
//        rb.velocity = new Vector2(rb.velocity.x, velocityY);
//    }

//    public void SetGravity(float value)
//    {
//        rb.gravityScale = value;
//    }
//}
