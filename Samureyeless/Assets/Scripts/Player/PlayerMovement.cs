using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimentação horizontal")]
    private Rigidbody2D rb;
    //[SerializeField] private float walkSpeed = 1;
    private float xAxis;
    private int jumpBufferCounter = 5;
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

    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;

    [Header("Configs de Ataque")]
    [SerializeField] Transform SideAttackTransform;
    [SerializeField] Vector2 SideAttackArea;
    [SerializeField] LayerMask attacableLayer;
    bool attack = false;
    [SerializeField] private float timeBetweenAtack;
    private float timeSinceAttack = 0.5f;
    

    
    private float lastOnGroundTime;
    private bool isJumpFalling;
    PlayerStateList pState;
    private bool canDash = true;
    private bool dashed;
    private float gravity;

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
        gravity = rb.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        GetInputs();
        UpdateJumpVariables();
        if (pState.dashing) return;
        StartDash();
        Attack();
    }

    private void FixedUpdate() {
        Run(lerpAmount);
        HandleJump();
    }

    void GetInputs() {
        xAxis = Input.GetAxisRaw("Horizontal");

        attack = Input.GetButtonDown("Attack");

        if (attack)
        {
            Debug.Log("ataq");
        }

        lastOnGroundTime -= Time.deltaTime;
        if (Grounded())
        {
            lastOnGroundTime = 0.1f;
        }
    }
    //movimento principal do personagem
    private void Run(float lerpAmount) {
        if (pState.dashing) return; //não mexe quando da dash

        float targetSpeed = xAxis * runMaxSpeed;

        //calculo adaptativo do lerpAmount
        float adaptiveLerp;
        if (Grounded())
        {
            adaptiveLerp = 0.8f; //valor mais alto para resposta rápida no chão
        }
        else
        {
            adaptiveLerp = 0.3f; //valor mais baixo para movimento suave no ar
        }

        //suaviza o movimento
        targetSpeed = Mathf.Lerp(rb.linearVelocity.x, targetSpeed, adaptiveLerp);

        targetSpeed = Mathf.Lerp(rb.linearVelocity.x, targetSpeed, lerpAmount);

        //decide se acelara o freia
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

        //mantém o momentum no ar
        if (doConserveMomentum && Mathf.Abs(rb.linearVelocity.x) > Mathf.Abs(targetSpeed) &&
            Mathf.Sign(rb.linearVelocity.x) == Mathf.Sign(targetSpeed) &&
            Mathf.Abs(targetSpeed) > 0.01f && lastOnGroundTime < 0)
        {
            accelRate = 0;
        }

        //aplica a força calculada
        float speedDif = targetSpeed - rb.linearVelocity.x;
        float movement = speedDif * accelRate;
        rb.AddForce(movement * Vector2.right, ForceMode2D.Force);

        //vira o sprite
        if (xAxis > 0.01f)
        {
            transform.localScale = Vector3.one;
        }
        else if (xAxis < -0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    void StartDash() {
        if(Input.GetButtonDown("Dash") && canDash && !dashed)
        {
            StartCoroutine(Dash());
            dashed = true;
        }

        if (Grounded())
        {
            dashed = false;
        }
    }

    IEnumerator Dash() {
        canDash = false;
        pState.dashing = true;
        rb.gravityScale = 0;
        rb.linearVelocity = new Vector3(transform.localScale.x * dashSpeed, 0);
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity;
        pState.dashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void HandleJump() {
        //pulo curto - se soltar o botão cedo, pula menos
        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0)
        {
            if (rb.linearVelocity.y > jumpForce * 0.4f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * 0.4f);
            }
            isJumpFalling = true;
        }

        //pulo normal
        if (!pState.jumping && jumpBufferCounter > 0 && coyoteTimeCounter > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            pState.jumping = true;
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

    void Attack() {
        timeSinceAttack += Time.deltaTime;
        if(attack && timeSinceAttack >= timeBetweenAtack)
        {
            timeSinceAttack = 0;
            //animação do ataque
        }

        Hit(SideAttackTransform, SideAttackArea);
        
    }

    private void Hit(Transform attackTransform, Vector2 attackArea) {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(attackTransform.position, attackArea, 0, attacableLayer);

        //if (objectsToHit.Length > 0) {
           // Debug.Log("hit");
        //}
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(SideAttackTransform.position, SideAttackArea);
    }
}
