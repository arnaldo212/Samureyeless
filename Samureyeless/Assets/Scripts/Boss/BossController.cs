using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BossController : MonoBehaviour
{
    [Header("Referências")]
    public Transform Player;
    public Rigidbody2D Rb { get; private set; }

    [Header("Parâmetros")]
    public float moveSpeed = 3f;
    public float detectionRange = 8f;
    public float attackRange = 1.5f;
    public float attackDuration = 0.8f;
    public float attackCooldown = 1.5f;

    [Header("Dano")]
    public BossHitBox HitBox;
    public float attackDamage = 20f;

    [Header("Patrulha")]
    public Vector2 patrolPointA = new Vector2(-3f, 0f);
    public Vector2 patrolPointB = new Vector2(3f, 0f);
    public float patrolSpeed = 1.5f;
    public float patrolWaitTime = 1f;

    [Header("Parry")]
    public float parryWindowDuration = 0.4f; // duração da janela de parry
    public bool isParryWindow = false;        // true durante a janela
    public float parryStunDuration = 2f;     // tempo que o boss fica stunado
    public float telegraphDuration = 1.5f;

    [Header("Telegraph Visual")]
    public SpriteRenderer spriteRenderer;
    public Color telegraphColor = Color.red;
    public Color stunColor = Color.yellow;


    public BossIdleState IdleState { get; private set; }
    public BossChaseState ChaseState { get; private set; }
    public BossAttackState AttackState { get; private set; }
    public BossPatrolState PatrolState { get; private set; }

    private BossState currentState;

    private void Awake() {
        Rb = GetComponent<Rigidbody2D>();

        //instancia os estados passando referência do controller
        IdleState = new BossIdleState(this);
        ChaseState = new BossChaseState(this);
        AttackState = new BossAttackState(this);
        PatrolState = new BossPatrolState(this);
    }

    private void Start() {
        //busca o jogador automaticamente se não foi assignado
        if (Player == null)
            Player = GameObject.FindGameObjectWithTag("Player").transform;

        ChangeState(IdleState);//talvez deve começar paatrulhando?
    }

    private void Update() {
        currentState?.Update();
    }

    public void ChangeState(BossState newState) {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    //visualiza os ranges 
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        //pontos de patrulha no editor
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(patrolPointA, 0.2f);
        Gizmos.DrawSphere(patrolPointB, 0.2f);
        Gizmos.DrawLine(patrolPointA, patrolPointB);
    }

    public void FaceDirection(float directionX) {
        if (directionX == 0) return;
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * Mathf.Sign(directionX) * -1f;
        transform.localScale = scale;
    }

    public void TriggerParry() {
        ChangeState(new BossStunState(this, parryStunDuration)); 
    }
}
