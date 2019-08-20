using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCStateIdle : NPCStateBase
{
    float PatrolCD; //进入巡逻所需时间
    float PatrolTime; //当前累计时间

    public override void OnInit()
    {
        base.OnInit();
        npcState = NPCState.Idle;
        aniName = "Idle";
    }

    public override void OnEnter()
    {
        //播放对应动画
        animator.SetInteger("Move", 0);

        //随机下次巡逻所需时间
        PatrolCD = Random.Range(manager.IdleCDMin, manager.IdleCDMax);
    }

    public override void OnExcute()
    {
        //状态保护 进入动画之后才执行
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(aniName))
            return;

        PatrolTime += Time.deltaTime; //累计时间

        if (PatrolTime > PatrolCD)
        {
            PatrolTime = 0; //归零累计时间
            //切换到 巡逻状态
            if (manager.ChangeState<NPCStatePatrol>())
                return;
        }
    }

    public override void OnExit()
    {
        
    }
}
