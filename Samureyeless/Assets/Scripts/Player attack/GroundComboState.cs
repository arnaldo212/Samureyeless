using UnityEngine;

public class GroundComboState : MeleeBaseState
{
    public override void OnEnter(StateMachine _statemachine) {
        base.OnEnter(_statemachine);

        //atack
        attackIndex = 2;
        duration = 0.5f;
        anim.SetTrigger("FrontAttack" + attackIndex);
        Debug.Log("Player attack" + 2 + " fired");
    }

    public override void OnUpdate() {
        base.OnUpdate();

        if (fixedtime >= duration)
        {
            if (shouldCombo)
            {
                stateMachine.SetNextState(new GroundFinisherState());
            }
            else
            {
                stateMachine.SetNextStateToMain();
            }
        }
    }
}
