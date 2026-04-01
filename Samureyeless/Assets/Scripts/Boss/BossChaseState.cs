using UnityEngine;
using UnityEngine.UIElements;

public class BossChaseState : BossState
{
    public BossChaseState(BossController boss) : base(boss) { }

    public override void Enter() {
        Debug.Log("Boss: Chasing");
        // Toca animação de corrida/movimento
    }

    public override void Update() {
        float distToPlayer = Vector2.Distance(boss.transform.position, boss.Player.position);

        // Chegou perto o suficiente para atacar?
        if (distToPlayer <= boss.attackRange)
        {
            boss.ChangeState(boss.AttackState);
            return;
        }

        // Perdeu o jogador de vista?
        if (distToPlayer > boss.detectionRange)
        {
            boss.ChangeState(boss.IdleState);
            return;
        }

        // Move em direção ao jogador
        Vector2 direction = (boss.Player.position - boss.transform.position).normalized;
        boss.Rb.linearVelocity = new Vector2(direction.x * boss.moveSpeed, boss.Rb.linearVelocity.y);

        // Vira o sprite na direção certa
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
}
