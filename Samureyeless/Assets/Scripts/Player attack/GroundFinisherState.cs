using UnityEngine;

public class GroundFinisherState : MeleeBaseState
{
    public override void OnEnter(StateMachine _statemachine) {
        base.OnEnter(_statemachine);

        //atack
        attackIndex = 3;
        duration = 0.75f;
        anim.SetTrigger("FrontAttack" + attackIndex);
        Debug.Log("Player attack" + 3 + " fired");
    }

    public override void OnUpdate() {
        base.OnUpdate();

        if (fixedtime >= duration)
        {
            if (shouldCombo)
            {
                stateMachine.SetNextStateToMain();
            }
        }
    }
}
