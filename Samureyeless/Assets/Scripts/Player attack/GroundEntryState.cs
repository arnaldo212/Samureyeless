using UnityEngine;

public class GroundEntryState : MeleeBaseState
{
    public override void OnEnter(StateMachine _statemachine) {
        base.OnEnter(_statemachine);

        //atack
        attackIndex = 1;
        duration = 0.5f;
        anim.SetTrigger("FrontAttack" + attackIndex);
        Debug.Log("Player attack" + 1 + " fired");
    }

    public override void OnUpdate() {
        base.OnUpdate();
        
        if(fixedtime >= duration)
        {
            if (shouldCombo)
            {
                stateMachine.SetNextState(new GroundComboState());
            }
            else
            {
                stateMachine.SetNextStateToMain();
            }
        }
    }
}
