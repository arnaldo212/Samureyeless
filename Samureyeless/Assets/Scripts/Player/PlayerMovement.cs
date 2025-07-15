using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimentação horizontal")]
    private Rigidbody2D rb;
    [SerializeField] private float walkSpeed = 1;
    private float xAxis;
    private int jumpBufferCounter = 0;
    [SerializeField] private float jumpForce = 45;
    [SerializeField] private int jumpBufferFrames;
    private float coyoteTimeCounter = 0;
    [SerializeField] private float coyoteTime;

    [Header("Advanced Movement Settings")]
    [SerializeField] private float runMaxSpeed = 15f;
    [SerializeField] private float runAccelAmount = 50f;
    [SerializeField] private float runDeccelAmount = 30f;
    [SerializeField] private float accelInAir = 0.6f;
    [SerializeField] private float deccelInAir = 0.4f;
    [SerializeField] private float jumpHangTimeThreshold = 1f;
    [SerializeField] private float jumpHangAccelerationMult = 1.2f;
    [SerializeField] private float jumpHangMaxSpeedMult = 1.1f;
    [SerializeField] private bool doConserveMomentum = true;
    [SerializeField] private float lerpAmount = 1f;

    [Header("Confere se está no chão")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] private LayerMask whatIsGround;

    // State variables
    private float lastOnGroundTime;
    private bool isJumpFalling;
    PlayerStateList pState;

    public static PlayerMovement Instance;
    private void Awake() {
        // garantir que só exista um playermovement na cena
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);//se ja existir outro esse se destroi
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pState = GetComponent<PlayerStateList>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInputs();
        UpdateJumpVariables();
        //Move();
        //Jump();
    }

    private void FixedUpdate() {
        Run(lerpAmount);
        HandleJump();
    }

    void GetInputs() {
        xAxis = Input.GetAxisRaw("Horizontal");

        lastOnGroundTime -= Time.deltaTime;
        if (Grounded())
        {
            lastOnGroundTime = 0.1f;
        }
    }
    private void Run(float lerpAmount) {
        // Calculate target speed
        float targetSpeed = xAxis * runMaxSpeed;

        // Cálculo adaptativo do lerpAmount
        float adaptiveLerp;
        if (Grounded())
        {
            adaptiveLerp = 0.8f; // Valor mais alto para resposta rápida no chão
        }
        else
        {
            adaptiveLerp = 0.3f; // Valor mais baixo para movimento suave no ar
        }

        targetSpeed = Mathf.Lerp(rb.linearVelocity.x, targetSpeed, adaptiveLerp);

        targetSpeed = Mathf.Lerp(rb.linearVelocity.x, targetSpeed, lerpAmount);

        // Calculate acceleration rate
        float accelRate;

        if (lastOnGroundTime > 0)
        {
            if (Mathf.Abs(targetSpeed) > 0.01f)
            {
                accelRate = runAccelAmount;
            }
            else
            {
                accelRate = runDeccelAmount;
            }
        }
        else
        {
            if (Mathf.Abs(targetSpeed) > 0.01f)
            {
                accelRate = runAccelAmount * accelInAir;
            }
            else
            {
                accelRate = runDeccelAmount * deccelInAir;
            }
        }

        // Jump hang modifier
        if ((pState.jumping || isJumpFalling) && Mathf.Abs(rb.linearVelocity.y) < jumpHangTimeThreshold)
        {
            accelRate *= jumpHangAccelerationMult;
            targetSpeed *= jumpHangMaxSpeedMult;
        }

        // Momentum conservation
        if (doConserveMomentum && Mathf.Abs(rb.linearVelocity.x) > Mathf.Abs(targetSpeed) &&
            Mathf.Sign(rb.linearVelocity.x) == Mathf.Sign(targetSpeed) &&
            Mathf.Abs(targetSpeed) > 0.01f && lastOnGroundTime < 0)
        {
            accelRate = 0;
        }

        // Apply movement force
        float speedDif = targetSpeed - rb.linearVelocity.x;
        float movement = speedDif * accelRate;
        rb.AddForce(movement * Vector2.right, ForceMode2D.Force);

        // Flip character sprite
        if (xAxis > 0.01f)
        {
            transform.localScale = Vector3.one;
        }
        else if (xAxis < -0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void HandleJump() {
        // Reduce jump height if button released early
        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0)
        {
            if (rb.linearVelocity.y > jumpForce * 0.4f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * 0.4f);
            }
            //pState.jumping = false;
            isJumpFalling = true;
        }

        // Normal jump
        if (!pState.jumping && jumpBufferCounter > 0 && coyoteTimeCounter > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            pState.jumping = true;
           // isJumpFalling = false;
            jumpBufferCounter = 0;
        }

        
    }

    void UpdateJumpVariables() {
        if (Grounded())
        {
            pState.jumping = false;
            isJumpFalling = false;
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferFrames;
        }
        else if (jumpBufferCounter > 0)
        {
            jumpBufferCounter--;
        }
    }

    private void Move() {
        rb.linearVelocity = new Vector2(walkSpeed * xAxis, rb.linearVelocity.y);

        if(xAxis > 0.01f)
        {
            transform.localScale = Vector3.one;
        }else if(xAxis < -0.01f)//se estiver indo pra esquesda, inverte o sprite
        {
            transform.localScale = new Vector3(-1 , 1, 1);
        }
    }

    public bool Grounded() {
        //faz tres raycast para checar se tem chão, um no centro, um na direita e um na esquerda do ponto de verificação
        if(Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround) 
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void Jump() {
        //pula menos dependendo de quanto apertar espaço
        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);

            pState.jumping = false;
        }
        //pulo "normal"
        if (!pState.jumping)
        {
            if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce);

                pState.jumping = true;
            }
        }
    }

    void UpdateJumpVariables2() {
        if (Grounded())
        {
            pState.jumping = false;
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferFrames;
        }
        else
        {
            jumpBufferCounter--;
        }
    }

}
