using UnityEngine;

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

    // Estados
    public BossIdleState IdleState { get; private set; }
    public BossChaseState ChaseState { get; private set; }
    public BossAttackState AttackState { get; private set; }

    private BossState currentState;

    private void Awake() {
        Rb = GetComponent<Rigidbody2D>();

        // Instancia os estados passando referência do controller
        IdleState = new BossIdleState(this);
        ChaseState = new BossChaseState(this);
        AttackState = new BossAttackState(this);
    }

    private void Start() {
        // Busca o jogador automaticamente se não foi assignado
        if (Player == null)
            Player = GameObject.FindGameObjectWithTag("Player").transform;

        ChangeState(IdleState);
    }

    private void Update() {
        currentState?.Update();
    }

    public void ChangeState(BossState newState) {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    // Visualiza os ranges no Editor
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
