using UnityEngine;

public class BossIdleState : BossState
{
    public BossIdleState(BossController boss) : base(boss) { }

    public override void Enter() {
        Debug.Log("Boss: Idle");
        // animação de idle
    }

    public override void Update() {
        //verifica se o jogador entrou no range de detecção
        float distToPlayer = Vector2.Distance(boss.transform.position, boss.Player.position);

        if (distToPlayer <= boss.detectionRange)
        {
            boss.ChangeState(boss.ChaseState);
        }
    }

    public override void Exit() { }
}