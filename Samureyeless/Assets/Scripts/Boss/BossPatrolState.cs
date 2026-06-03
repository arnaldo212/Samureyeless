using UnityEngine;

public class BossPatrolState : BossState
{
    private Vector2 targetPoint;
    private float waitTimer = 0f;
    private bool isWaiting = false;
    private bool goingToB = true;

    public BossPatrolState(BossController boss) : base(boss) { }

    public override void Enter() {
        Debug.Log("Boss: Patrolling");
        isWaiting = false;
        waitTimer = 0f;
        // Vai para o ponto mais distante da posição atual
        float distToA = Mathf.Abs(boss.transform.position.x - boss.patrolPointA.x);
        float distToB = Mathf.Abs(boss.transform.position.x - boss.patrolPointB.x);
        goingToB = distToA > distToB; // vai para o mais próximo primeiro
        SetNextPatrolPoint();
    }

    public override void Update() {
        //detectou o jogador
        float distToPlayer = Vector2.Distance(boss.transform.position, boss.Player.position);
        if (distToPlayer <= boss.detectionRange)
        {
            boss.ChangeState(boss.ChaseState);
            return;
        }

        //aguardando no ponto de patrulha
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;
                goingToB = !goingToB;
                SetNextPatrolPoint();
                Debug.Log("Indo para novo ponto: " + targetPoint);
            }
            return;
        }

        //move em direção ao ponto alvo
        float distToTarget = Mathf.Abs(boss.transform.position.x - targetPoint.x);
        if (distToTarget <= 0.5f)
        {
            // Chegou no ponto, espera antes de ir ao próximo
            boss.Rb.linearVelocity = Vector2.zero;
            isWaiting = true;
            waitTimer = boss.patrolWaitTime;
            Debug.Log("Chegou no ponto! goingToB: " + goingToB);
            return;
        }

        Vector2 direction = (targetPoint - (Vector2)boss.transform.position).normalized;
        boss.Rb.linearVelocity = new Vector2(direction.x * boss.patrolSpeed, boss.Rb.linearVelocity.y);

        //vira o sprite na direção certa
        boss.FaceDirection(direction.x);
    }

    public override void Exit() {
        boss.Rb.linearVelocity = Vector2.zero;
    }
    private void SetNextPatrolPoint() {
        //alterna entre os dois pontos de patrulha //depois tem que corrigir os pontos para o lugar certo
        targetPoint = goingToB ? boss.patrolPointB : boss.patrolPointA;
    }
}