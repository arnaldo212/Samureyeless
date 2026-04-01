using UnityEngine;

using System.Collections;

public class BossAttackState : BossState
{
    private bool isAttacking = false;

    public BossAttackState(BossController boss) : base(boss) { }

    public override void Enter() {
        Debug.Log("Boss: Attacking");
        isAttacking = false;
        boss.StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine() {
        isAttacking = true;

        // fazer animação de ataque
        //aplicar dano ao jogador se estiver no range (fazer isso no meio da animação via animation event de preferência)
        Debug.Log("Boss atacou!");

        yield return new WaitForSeconds(boss.attackDuration);

        isAttacking = false;

        //Volta a perseguir após o ataque
        boss.ChangeState(boss.ChaseState);
    }

    public override void Update() {
        //exemplo jogador saiu do range durante o ataque? 
    }

    public override void Exit() {
        isAttacking = false;
    }
}
