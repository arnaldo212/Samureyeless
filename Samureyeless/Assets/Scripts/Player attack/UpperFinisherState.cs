using UnityEngine;

public class UpperFinisherState : UpperBaseState
{
    public override void OnEnter(StateMachine _stateMachine) {
        base.OnEnter(_stateMachine);

        attackDamage = 15;
        attackIndex = 2;
        duration = 0.7f;

        anim.SetTrigger("UpAttack" + attackIndex);
        Debug.Log("Upper attack " + attackIndex + " fired (Finisher)");
    }

    public override void OnUpdate() {
        base.OnUpdate();

        if (fixedtime >= duration)
        {
            stateMachine.SetNextStateToMain();
        }
    }
}
