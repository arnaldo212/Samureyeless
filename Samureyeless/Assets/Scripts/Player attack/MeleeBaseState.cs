using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class MeleeBaseState : State
{
    //how long this state should be active for before moving on
    public float duration;
    //cached animator component
    protected Animator anim;
    //bool to check whether or not the next attack in the sequence should be played or not
    protected bool shouldCombo;
    //the attack index in the sequence of attacks
    protected int attackIndex;

    //the cached hit collider of this attack
    protected Collider2D hitBox;
    //cached already struck objects of said attack to avoid overlapping attacks on same target
    private List<Collider2D> collidersDamaged;
    //input buffer timer
    private float attackPressedTimer = 0;

    public float attackDamage;
    
    public override void OnEnter(StateMachine _stateMachine) {
        base.OnEnter(_stateMachine);
        anim = GetComponent<Animator>();
        collidersDamaged = new List<Collider2D>();
        ComboCharacter comboChar = GetComponent<ComboCharacter>();
        hitBox = comboChar.hitbox;
    }

    public override void OnUpdate() {
        base.OnUpdate();
        attackPressedTimer -= Time.deltaTime;

        PlayerStateList pState = GetComponent<PlayerStateList>();

        if (anim.GetFloat("Weapon.Active") > 0f)
        {
            Attack();
            if(pState != null) pState.parrying = true; // está atacando = pode fazer parry
        }
        else
        {
            if (pState != null) pState.parrying = false;
        }

        if (Input.GetMouseButtonDown(0)) {
            attackPressedTimer = 1;
        }

        if (anim.GetFloat("Attack.Window.Open") > 0f && attackPressedTimer > 0)
        {
            shouldCombo = true;
        }
    }

    public override void OnExit() {
        base.OnExit();

        shouldCombo = false;
        attackPressedTimer = 0;

        PlayerStateList pState = GetComponent<PlayerStateList>();
        if (pState != null) pState.parrying = false;
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

                //only check colliders with a valid team comoponent attached
                if(hitTeamComponent && hitTeamComponent.teamindex == TeamIndex.Enemy)
                {
                    //encontra o componente Enemy no objeto atingido
                    Enemy enemy = collidersToDamage[i].GetComponent<Enemy>();
                    if(enemy != null)
                    {
                        enemy.EnemyHit(attackDamage);
                        Debug.Log("Enemy has taken:" + attackIndex + "Damage");
                    }
                        collidersDamaged.Add(collidersToDamage[i]);
                }
            }
        }
    }
}
