using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateDeath : PlayerStateBase
{
    public override void OnInit()
    {
        base.OnInit();
        playerState = PlayerState.Death; //记录状态
        aniName = "Death"; //记录动画名
    }

    public override void OnEnter()
    {
        //播放对应动画
        animator.SetBool(aniName, true);
    }

    public override void OnControl()
    {
        //状态保护 进入动画后才执行方法
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(aniName))
            return;
    }

    public override void OnExcute()
    {
        Gravity(); //模拟重力

        //状态保护 进入动画后才执行方法
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(aniName))
            return;

        //应用动画位移
    }

    public override void OnExit()
    {
        
    }
}
