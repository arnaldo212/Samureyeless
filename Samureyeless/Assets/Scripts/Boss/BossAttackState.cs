using UnityEngine;

using System.Collections;

public class BossAttackState : BossState
{
    private Coroutine attackCoroutine;
    public BossAttackState(BossController boss) : base(boss) { }

    public override void Enter() {
        Debug.Log("Boss: Attacking");
        boss.Rb.linearVelocity = Vector2.zero; // para o boss

        // vira para o jogador antes de atacar
        float dir = boss.Player.position.x - boss.transform.position.x;
        boss.FaceDirection(dir);

        attackCoroutine = boss.StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine() {

        // fazer animação de ataque
        //aplicar dano ao jogador se estiver no range (fazer isso no meio da animação via animation event de preferência)
        //yield return new WaitForSeconds(boss.attackDuration * 0.5f);//espera um pouco antes de aplicar o
        // telegraph: hitbox NÃO habilitada ainda, só avisa visualmente (animação/cor)
        yield return new WaitForSeconds(boss.telegraphDuration);

        // abre janela de parry antes do ataque
        boss.isParryWindow = true;
        boss.HitBox.EnableHitbox();
        Debug.Log("PARRY WINDOW ABERTA");
        yield return new WaitForSeconds(boss.parryWindowDuration);

        boss.isParryWindow = false;
        Debug.Log("PARRY WINDOW FECHADA");

        // se foi parried durante a janela, a coroutine já foi interrompida


        Debug.Log("Boss atacou!");

        yield return new WaitForSeconds(boss.attackDuration * 0.5f);

        boss.HitBox.DisableHitbox();

        yield return new WaitForSeconds(boss.attackCooldown);

        //Volta a perseguir após o ataque
        boss.ChangeState(boss.ChaseState);
    }


    public override void Update() {
        //exemplo jogador saiu do range durante o ataque? 
        
        //mantém o boss parado durante o ataque
        boss.Rb.linearVelocity = new Vector2(0, boss.Rb.linearVelocity.y);
    }

    public override void Exit() {
        if (attackCoroutine != null)
        {
            boss.StopCoroutine(attackCoroutine); // garante que a coroutine antiga não continua correndo
            attackCoroutine = null;
        }
        boss.isParryWindow = false; // garante reset
        boss.HitBox.DisableHitbox();
    }
}
