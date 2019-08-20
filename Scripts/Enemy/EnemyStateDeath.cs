using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateDeath : EnemyStateBase
{
    public override void OnInit()
    {
        base.OnInit();
        enemyState = EnemyState.Death;
        aniName = "Death";

        speed = 5.0f;
    }

    public override void OnEnter()
    {
        //播放对应动画
        animator.SetBool("Death", true);
    }

    public override void OnExcute()
    {
        Gravity();

        //状态保护 进入动画之后才执行
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(aniName))
            return;

        //应用动画位移

        //死亡动画结束后
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f)
        {
            //关闭角色脚本 缓存池回收UI
            enemy.LateDeath();
        }
    }

    public override void OnExit()
    {

    }
}
