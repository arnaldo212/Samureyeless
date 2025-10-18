using UnityEngine;

public class UpperEntryState : UpperBaseState
{
    public override void OnEnter(StateMachine _stateMachine) {
        base.OnEnter(_stateMachine);

        attackDamage = 10;
        attackIndex = 1;
        duration = 0.5f;

        anim.SetTrigger("UpAttack" + attackIndex);
        Debug.Log("Upper attack " + attackIndex + " fired");
    }

    public override void OnUpdate() {
        base.OnUpdate();

        if (fixedtime >= duration)
        {
            if (shouldCombo)
            {
                stateMachine.SetNextState(new UpperFinisherState());
            }
            else
            {
                stateMachine.SetNextStateToMain();
            }
        }
    }
}
