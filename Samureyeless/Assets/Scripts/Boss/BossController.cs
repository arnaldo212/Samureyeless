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

    [Header("Patrulha")]
    public Vector2 patrolPointA = new Vector2(-3f, 0f);
    public Vector2 patrolPointB = new Vector2(3f, 0f);
    public float patrolSpeed = 1.5f;
    public float patrolWaitTime = 1f;


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
}
