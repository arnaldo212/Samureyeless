using System.Collections.Generic;
using UnityEngine;

public class UpperBaseState : State
{
    public float duration;
    protected Animator anim;
    protected bool shouldCombo;
    protected int attackIndex;

    protected Collider2D hitBox;
    private List<Collider2D> collidersDamaged;
    public float attackDamage;

    public override void OnEnter(StateMachine _stateMachine) {
        base.OnEnter(_stateMachine);
        anim = GetComponent<Animator>();
        collidersDamaged = new List<Collider2D>();

        ComboCharacter comboChar = GetComponent<ComboCharacter>();
        hitBox = comboChar.upHitbox; // usamos o hitbox de ataque para cima
    }

    public override void OnUpdate() {
        base.OnUpdate();

        Attack();

        if (Input.GetMouseButtonDown(0))
        {
            shouldCombo = true;
        }
    }

    protected void Attack() {
        Collider2D[] collidersToDamage = new Collider2D[10];
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true;

        int colliderCount = Physics2D.OverlapCollider(hitBox, filter, collidersToDamage);

        for (int i = 0; i < colliderCount; i++)
        {
            if (!collidersDamaged.Contains(collidersToDamage[i]))
            {
                TeamComponent hitTeamComponent = collidersToDamage[i].GetComponentInChildren<TeamComponent>();
                if (hitTeamComponent && hitTeamComponent.teamindex == TeamIndex.Enemy)
                {
                    Enemy enemy = collidersToDamage[i].GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.EnemyHit(attackDamage);
                        Debug.Log("Enemy hit by UP attack " + attackIndex + " for " + attackDamage + " damage");
                    }
                    collidersDamaged.Add(collidersToDamage[i]);
                }
            }
        }
    }
}
