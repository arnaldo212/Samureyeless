using UnityEngine;

public class BossPatrolState : BossState
{
    private Vector2 targetPoint;
    private float waitTimer = 0f;
    private bool isWaiting = false;

    public BossPatrolState(BossController boss) : base(boss) { }

    public override void Enter() {
        Debug.Log("Boss: Patrolling");
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
                SetNextPatrolPoint();
            }
            return;
        }

        //move em direção ao ponto alvo
        float distToTarget = Vector2.Distance(boss.transform.position, targetPoint);
        if (distToTarget <= 0.2f)
        {
            // Chegou no ponto, espera antes de ir ao próximo
            boss.Rb.linearVelocity = Vector2.zero;
            isWaiting = true;
            waitTimer = boss.patrolWaitTime;
            return;
        }

        Vector2 direction = (targetPoint - (Vector2)boss.transform.position).normalized;
        boss.Rb.linearVelocity = new Vector2(direction.x * boss.patrolSpeed, boss.Rb.linearVelocity.y);

        //vira o sprite na direção certa
        if (direction.x != 0)
        {
            Vector3 scale = boss.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(direction.x);
            boss.transform.localScale = scale;
        }
    }

    public override void Exit() {
        boss.Rb.linearVelocity = Vector2.zero;
    }

    private void SetNextPatrolPoint() {
        //alterna entre os dois pontos de patrulha //depois tem que corrigir os pontos para o lugar certo
        float currentX = boss.transform.position.x;
        float distToA = Mathf.Abs(currentX - boss.patrolPointA.x);
        float distToB = Mathf.Abs(currentX - boss.patrolPointB.x);

        targetPoint = distToA > distToB ? boss.patrolPointA : boss.patrolPointB;
    }
}