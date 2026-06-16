using UnityEngine;
using System.Collections;

public class BossStunState : BossState
{
    private float stunDuration;

    public BossStunState(BossController boss, float duration) : base(boss) {
        stunDuration = duration;
    }

    public override void Enter() {
        Debug.Log("Boss: Stunado por parry!");
        boss.Rb.linearVelocity = Vector2.zero;
        boss.StartCoroutine(StunRoutine());
    }

    private IEnumerator StunRoutine() {
        //adicionar animação de stun
        yield return new WaitForSeconds(stunDuration);
        boss.ChangeState(boss.ChaseState);
    }

    public override void Update() {
        boss.Rb.linearVelocity = Vector2.zero; // garante que fica parado
    }

    public override void Exit() {
        Debug.Log("Boss: Stun acabou");
    }
}