using Unity.VisualScripting;
using UnityEngine;

public abstract class BossState {
    protected BossController boss;

    public BossState(BossController boss) {
    this.boss = boss;
    }


    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();


}
