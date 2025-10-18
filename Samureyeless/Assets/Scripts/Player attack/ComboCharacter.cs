using UnityEngine;

public class ComboCharacter : MonoBehaviour
{
    private StateMachine meleeStateMachine;

    [SerializeField] public Collider2D hitbox;
    [SerializeField] public Collider2D upHitbox;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        meleeStateMachine = GetComponent<StateMachine>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && meleeStateMachine.CurrentState.GetType() == typeof(IdleCombatState)) {
            meleeStateMachine.SetNextState(new GroundEntryState());
        }

        // ataque pra cima (por exemplo, segurando W ou seta pra cima)
        if (Input.GetKey(KeyCode.W) && Input.GetMouseButtonDown(0) && meleeStateMachine.CurrentState.GetType() == typeof(IdleCombatState))
        {
            meleeStateMachine.SetNextState(new UpperEntryState());
        }

    }
}
